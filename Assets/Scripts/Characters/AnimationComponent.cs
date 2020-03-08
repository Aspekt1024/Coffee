using UnityEngine;

namespace Coffee.Characters
{
    public class AnimationComponent
    {
        private enum Animations
        {
            Idle,
            Run,
            Grab,
            Open,
        }

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
                    Controller.Play(Animations.Grab.ToString(), 0, 0f);
                    break;
                case InteractionTypes.Open:
                    //Controller.Play(Animations.Open.ToString(), 0, 0f);
                    break;
                default:
                    Debug.LogError("Invalid Interaction type: " + type);
                    break;
            }
        }
    }
}