using UnityEngine;

public class SimpleRotator : MonoBehaviour
{
    public float rotationSpeed = 50f; // ÿ����ת�Ķ���

    void Update()
    {
        // ��Y����ת��ÿ��rotationSpeed��
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
