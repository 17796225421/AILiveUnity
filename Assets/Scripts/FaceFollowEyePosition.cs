using UnityEngine;

public class FaceFollowEyePosition : MonoBehaviour
{
    public EyeDetection eyeDetection;  // EyeDetection脚本的引用
    public Transform faceTransform;  // 人物脸部的Transform

    private int readIndex = 0;  // 读取数据的索引

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

            // 根据眼睛的位置调整人物的脸部方向
            // 注意：你可能需要根据你的游戏的具体情况来调整这部分的代码
            faceTransform.LookAt(new Vector3(eyePosition.x, eyePosition.y, faceTransform.position.z));

            // 更新读取数据的索引
            readIndex = (readIndex + 1) % EyeDetection.ArraySize;
        }
    }
}
