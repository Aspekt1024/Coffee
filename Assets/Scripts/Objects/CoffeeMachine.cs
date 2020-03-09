using System;
using System.Collections;
using Coffee.Characters;
using UnityEngine;

namespace Coffee
{
    public class CoffeeMachine: Resettable, IInteractible
    {
        #pragma warning disable 649
        [SerializeField] private Transform cupPlacementPoint;
        [SerializeField] private Transform zoomTransform;
        [SerializeField] private Cup cupVisual;
        [SerializeField] private Animator animator;
        #pragma warning restore 649

        private const float FillDuration = 0f;
        
        private Cup heldCup;

        private enum States
        {
            None,
            Operating,
            Making,
            Ready,
        }

        private States state;
        private static readonly int IsDispensingAnimProp = Animator.StringToHash("isDispensing");

        public InteractionTypes Use(IInteractionComponent interactor)
        {
            switch (state)
            {
                case States.None:
                    if (heldCup == null && interactor.CurrentItem is Cup c && !c.HasCoffee)
                    {
                        GameManager.Camera.Zoom(zoomTransform);
                        return InteractionTypes.Place;
                    }
                    break;
                case States.Ready:
                    if (interactor.CurrentItem == null)
                    {
                        return InteractionTypes.Grab;
                    }
                    break;
                default:
                    return InteractionTypes.None;
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
                case States.Ready:
                    if (interactor.ReceiveItem(heldCup))
                    {
                        heldCup.gameObject.SetActive(true);
                        cupVisual.gameObject.SetActive(false);
                        heldCup = null;
                        state = States.None;
                    }
                    break;
            }
        }

        private void OnMouseUp()
        {
            if (state == States.Operating)
            {
                state = States.Making;
                StartCoroutine(Dispense());
            }
        }

        private IEnumerator Dispense()
        {
            animator.SetBool(IsDispensingAnimProp, true);
            cupVisual.AddCoffee();
            heldCup.AddCoffee();
            
            yield return new WaitForSeconds(1.2f);
            
            state = States.Ready;
            animator.SetBool(IsDispensingAnimProp, false);
            
            yield return new WaitForSeconds(0.5f);
            GameManager.Camera.UnZoom();
        }

        private void TakeCup(Cup c)
        {
            heldCup = c;
            heldCup.transform.SetParent(cupPlacementPoint);
            heldCup.gameObject.SetActive(false);
            cupVisual.gameObject.SetActive(true);
        }

        public override void ResetState()
        {
            if (heldCup != null)
            {
                heldCup.Destroy();
                heldCup = null;
            }
            cupVisual.ResetState();
            cupVisual.gameObject.SetActive(false);
        }
    }
}