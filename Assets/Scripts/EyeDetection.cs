using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.ObjdetectModule;
using OpenCVForUnity.ImgprocModule;
using System.Collections.Concurrent;

public class EyeDetection : MonoBehaviour
{
    public WebCamTexture webCamTexture;
    public CascadeClassifier eyesCascade;
    private const int ArraySize = 1000;  // 定义数组大小为常量
    private const int WindowSize = 10;  // 滑动窗口的大小

    public Vector2[] eyePositions = new Vector2[ArraySize];  // 存储眼睛位置的数组
    public int currentIndex = 0;  // 当前的索引，需要线程安全

    private Vector2[] window = new Vector2[WindowSize];  // 滑动窗口
    private int windowIndex = 0;  // 滑动窗口的索引
    private Vector2 windowSum = Vector2.zero;  // 滑动窗口的累积和
    private Thread thread;
    private ConcurrentQueue<Mat> frameQueue = new ConcurrentQueue<Mat>();

    void Start()
    {
        string frontCamName = "";
        foreach (var device in WebCamTexture.devices)
        {
            Debug.Log("摄像头名称: " + device.name);
            if (device.isFrontFacing)
            {
                frontCamName = device.name;
                break;
            }
        }
        Debug.Log("前置摄像头名称: " + frontCamName);

        webCamTexture = new WebCamTexture(frontCamName, 4 * 75, 3 * 75);
        webCamTexture.Play();

        eyesCascade = new CascadeClassifier();

        // 加载级联分类器
        TextAsset cascadeAsset = Resources.Load<TextAsset>("haarcascade_eye");
        if (cascadeAsset != null)
        {
            string tempFilePath = Application.temporaryCachePath + "/haarcascade_eye.xml";
            System.IO.File.WriteAllBytes(tempFilePath, cascadeAsset.bytes);
            bool loaded = eyesCascade.load(tempFilePath);
            Debug.Log("级联分类器加载" + (loaded ? "成功" : "失败"));
        }
        else
        {
            Debug.Log("无法加载haarcascade_eye.xml");
        }

        // 检查级联分类器是否为空
        if (eyesCascade.empty())
        {
            Debug.Log("级联分类器为空");
        }
        else
        {
            Debug.Log("级联分类器已加载");
        }

        // 创建一个后台线程来执行眼睛检测和滑动窗口的计算
        thread = new Thread(DetectEyes);
        thread.IsBackground = true;
        thread.Start();

        // 开始执行协程
        StartCoroutine(ProcessFrame());
    }

    IEnumerator ProcessFrame()
    {
        while (true)
        {
            // 在主线程中捕获摄像头的图像
            Mat frame = new Mat(webCamTexture.height, webCamTexture.width, CvType.CV_8UC4);
            Utils.webCamTextureToMat(webCamTexture, frame);
            frameQueue.Enqueue(frame);  // 将图像数据添加到队列中

            // 等待x秒
            yield return new WaitForSeconds(0.5f);
        }
    }

    void DetectEyes()
    {
        while (true)
        {
            // 从队列中取出图像数据
            if (!frameQueue.TryDequeue(out Mat frame))
            {
                Thread.Sleep(100);
                continue;
            }

            // 旋转图像
            Core.rotate(frame, frame, Core.ROTATE_90_COUNTERCLOCKWISE);
            
            // 镜面反转图像
            Core.flip(frame, frame, 1);  // 1 表示沿着垂直轴反转

            Mat gray = new Mat();
            Imgproc.cvtColor(frame, gray, Imgproc.COLOR_RGBA2GRAY); // 修改这里

            MatOfRect eyes = new MatOfRect();
            eyesCascade.detectMultiScale(gray, eyes);

            Debug.Log("检测到眼睛的数量: " + eyes.toArray().Length);

            foreach (OpenCVForUnity.CoreModule.Rect eye in eyes.toArray())
            {
                // 在这里，你可以获取到眼睛的位置
                Debug.Log("检测到眼睛，位置：" + eye.x + ", " + eye.y);

                // 将眼睛的位置转换为以图像中心为原点的坐标
                int centerX = frame.width() / 2;
                int centerY = frame.height() / 2;
                int eyeCenterX = eye.x + eye.width / 2;
                int eyeCenterY = eye.y + eye.height / 2;
                int eyeX = eyeCenterX - centerX;
                int eyeY = centerY - eyeCenterY;  // 注意这里取了反

                Debug.Log("转换后的眼睛位置：" + eyeX + ", " + eyeY);

                // 更新滑动窗口的累积和
                Vector2 newEyePos = new Vector2(eyeX, eyeY);
                windowSum = windowSum - window[windowIndex] + newEyePos;

                // 将新的眼睛位置添加到滑动窗口
                window[windowIndex] = newEyePos;
                windowIndex = (windowIndex + 1) % WindowSize;

                // 计算滑动窗口中眼睛位置的平均值
                Vector2 averageEyePos = windowSum / WindowSize;

                // 打印平均眼睛位置
                Debug.Log("平均眼睛位置：" + averageEyePos);

                // 将平均的眼睛位置添加到数组中
                lock (eyePositions)  // 确保线程安全
                {
                    eyePositions[currentIndex] = averageEyePos;
                    currentIndex = (currentIndex + 1) % ArraySize;
                }
            }
        }
    }
}
