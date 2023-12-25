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

            Debug.Log("触摸位置: " + touch.position);

            Vector3 touchPosition = mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, mainCamera.nearClipPlane));

            Debug.Log("世界坐标位置: " + touchPosition);

            Vector3 directionToLook = touchPosition - headTransform.position;

            Debug.Log("朝向方向: " + directionToLook);

            headTransform.rotation = Quaternion.LookRotation(directionToLook);

            Debug.Log("头部旋转: " + headTransform.rotation);
        }
    }
}
