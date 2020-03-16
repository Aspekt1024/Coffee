using Coffee.Characters;
using UnityEngine;

namespace Coffee
{
    public class Fridge : Resettable, IInteractible
    {
        #pragma warning disable 649
        [SerializeField] private GameObject milkPrefab;
        #pragma warning restore 649

        private enum States
        {
            Closed, Open
        }

        private Animator anim;
        private States state;
        
        private static readonly int IsOpenAnimProp = Animator.StringToHash("isOpen");

        private void Awake()
        {
            anim = GetComponent<Animator>();
        }

        public InteractionTypes Use(IInteractionComponent interactor)
        {
            if (state == States.Closed) return InteractionTypes.Open;
            
            switch (interactor.CurrentItem)
            {
                case null:
                    return InteractionTypes.Grab;
                case Milk _:
                    return InteractionTypes.Place;
                default:
                    return InteractionTypes.None;
            }
        }

        public void ApplyUse(IInteractionComponent interactor)
        {
            if (state == States.Closed)
            {
                Open();
                return;
            }
            
            switch (interactor.CurrentItem)
            {
                case null:
                    var newMilk = Instantiate(milkPrefab).GetComponent<Milk>();
                    if (!interactor.ReceiveItem(newMilk))
                    {
                        Destroy(newMilk.gameObject);
                        return;
                    }
                    GameManager.House.AddTemporaryItem(newMilk);
                    break;
                case Milk _:
                    var milk = interactor.RemoveItem();
                    milk.Destroy();
                    break;
            }
            Close();
        }

        public override void ResetState()
        {
            anim.Play("Close", 0, 1f);
            state = States.Closed;
        }

        private void Open()
        {
            state = States.Open;
            anim.SetBool(IsOpenAnimProp, true);
        }

        private void Close()
        {
            state = States.Closed;
            anim.SetBool(IsOpenAnimProp, false);
        }
    }
}