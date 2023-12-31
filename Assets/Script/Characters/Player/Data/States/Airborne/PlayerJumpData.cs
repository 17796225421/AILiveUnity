using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AILive
{
    [Serializable]
    public class PlayerJumpData
    {
        [field:SerializeField]public Vector3 StationaryForce {  get;private set; }
        [field:SerializeField]public Vector3 WeakForce {  get;private set; }
        [field:SerializeField]public Vector3 MediumForce {  get;private set; }
        [field:SerializeField]public Vector3 StrongForce {  get; private set; }
    }
}