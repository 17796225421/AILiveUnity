 using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
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
        public override void Enter()
        {
            base.Enter();

            UpdateShouldSprintState();

            UpdateCameraRecenteringState(stateMachine.ReusableData.MovementInput);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            Float();
        }
        #endregion
        #region Main Methods
        private void UpdateShouldSprintState()
        {
            if (!stateMachine.ReusableData.ShouldSprint)
            {
                return;
            }

            if (stateMachine.ReusableData.MovementInput != Vector2.zero)
            {
                return;
            }

            stateMachine.ReusableData.ShouldSprint = false;
        }
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

            if(stateMachine.ReusableData.MovementOnSlopesSpeedModifier!=slopeSpeedModifier)
            {
                stateMachine.ReusableData.MovementOnSlopesSpeedModifier = slopeSpeedModifier;

                UpdateCameraRecenteringState(stateMachine.ReusableData.MovementInput);
            }

            return slopeSpeedModifier;
        }
        private bool IsThereGroundUnderNeath()
        {
            BoxCollider groundCheckCollider = stateMachine.Player.ColliderUtility.TriggerColliderData.GroundCheckCollider;

            Vector3 groundColliderCenterInWorldSpace = groundCheckCollider.bounds.center;

            Collider[] overlappedGroundColliders = Physics.OverlapBox(groundColliderCenterInWorldSpace, stateMachine.Player.ColliderUtility.TriggerColliderData.GroundCheckColliderExtents,groundCheckCollider.transform.rotation,stateMachine.Player.LayerData.GroundLayer,QueryTriggerInteraction.Ignore);

            return overlappedGroundColliders.Length > 0;
        }

        #endregion
        #region Reusable Methods
        protected override void AddInputActionsCallbacks()
        {
            base.AddInputActionsCallbacks();

            stateMachine.Player.Input.PlayerActions.Dash.started += OnDashStarted;

            stateMachine.Player.Input.PlayerActions.Jump.started += OnJumpStarted;
        }

        protected override void RemoveInputActionsCallbacks()
        {
            base.RemoveInputActionsCallbacks();

            stateMachine.Player.Input.PlayerActions.Dash.started -= OnDashStarted;

            stateMachine.Player.Input.PlayerActions.Jump.started -= OnJumpStarted;
        }
        protected virtual void OnMove()
        {
            if (stateMachine.ReusableData.ShouldSprint)
            {
                stateMachine.ChangeState(stateMachine.SprintingState);

                return;
            }
            if (stateMachine.ReusableData.ShouldWalk)
            {
                stateMachine.ChangeState(stateMachine.WalkingState);

                return;
            }

            stateMachine.ChangeState(stateMachine.RunningState);
        }

        protected override void OnContactWithGroundExited(Collider collider)
        {
            base.OnContactWithGroundExited(collider);

            if (IsThereGroundUnderNeath())
            {
                return;
            }

            Vector3 capsuleColliderCenterInWorldSpace = stateMachine.Player.ColliderUtility.CapsuleColliderData.Collider.bounds.center;

            Ray downwardsRayFromCapsuleBottom = new Ray(capsuleColliderCenterInWorldSpace-stateMachine.Player.ColliderUtility.CapsuleColliderData.ColliderVerticalExtents, Vector3.down);

            if(!Physics.Raycast(downwardsRayFromCapsuleBottom,out _,movementData.GroundToFallRayDistance,stateMachine.Player.LayerData.GroundLayer,QueryTriggerInteraction.Ignore))
            {
                OnFall();
            }
        }
        protected virtual void OnFall()
        {
            stateMachine.ChangeState(stateMachine.FallingState);
        }
        #endregion

        #region Input Methods
        protected override void OnWalkToggleStarted(InputAction.CallbackContext context)
        {
            base.OnWalkToggleStarted(context);

            stateMachine.ChangeState(stateMachine.RunningState);
        }
        protected virtual void OnDashStarted(InputAction.CallbackContext context)
        {
            stateMachine.ChangeState(stateMachine.DashingState);
        }
        protected virtual void OnJumpStarted(InputAction.CallbackContext context)
        {
            stateMachine.ChangeState(stateMachine.JumpingState);
        }
        #endregion
    }
}
