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
    private const int ArraySize = 1000;  // ���������СΪ����
    private const int WindowSize = 10;  // �������ڵĴ�С

    public Vector2[] eyePositions = new Vector2[ArraySize];  // �洢�۾�λ�õ�����
    public int currentIndex = 0;  // ��ǰ����������Ҫ�̰߳�ȫ

    private Vector2[] window = new Vector2[WindowSize];  // ��������
    private int windowIndex = 0;  // �������ڵ�����
    private Vector2 windowSum = Vector2.zero;  // �������ڵ��ۻ���
    private Thread thread;
    private ConcurrentQueue<Mat> frameQueue = new ConcurrentQueue<Mat>();

    void Start()
    {
        string frontCamName = "";
        foreach (var device in WebCamTexture.devices)
        {
            Debug.Log("����ͷ����: " + device.name);
            if (device.isFrontFacing)
            {
                frontCamName = device.name;
                break;
            }
        }
        Debug.Log("ǰ������ͷ����: " + frontCamName);

        webCamTexture = new WebCamTexture(frontCamName, 4 * 75, 3 * 75);
        webCamTexture.Play();

        eyesCascade = new CascadeClassifier();

        // ���ؼ���������
        TextAsset cascadeAsset = Resources.Load<TextAsset>("haarcascade_eye");
        if (cascadeAsset != null)
        {
            string tempFilePath = Application.temporaryCachePath + "/haarcascade_eye.xml";
            System.IO.File.WriteAllBytes(tempFilePath, cascadeAsset.bytes);
            bool loaded = eyesCascade.load(tempFilePath);
            Debug.Log("��������������" + (loaded ? "�ɹ�" : "ʧ��"));
        }
        else
        {
            Debug.Log("�޷�����haarcascade_eye.xml");
        }

        // ��鼶���������Ƿ�Ϊ��
        if (eyesCascade.empty())
        {
            Debug.Log("����������Ϊ��");
        }
        else
        {
            Debug.Log("�����������Ѽ���");
        }

        // ����һ����̨�߳���ִ���۾����ͻ������ڵļ���
        thread = new Thread(DetectEyes);
        thread.IsBackground = true;
        thread.Start();

        // ��ʼִ��Э��
        StartCoroutine(ProcessFrame());
    }

    IEnumerator ProcessFrame()
    {
        while (true)
        {
            // �����߳��в�������ͷ��ͼ��
            Mat frame = new Mat(webCamTexture.height, webCamTexture.width, CvType.CV_8UC4);
            Utils.webCamTextureToMat(webCamTexture, frame);
            frameQueue.Enqueue(frame);  // ��ͼ��������ӵ�������

            // �ȴ�x��
            yield return new WaitForSeconds(0.5f);
        }
    }

    void DetectEyes()
    {
        while (true)
        {
            // �Ӷ�����ȡ��ͼ������
            if (!frameQueue.TryDequeue(out Mat frame))
            {
                Thread.Sleep(100);
                continue;
            }

            // ��תͼ��
            Core.rotate(frame, frame, Core.ROTATE_90_COUNTERCLOCKWISE);
            
            // ���淴תͼ��
            Core.flip(frame, frame, 1);  // 1 ��ʾ���Ŵ�ֱ�ᷴת

            Mat gray = new Mat();
            Imgproc.cvtColor(frame, gray, Imgproc.COLOR_RGBA2GRAY); // �޸�����

            MatOfRect eyes = new MatOfRect();
            eyesCascade.detectMultiScale(gray, eyes);

            Debug.Log("��⵽�۾�������: " + eyes.toArray().Length);

            foreach (OpenCVForUnity.CoreModule.Rect eye in eyes.toArray())
            {
                // ���������Ի�ȡ���۾���λ��
                Debug.Log("��⵽�۾���λ�ã�" + eye.x + ", " + eye.y);

                // ���۾���λ��ת��Ϊ��ͼ������Ϊԭ�������
                int centerX = frame.width() / 2;
                int centerY = frame.height() / 2;
                int eyeCenterX = eye.x + eye.width / 2;
                int eyeCenterY = eye.y + eye.height / 2;
                int eyeX = eyeCenterX - centerX;
                int eyeY = centerY - eyeCenterY;  // ע������ȡ�˷�

                Debug.Log("ת������۾�λ�ã�" + eyeX + ", " + eyeY);

                // ���»������ڵ��ۻ���
                Vector2 newEyePos = new Vector2(eyeX, eyeY);
                windowSum = windowSum - window[windowIndex] + newEyePos;

                // ���µ��۾�λ����ӵ���������
                window[windowIndex] = newEyePos;
                windowIndex = (windowIndex + 1) % WindowSize;

                // ���㻬���������۾�λ�õ�ƽ��ֵ
                Vector2 averageEyePos = windowSum / WindowSize;

                // ��ӡƽ���۾�λ��
                Debug.Log("ƽ���۾�λ�ã�" + averageEyePos);

                // ��ƽ�����۾�λ����ӵ�������
                lock (eyePositions)  // ȷ���̰߳�ȫ
                {
                    eyePositions[currentIndex] = averageEyePos;
                    currentIndex = (currentIndex + 1) % ArraySize;
                }
            }
        }
    }
}
