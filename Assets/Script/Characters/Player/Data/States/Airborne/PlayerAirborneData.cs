using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AILive
{
    [Serializable]
    public class PlayerAirborneData
    {
        [field:SerializeField]public PlayerJumpData JumpData {  get;private set; }
    }
}