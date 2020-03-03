using UnityEngine;

namespace Coffee
{
    public class CoffeeMachine: MonoBehaviour, IInteractible
    {
        public bool Use(IActor actor)
        {
            if (actor.CurrentItem is Cup cup)
            {
                if (cup.HasCoffee) return false;
                cup.AddCoffee();
                return true;
            }

            return false;
        }
    }
}