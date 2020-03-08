using System;
using UnityEngine;

namespace Coffee.Characters
{
    [Serializable]
    public class BasicMovement : IMovementController
    {
        #pragma warning disable 649
        [SerializeField] private float maxSpeed;
        [SerializeField] private Rigidbody body;
        #pragma warning restore 649

        private IActor actor;

        public void Move(float xAxis, float zAxis)
        {
            var heading = new Vector3(xAxis, 0f, zAxis).normalized;

            body.velocity = heading * maxSpeed;
            actor.Animator.Controller.SetFloat(AnimationComponent.AnimSpeedParam, body.velocity.magnitude);

            if (body.velocity.magnitude < 0.2f) return;
            
            var rot = body.transform.eulerAngles;
            rot.y = Mathf.Atan2(heading.x, heading.z) * Mathf.Rad2Deg;
            body.transform.eulerAngles = rot;
            
        }

        public void Init(IActor actor)
        {
            this.actor = actor;
        }
    }
}