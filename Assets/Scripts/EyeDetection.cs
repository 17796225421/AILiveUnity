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
    //public RawImage rawImage;  // ������Unity������RawImage��ק������
    // ��������������ֶ�
    public int eyeX;
    public int eyeY;

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

        webCamTexture = new WebCamTexture(frontCamName);
        //rawImage.texture = webCamTexture;  // ������ͷͼ�񸳸�RawImage
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
    }

    void Update()
    {
        Mat frame = new Mat(webCamTexture.height, webCamTexture.width, CvType.CV_8UC4); // �޸�����
        Utils.webCamTextureToMat(webCamTexture, frame);

        // ��תͼ��
        Core.rotate(frame, frame, Core.ROTATE_90_COUNTERCLOCKWISE);

        Mat gray = new Mat();
        Imgproc.cvtColor(frame, gray, Imgproc.COLOR_RGBA2GRAY); // �޸�����

        MatOfRect eyes = new MatOfRect();
        eyesCascade.detectMultiScale(gray, eyes);

        Debug.Log("��⵽�۾�������: " + eyes.toArray().Length);

        foreach (OpenCVForUnity.CoreModule.Rect eye in eyes.toArray())
        {
            // ���������Ի�ȡ���۾���λ��
            Debug.Log("��⵽�۾���λ�ã�" + eye.x + ", " + eye.y);

            // ���۾���λ�ø�ֵ�������ֶ�
            eyeX = eye.x;
            eyeY = eye.y;

            //// ���۾���λ���ϻ��ƺ��
            //Imgproc.rectangle(frame, new Point(eye.x, eye.y), new Point(eye.x + eye.width, eye.y + eye.height), new Scalar(255, 0, 0, 255), 2);
        }

        //// �����к���ͼ����ʾ��Unity��RawImage��
        //Texture2D texture = new Texture2D(frame.cols(), frame.rows(), TextureFormat.RGBA32, false);
        //Utils.matToTexture2D(frame, texture);
        //rawImage.texture = texture;

        frame.release();
        gray.release();
    }
}
