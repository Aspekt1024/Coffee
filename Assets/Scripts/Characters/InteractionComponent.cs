using System;
using UnityEngine;

namespace Coffee.Characters
{
    [Serializable]
    public class InteractionComponent : IInteractionComponent
    {
        #pragma warning disable 649
        [SerializeField] private Transform itemGrabPoint;
        #pragma warning restore 649
        
        private IActor actor;

        private IInteractible currentInteractible;

        private enum States
        {
            None,
            UsingInteractible,
            UsingItem,
        }

        private States state;

        public bool IsBusy => state != States.None;
        public IItem CurrentItem { get; private set; }
        
        public void Interact()
        {
            if (IsBusy) return;
            
            // Use interactibles first, whether we're holding an item or not.
            // If that fails, try and use the held item if we're holding one.
            currentInteractible = GetInteractible();
            if (currentInteractible != null)
            {
                var type = currentInteractible.Use(this);

                if (type != InteractionTypes.None)
                {
                    actor.Animator.ApplyInteractionAnimation(type);
                    state = States.UsingInteractible;
                    return;
                }
            }

            // No interactibles were used
            if (CurrentItem != null)
            {
                var type = CurrentItem.CanUse(this);
                if (type != InteractionTypes.None)
                {
                    actor.Animator.ApplyInteractionAnimation(type);
                    state = States.UsingItem;
                    return;
                }
            }
            
            state = States.None;
        }

        public void Apply()
        {
            switch (state)
            {
                case States.None:
                    break;
                case States.UsingInteractible:
                    currentInteractible?.ApplyUse(this);
                    break;
                case States.UsingItem:
                    CurrentItem?.Use(this);
                    break;
                default:
                    Debug.LogError("invalid state: " + state);
                    break;
            }

            state = States.None;
        }
        
        public bool ReceiveItem(IItem item)
        {
            if (CurrentItem != null) return false;
            CurrentItem = item;
            
            var currentTf = item.Transform;
            currentTf.SetParent(itemGrabPoint);
            currentTf.SetPositionAndRotation(itemGrabPoint.position, itemGrabPoint.rotation);
            return true;
        }

        public IItem RemoveItem()
        {
            var item = CurrentItem;
            CurrentItem = null;
            return item;
        }

        public void Init(IActor actor)
        {
            this.actor = actor;
        }

        private IInteractible GetInteractible()
        {
            var origin = actor.Transform.position;
            origin.y = 1f;

            var layer = LayerUtil.GetMask(Layers.Interactible);
            bool hit = Physics.Raycast(origin, actor.Transform.forward, out var hitInfo, 2f, layer);

            if (!hit) return null;
            
            var interactible = hitInfo.collider.GetComponent<IInteractible>();
            if (interactible == null)
            {
                interactible = hitInfo.collider.GetComponentInParent<IInteractible>();
            }
            return interactible;
        }
    }
}