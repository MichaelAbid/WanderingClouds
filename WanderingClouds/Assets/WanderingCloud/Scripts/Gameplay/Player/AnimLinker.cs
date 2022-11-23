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
        
        private static readonly int Dash = Animator.StringToHash("dash");
        private static readonly int Jump = Animator.StringToHash("jump");
        private static readonly int Speed = Animator.StringToHash("speed");
        private static readonly int Fall = Animator.StringToHash("fall");

        private void Update()
        {
            var velocity = body.velocity;
            animator.SetFloat(Speed, new Vector2(velocity.x, velocity.z).magnitude);
            animator.SetFloat(Fall, velocity.y);
        }

        public void Anim_Jump()=>animator.SetTrigger(Jump);
        public void Anim_Dash()=>animator.SetTrigger(Jump);
    }
}