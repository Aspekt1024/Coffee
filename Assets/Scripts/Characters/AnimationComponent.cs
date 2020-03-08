using System.Collections.Generic;
using UnityEngine;

namespace Coffee.Characters
{
    public class AnimationComponent
    {
        public static readonly int AnimSpeedParam = Animator.StringToHash("speed");
        
        private const string IdleAnim = "Idle";
        private const string RunAnim = "Run";
        private const string GrabAnim = "Grab";
        private const string OpenAnim = "Open";
        private const string DrinkAnim = "Drink";
        private const string PourAnim = "Pour";
        
        public Animator Controller { get; }

        public AnimationComponent(Animator anim)
        {
            Controller = anim;
        }

        public void ApplyInteractionAnimation(InteractionTypes type)
        {
            switch (type)
            {
                case InteractionTypes.None:
                    break;
                case InteractionTypes.Grab:
                    Controller.Play(GrabAnim, 0, 0f);
                    break;
                case InteractionTypes.Open:
                    Controller.Play(GrabAnim, 0, 0f);
                    break;
                case InteractionTypes.Drink:
                    Controller.Play(GrabAnim, 0, 0f);
                    break;
                case InteractionTypes.Pour:
                    Controller.Play(GrabAnim, 0, 0f);
                    break;
                case InteractionTypes.Instant:
                    Controller.Play(GrabAnim, 0, 0f);
                    break;
                case InteractionTypes.Place:
                    Controller.Play(GrabAnim, 0, 0f);
                    break;
                default:
                    Debug.LogError("Invalid Interaction type: " + type);
                    break;
            }
        }
    }
}