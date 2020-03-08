using Coffee.Characters;
using UnityEngine;

namespace Coffee
{
    public class Fridge : MonoBehaviour, IInteractible
    {
        #pragma warning disable 649
        [SerializeField] private GameObject milkPrefab;
        #pragma warning restore 649
        
        public InteractionTypes Use(IInteractionComponent interactor)
        {
            if (interactor.CurrentItem == null)
            {
                var milk = Instantiate(milkPrefab, transform).GetComponent<Milk>();
                interactor.ReceiveItem(milk);
                return InteractionTypes.Grab;
            }

            return InteractionTypes.None;
        }
    }
}