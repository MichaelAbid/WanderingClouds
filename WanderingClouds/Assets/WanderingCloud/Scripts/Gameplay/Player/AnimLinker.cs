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
            animator.SetFloat("speed", new Vector2(velocity.x, velocity.z).magnitude);
            animator.SetFloat("fall", velocity.y);
        }

        public void Anim_Jump()=>animator.SetTrigger("jump");
        public void Anim_Dash()=>animator.SetTrigger("dash");
        public void Anim_Land()=>animator.SetTrigger("land");
    }
}