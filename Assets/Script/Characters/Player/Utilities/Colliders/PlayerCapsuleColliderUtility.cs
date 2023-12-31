using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AILive
{
    [Serializable]
    public class PlayerCapsuleColliderUtility : CapsuleColliderUtility
    {
        [field:SerializeField]public PlayerTriggerColliderData TriggerColliderData {  get; private set; }
    }
}
