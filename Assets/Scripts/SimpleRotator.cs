using UnityEngine;

public class SimpleRotator : MonoBehaviour
{
    public float rotationSpeed = 50f; // 每秒旋转的度数

    void Update()
    {
        // 绕Y轴旋转，每秒rotationSpeed度
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
