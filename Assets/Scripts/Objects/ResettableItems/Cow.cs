using Coffee.Characters;
using UnityEngine;

namespace Coffee
{
    public class Cow : Resettable, IInteractible
    {
#pragma warning disable 649
        [SerializeField] private GameObject model;
#pragma warning restore 649

        private Collider interactionCollider;

        private bool hasSpoken; 

        private void Awake()
        {
            interactionCollider = GetComponent<Collider>();
        }

        public override void ResetState()
        {
            bool isActive = GameManager.Data.Day >= 1;
            model.SetActive(isActive);
            interactionCollider.enabled = isActive;
        }

        public InteractionTypes Use(IInteractionComponent interactor)
        {
            if (!hasSpoken)
            {
                hasSpoken = true;
                GameManager.UI.ShowDialogue("Cow", "Yes?");
            }

            return InteractionTypes.None;
        }

        public void ApplyUse(IInteractionComponent interactor)
        {
        }
    }
}