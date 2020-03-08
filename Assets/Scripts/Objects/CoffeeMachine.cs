using Coffee.Characters;
using UnityEngine;

namespace Coffee
{
    public class CoffeeMachine: MonoBehaviour, IInteractible
    {
        public InteractionTypes Use(IInteractionComponent interactor)
        {
            if (interactor.CurrentItem is Cup cup)
            {
                if (cup.HasCoffee) return InteractionTypes.None;
                cup.AddCoffee();
                return InteractionTypes.Grab;
            }

            return InteractionTypes.None;
        }
    }
}