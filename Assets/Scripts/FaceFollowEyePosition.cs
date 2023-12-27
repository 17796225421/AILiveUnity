using UnityEngine;

public class FaceFollowEyePosition : MonoBehaviour
{
    public EyeDetection eyeDetection;  // EyeDetection脚本的引用
    public Transform faceTransform;  // 人物脸部的Transform
    public float moveSpeed = 0.2f;  // 移动速度
    public float randomRange = 30f;  // 随机偏移范围

    private int readIndex = 0;  // 读取数据的索引
    private Vector3 targetPosition;  // 目标位置
    private Vector3 lastVector;

    void Update()
    {
        int currentIndex;

        // 获取当前的currentIndex
        lock (eyeDetection.currentIndexLock)
        {
            currentIndex = eyeDetection.currentIndex;
        }

        // 如果有新的数据
        if (readIndex != currentIndex)
        {
            // 获取眼睛的位置
            Vector2 eyePosition = eyeDetection.eyePositions[readIndex];

            // 设置目标位置
            targetPosition = new Vector3(eyePosition.x, eyePosition.y, 5000f);

            // 更新读取数据的索引
            readIndex = (readIndex + 1) % EyeDetection.ArraySize;
        }

        // 添加随机偏移
        Vector3 randomOffset = new Vector3(Random.Range(-randomRange, randomRange), Random.Range(-randomRange, randomRange), 0);

        Vector3 nextVector = Vector3.Lerp(lastVector, targetPosition + randomOffset, moveSpeed);
        lastVector = nextVector;
        Debug.Log("当前向量：" + lastVector + "下一个向量" + nextVector + "目标向量: " + targetPosition);
        // 平滑移动
        faceTransform.LookAt(nextVector);
    }
}
