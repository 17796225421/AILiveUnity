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

            // 将触摸点从屏幕坐标转换为世界坐标
            Vector3 touchPosition = new Vector3(touch.position.x, touch.position.y, 10f);
            touchPosition.x =0.96f;
            touchPosition.y =0f;
            touchPosition.z =10f;
            // 让角色的头部面向触摸点
            Debug.Log("转换后的眼睛位置：" + touchPosition);
            headTransform.LookAt(touchPosition);
        }
    }
}
