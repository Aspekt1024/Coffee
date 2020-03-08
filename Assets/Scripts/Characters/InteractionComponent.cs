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
        
        public Item CurrentItem { get; private set; }
        
        public bool Interact()
        {
            var origin = actor.Transform.position;
            origin.y = 1f;

            var layer = LayerUtil.GetMask(Layers.Interactible);
            bool hit = Physics.Raycast(origin, actor.Transform.forward, out var hitInfo, 5f, layer);

            if (!hit) return false;
            
            var interactible = hitInfo.collider.GetComponent<IInteractible>();
            if (interactible == null)
            {
                interactible = hitInfo.collider.GetComponentInParent<IInteractible>();
                if (interactible == null) return false;
            }
            var type = interactible.Use(this);
            actor.Animator.ApplyInteractionAnimation(type);
            return type != InteractionTypes.None;
        }

        public void Apply()
        {
            if (CurrentItem == null) return;
            
            var currentTf = CurrentItem.transform;
            currentTf.SetParent(itemGrabPoint);
            currentTf.position = itemGrabPoint.transform.position;
            currentTf.rotation = itemGrabPoint.transform.rotation;
        }
        
        public bool ReceiveItem(Item item)
        {
            if (CurrentItem != null) return false;
            CurrentItem = item;
            return true;
        }

        public Item RemoveItem()
        {
            var item = CurrentItem;
            CurrentItem = null;
            return item;
        }

        public void Init(IActor actor)
        {
            this.actor = actor;
        }
    }
}