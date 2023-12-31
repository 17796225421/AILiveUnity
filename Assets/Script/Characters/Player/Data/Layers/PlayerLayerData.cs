using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace AILive
{
    [Serializable]
    public class PlayerLayerData
    {
        [field:SerializeField]public LayerMask GroundLayer {  get;private set; }

        public bool ContainLayer(LayerMask layerMask,int layer)
        {
            return (1 << layer & layerMask) != 0;
        }

        public bool IsGroundLayer(int layer)
        {
            return ContainLayer(GroundLayer,layer);
        }
    }
}
