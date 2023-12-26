using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.ObjdetectModule;
using OpenCVForUnity.ImgprocModule;

public class EyeDetection : MonoBehaviour
{
    public WebCamTexture webCamTexture;
    public CascadeClassifier eyesCascade;
    //public RawImage rawImage;  // 把你在Unity创建的RawImage拖拽到这里
    // 添加这两个公共字段
    public int eyeX;
    public int eyeY;

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

        webCamTexture = new WebCamTexture(frontCamName);
        //rawImage.texture = webCamTexture;  // 将摄像头图像赋给RawImage
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
    }

    void Update()
    {
        Mat frame = new Mat(webCamTexture.height, webCamTexture.width, CvType.CV_8UC4); // 修改这里
        Utils.webCamTextureToMat(webCamTexture, frame);

        // 旋转图像
        Core.rotate(frame, frame, Core.ROTATE_90_COUNTERCLOCKWISE);

        Mat gray = new Mat();
        Imgproc.cvtColor(frame, gray, Imgproc.COLOR_RGBA2GRAY); // 修改这里

        MatOfRect eyes = new MatOfRect();
        eyesCascade.detectMultiScale(gray, eyes);

        Debug.Log("检测到眼睛的数量: " + eyes.toArray().Length);

        foreach (OpenCVForUnity.CoreModule.Rect eye in eyes.toArray())
        {
            // 在这里，你可以获取到眼睛的位置
            Debug.Log("检测到眼睛，位置：" + eye.x + ", " + eye.y);

            // 将眼睛的位置赋值给公共字段
            eyeX = eye.x;
            eyeY = eye.y;

            //// 在眼睛的位置上绘制红框
            //Imgproc.rectangle(frame, new Point(eye.x, eye.y), new Point(eye.x + eye.width, eye.y + eye.height), new Scalar(255, 0, 0, 255), 2);
        }

        //// 将带有红框的图像显示在Unity的RawImage上
        //Texture2D texture = new Texture2D(frame.cols(), frame.rows(), TextureFormat.RGBA32, false);
        //Utils.matToTexture2D(frame, texture);
        //rawImage.texture = texture;

        frame.release();
        gray.release();
    }
}
