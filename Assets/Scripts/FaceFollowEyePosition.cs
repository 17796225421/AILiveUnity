using UnityEngine;

public class FaceFollowEyePosition : MonoBehaviour
{
    public EyeDetection eyeDetection;  // EyeDetection�ű�������
    public Transform faceTransform;  // ����������Transform

    private int readIndex = 0;  // ��ȡ���ݵ�����

    void Update()
    {
        int currentIndex;

        // ��ȡ��ǰ��currentIndex
        lock (eyeDetection.currentIndexLock)
        {
            currentIndex = eyeDetection.currentIndex;
        }

        // ������µ�����
        if (readIndex != currentIndex)
        {
            // ��ȡ�۾���λ��
            Vector2 eyePosition = eyeDetection.eyePositions[readIndex];

            // �����۾���λ�õ����������������
            // ע�⣺�������Ҫ���������Ϸ�ľ�������������ⲿ�ֵĴ���
            faceTransform.LookAt(new Vector3(eyePosition.x, eyePosition.y, faceTransform.position.z));

            // ���¶�ȡ���ݵ�����
            readIndex = (readIndex + 1) % EyeDetection.ArraySize;
        }
    }
}
