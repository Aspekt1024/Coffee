using System;
using Coffee.Characters;
using UnityEngine;

namespace Coffee
{
    public class ItemPoint : Resettable, IInteractible
    {
        #pragma warning disable 649
        [SerializeField] private Item itemPrefab;
        [SerializeField] private Transform spawnPoint;
        #pragma warning restore 649

        private IItem item;
        
        private enum States
        {
            Empty,
            HasItem,
        }

        private States state;
        
        private void Start()
        {
            if (GameManager.Instance == null) return;
            GameManager.House.AddResettable(this);
        }

        public override void ResetState()
        {
            state = States.Empty;
            item = null;
            if (itemPrefab == null) return;

            item = Instantiate(itemPrefab);
            if (item == null) return;

            state = States.HasItem;
            item.ResetState();
            GameManager.House.AddTemporaryItem(item);
            Set(item);
        }
        
        public InteractionTypes Use(IInteractionComponent interactor)
        {
            switch (state)
            {
                case States.Empty:
                    if (interactor.CurrentItem != null)
                    {
                        return InteractionTypes.Place;
                    }
                    return InteractionTypes.None;
                case States.HasItem:
                    if (interactor.CurrentItem != null)
                    {
                        return item.CanCombine(interactor.CurrentItem.Ingredient);
                    }
                    return InteractionTypes.Grab;
                default:
                    Debug.LogError("invalid state: " + state);
                    break;
            }

            return InteractionTypes.None;
        }

        public void ApplyUse(IInteractionComponent interactor)
        {
            switch (state)
            {
                case States.Empty:
                    if (interactor.CurrentItem == null) return;
                    Set(interactor.RemoveItem());
                    break;
                case States.HasItem:
                    if (interactor.CurrentItem != null)
                    {
                        item.Combine(interactor.CurrentItem.Ingredient);
                        return;
                    }
                    if (interactor.ReceiveItem(item))
                    {
                        item = null;
                        state = States.Empty;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Set(IItem newItem)
        {
            item = newItem;
            if (item == null)
            {
                state = States.Empty;
                return;
            }
            
            var itemTf = item.Transform;
            var spawnTf = spawnPoint.transform;
            
            itemTf.SetParent(spawnTf);
            itemTf.SetPositionAndRotation(spawnTf.position, spawnTf.rotation);

            state = States.HasItem;
        }

    }
}