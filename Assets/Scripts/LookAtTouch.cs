using UnityEngine;

public class LookAtTouch : MonoBehaviour
{
    public Camera mainCamera;
    public Transform headTransform;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // �����������Ļ����ת��Ϊ��������
            Vector3 touchPosition = new Vector3(touch.position.x, touch.position.y, 10f);
            touchPosition.x =0.96f;
            touchPosition.y =0f;
            touchPosition.z =10f;
            // �ý�ɫ��ͷ����������
            Debug.Log("ת������۾�λ�ã�" + touchPosition);
            headTransform.LookAt(touchPosition);
        }
    }
}
