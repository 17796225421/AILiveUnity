using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AILive
{
    public class PlayerGroundedState : PlayerMovementState
    {
        private SlopData slopData;
        public PlayerGroundedState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            slopData=stateMachine.Player.ColliderUtility.SlopData;
        }

        #region IState Methods
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            Float();
        }
        #endregion
        #region Main Methods
        private void Float()
        {
            Vector3 capsuleColliderCenterInWorldSpace = stateMachine.Player.ColliderUtility.CapsuleColliderData.Collider.bounds.center;

            Ray downWardsRayFromCapsuleCenter = new Ray(capsuleColliderCenterInWorldSpace, Vector3.down);

            if(Physics.Raycast(downWardsRayFromCapsuleCenter, out RaycastHit hit,slopData.FloatRayDistance,stateMachine.Player.LayerData.GroundLayer,QueryTriggerInteraction.Ignore))
            {
                float groundAngle = Vector3.Angle(hit.normal,-downWardsRayFromCapsuleCenter.direction);

                float slopeSpeedModifier = SetSlopSpeedModifierOnAngle(groundAngle);

                if (slopeSpeedModifier == 0f)
                {
                    return;
                }

                float distanceToFloatingPoint = stateMachine.Player.ColliderUtility.CapsuleColliderData.ColliderCenterInLocalSpace.y * stateMachine.Player.transform.localScale.y - hit.distance;

                if(distanceToFloatingPoint == 0f) {
                    return;
                }

                float amountToLift = distanceToFloatingPoint *slopData.StepReachForce -GetPlayerVerticalVelocity().y;

                Vector3 liftForce=new Vector3(0f,amountToLift,0);

                stateMachine.Player.Rigidbody.AddForce(liftForce, ForceMode.VelocityChange);
            }
        }

        private float SetSlopSpeedModifierOnAngle(float angle)
        {
            float slopeSpeedModifier = movementData.SlopeSpeedAngles.Evaluate(angle);
            
            stateMachine.ReusableData.MovementOnSlopesSpeedModifier = slopeSpeedModifier;

            return slopeSpeedModifier;
        }
        #endregion
        #region Reusable Methods
        protected override void AddInputActionsCallbacks()
        {
            base.AddInputActionsCallbacks();

            stateMachine.Player.Input.PlayerActions.Movement.canceled += OnMovementCanceled;

            stateMachine.Player.Input.PlayerActions.Dash.started += OnDashStarted;
        }

        protected override void RemoveInputActionsCallbacks()
        {
            base.RemoveInputActionsCallbacks();

            stateMachine.Player.Input.PlayerActions.Movement.canceled -= OnMovementCanceled;

            stateMachine.Player.Input.PlayerActions.Dash.started -= OnDashStarted;
        }
        protected virtual void OnMove()
        {
            if (stateMachine.ReusableData.ShouldWalk)
            {
                stateMachine.ChangeState(stateMachine.WalkingState);

                return;
            }

            stateMachine.ChangeState(stateMachine.RunningState);
        }
        #endregion

        #region Input Methods
        protected override void OnWalkToggleStarted(InputAction.CallbackContext context)
        {
            base.OnWalkToggleStarted(context);

            stateMachine.ChangeState(stateMachine.RunningState);
        }
        protected virtual void OnMovementCanceled(InputAction.CallbackContext context)
        {
            stateMachine.ChangeState(stateMachine.IdlingState);
        }
        protected virtual void OnDashStarted(InputAction.CallbackContext context)
        {
            stateMachine.ChangeState(stateMachine.DashingState);
        }
        #endregion
    }
}
