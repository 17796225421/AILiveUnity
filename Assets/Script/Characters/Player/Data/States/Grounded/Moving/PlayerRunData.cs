using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AILive
{
    [Serializable]
    public class PlayerRunData 
    {
        [field: SerializeField][field: Range(1f, 2f)] public float SpeedModifier { get; private set; } = 1f;
    }
}
