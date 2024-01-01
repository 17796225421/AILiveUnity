using System;
using Unity.VisualScripting;
using UnityEngine;

namespace AILive
{
    public class PlayerIdlingState : PlayerGroundedState
    {
        private PlayerIdleData idleData;
        public PlayerIdlingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            idleData = movementData.IdleData;
        }
        #region IState Methods
        public override void Enter()
        {
            stateMachine.ReusableData.MovementSpeedModifier = 0f;

            stateMachine.ReusableData.BackwardsCameraRecenteringData = idleData.BackwardsCameraTecenteringData;

            base.Enter();

            stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.StationaryForce;

            ResetVelocity();
        }

        public override void Update()
        {
            base.Update();

            if(stateMachine.ReusableData.MovementInput == Vector2.zero)
            {
                return;
            }

            OnMove();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            if (!IsMovingHorizontally())
            {
                return;
            }

            ResetVelocity();
        }

        #endregion
    }
}
