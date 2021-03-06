using Coffee.Characters;
using UnityEngine;

namespace Coffee
{
    public abstract class Item : MonoBehaviour, IItem
    {
        #pragma warning disable 649
        [SerializeField] private Ingredients ingredient;
        [SerializeField] private Transform grabPoint;
        [SerializeField] private Transform model;
        #pragma warning restore 649

        public Transform Transform => transform;
        public Ingredients Ingredient => ingredient;

        public abstract void ResetState();
        public void SetHeldState()
        {
            model.localPosition = grabPoint.localPosition;
        }

        public void SetPlacedState()
        {
            model.localPosition = Vector3.zero;
        }

        public abstract InteractionTypes CanUse(IInteractionComponent interactor);
        public abstract InteractionTypes CanCombine(Ingredients ingredient);
        public abstract void Use(IInteractionComponent interactor);
        public abstract void Combine(Ingredients ingredient);

        public virtual void Destroy()
        {
            GameManager.House.RemoveTemporaryItem(this);
            Destroy(gameObject);
        }

    }
}