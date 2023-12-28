using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AILive
{
    public class PlayerIdlingState : PlayerMovementState
    {
        public PlayerIdlingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }
    }
}
