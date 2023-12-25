using UnityEngine;

public class LookAtTouch : MonoBehaviour
{
    public Camera mainCamera;
    public Transform headTransform;
    public int a;
    public int b;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            Debug.Log("����λ��: " + touch.position);

            Vector3 touchPosition = mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, mainCamera.nearClipPlane));

            Debug.Log("��������λ��: " + touchPosition);

            Vector3 directionToLook = touchPosition - headTransform.position;

            Debug.Log("������: " + directionToLook);

            headTransform.rotation = Quaternion.LookRotation(directionToLook);

            Debug.Log("ͷ����ת: " + headTransform.rotation);
        }
    }
}
