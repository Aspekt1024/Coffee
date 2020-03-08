using Coffee.Characters;
using UnityEngine;

namespace Coffee
{
    public class CoffeeMachine: MonoBehaviour, IInteractible
    {
        #pragma warning disable 649
        [SerializeField] private Transform cupPlacementPoint;
        [SerializeField] private Transform zoomTransform;
        #pragma warning restore 649

        private Cup cup;

        private enum States
        {
            None,
            Operating,
            Making,
            Ready,
        }

        private States state;
        
        public InteractionTypes Use(IInteractionComponent interactor)
        {
            switch (state)
            {
                case States.None:
                    if (cup == null && interactor.CurrentItem is Cup c && !c.HasCoffee)
                    {
                        GameManager.Camera.Zoom(zoomTransform);
                        return InteractionTypes.Place;
                    }
                    break;
                case States.Operating:
                    return InteractionTypes.Grab; // TODO Press
                case States.Making:
                    break;
                case States.Ready:
                    if (interactor.CurrentItem == null)
                    {
                        return InteractionTypes.Grab;
                    }
                    break;
                default:
                    Debug.LogError("invalid coffee machine state: " + state);
                    break;
            }

            return InteractionTypes.None;
        }

        public void ApplyUse(IInteractionComponent interactor)
        {
            switch (state)
            {
                case States.None:
                    if (interactor.CurrentItem is Cup c && !c.HasCoffee)
                    {
                        state = States.Operating;
                        TakeCup((Cup)interactor.RemoveItem());
                    }
                    break;
                case States.Operating:
                    state = States.Ready; // TODO make (use animation)
                    cup.AddCoffee();
                    break;
                case States.Making:
                    break;
                case States.Ready:
                    if (interactor.ReceiveItem(cup))
                    {
                        cup = null;
                        state = States.None;
                        GameManager.Camera.Return();
                    }
                    break;
                default:
                    Debug.LogError("invalid coffee machine state: " + state);
                    break;
            }
        }

        private void TakeCup(Cup c)
        {
            cup = c;
            cup.transform.SetParent(cupPlacementPoint);
            cup.Transform.SetPositionAndRotation(cupPlacementPoint.position, cupPlacementPoint.rotation);
        }
    }
}