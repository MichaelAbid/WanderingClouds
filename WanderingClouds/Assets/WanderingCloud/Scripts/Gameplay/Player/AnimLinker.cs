using System.Collections.Generic;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using WanderingCloud.Controller;

namespace WanderingCloud
{
    [RequireComponent(typeof(Animator))]
    public class AnimLinker : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Rigidbody body;
        [SerializeField] private PlayerMovement movement;
        [SerializeField] private PlayerBrain player;

        private void Update()
        {
            var worldVelocity = body.velocity;
            var velocityXZ = Vector3.ProjectOnPlane(body.velocity, Vector3.up).normalized;

            Vector3 forward = Vector3.ProjectOnPlane(player.Camera.transform.forward, Vector3.up).normalized;
            Vector3 right = Vector3.ProjectOnPlane(player.Camera.transform.right, Vector3.up).normalized;
            var localVelocity = new Vector2(Vector3.Dot(velocityXZ, forward), Vector3.Dot(velocityXZ, right));

            animator.SetFloat("rightVelocity", localVelocity.y);
            animator.SetFloat("forwardVelocity", localVelocity.x);
            animator.SetFloat("speed", new Vector2(worldVelocity.x, worldVelocity.z).magnitude);
            animator.SetFloat("fallSpeed", worldVelocity.y);
        }

        public void SetUpperBodyLayerWeight(float weight)
        {
            animator.SetLayerWeight(animator.GetLayerIndex("UpperBodyOverride"), weight);
        }


        public void Anim_Aim()=>animator.SetBool("aim", !animator.GetBool("aim"));
        public void Anim_Fall(bool state)=>animator.SetBool("fall", state);
        public void Anim_Jump()=>animator.SetTrigger("jump");
        public void Anim_Dash()=>animator.SetTrigger("dash");
        public void Anim_Land()=>animator.SetTrigger("land");


    }
}