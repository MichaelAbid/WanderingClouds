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

        private void Update()
        {
            var velocity = body.velocity;
            animator.SetFloat("xVelocity", velocity.normalized.x);
            animator.SetFloat("zVelocity", velocity.normalized.z);
            animator.SetFloat("speed", new Vector2(velocity.x, velocity.z).magnitude);
            animator.SetFloat("fallSpeed", velocity.y);
        }


        public void Anim_Aim()=>animator.SetBool("aim", !animator.GetBool("aim"));
        public void Anim_Fall(bool state)=>animator.SetBool("fall", state);
        public void Anim_Jump()=>animator.SetTrigger("jump");
        public void Anim_Dash()=>animator.SetTrigger("dash");
        public void Anim_Land()=>animator.SetTrigger("land");


    }
}