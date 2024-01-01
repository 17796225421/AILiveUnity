using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AILive
{
    [Serializable]
    public class PlayerIdleData
    {
        [field:SerializeField]public List<PlayerCameraRecenteringData> BackwardsCameraTecenteringData { get;private set; }
    }
}
