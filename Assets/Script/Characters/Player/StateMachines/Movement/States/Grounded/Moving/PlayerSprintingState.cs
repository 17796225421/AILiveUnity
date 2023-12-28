using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AILive
{
    public class PlayerSprintingState : PlayerMovementState
    {
        public PlayerSprintingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }
    }
}
