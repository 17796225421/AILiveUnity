using UnityEngine;

public class FaceFollowEyePosition : MonoBehaviour
{
    public EyeDetection eyeDetection;  // EyeDetection�ű�������
    public Transform faceTransform;  // ����������Transform
    public float moveSpeed = 0.2f;  // �ƶ��ٶ�
    public float randomRange = 30f;  // ���ƫ�Ʒ�Χ

    private int readIndex = 0;  // ��ȡ���ݵ�����
    private Vector3 targetPosition;  // Ŀ��λ��
    private Vector3 lastVector;

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

            // ����Ŀ��λ��
            targetPosition = new Vector3(eyePosition.x, eyePosition.y, 5000f);

            // ���¶�ȡ���ݵ�����
            readIndex = (readIndex + 1) % EyeDetection.ArraySize;
        }

        // ������ƫ��
        Vector3 randomOffset = new Vector3(Random.Range(-randomRange, randomRange), Random.Range(-randomRange, randomRange), 0);

        Vector3 nextVector = Vector3.Lerp(lastVector, targetPosition + randomOffset, moveSpeed);
        lastVector = nextVector;
        Debug.Log("��ǰ������" + lastVector + "��һ������" + nextVector + "Ŀ������: " + targetPosition);
        // ƽ���ƶ�
        faceTransform.LookAt(nextVector);
    }
}
