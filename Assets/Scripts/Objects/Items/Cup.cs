using System;
using Coffee.Characters;
using UnityEngine;

namespace Coffee
{
    public class Cup : Item
    {
        #pragma warning disable 649
        [SerializeField] private GameObject liquid;
        #pragma warning restore 649
        
        private static readonly int IsFilledAnimParam = Animator.StringToHash("isFilled");

        private Animator anim;

        public bool HasCoffee { get; private set; }
        public bool HasMilk { get; private set; }
        
        private bool IsReady => HasCoffee && HasMilk;

        private void Start()
        {
            anim = GetComponent<Animator>();
        }

        public override InteractionTypes CanUse(IInteractionComponent interactor)
        {
            return CanDrink(interactor) ? InteractionTypes.Drink : InteractionTypes.None;
        }
        
        public override InteractionTypes CanCombine(Ingredients ingredient)
        {
            switch (ingredient)
            {
                case Ingredients.Milk:
                    return InteractionTypes.Pour;
                case Ingredients.Coffee:
                    return InteractionTypes.Instant;
                default:
                    return InteractionTypes.None;
            }
        }

        public override void Use(IInteractionComponent interactor)
        {
            if (!CanDrink(interactor)) return;
            GameManager.Data.HadCoffee = true;
            Empty();
        }
        
        public override void Combine(Ingredients ingredient)
        {
            switch (ingredient)
            {
                case Ingredients.Coffee:
                    HasCoffee = true;
                    break;
                case Ingredients.Milk:
                    HasMilk = true;
                    // TODO show milk
                    break;
            }
        }

        public override void ResetState()
        {
            Empty();
        }

        public void AddCoffee(bool immediate)
        {
            HasCoffee = true;
            Fill();
            
            if (immediate)
            {
                anim.Play("Fill", 0, 1f);
            }
        }

        private void Fill()
        {
            liquid.SetActive(true);
            GetComponent<Animator>().SetBool(IsFilledAnimParam, true);
        }

        private void Empty()
        {
            HasCoffee = false;
            HasMilk = false;
            liquid.SetActive(false);
            GetComponent<Animator>().SetBool(IsFilledAnimParam, false);
        }

        private bool CanDrink(IInteractionComponent interactor)
        {
            return IsReady && interactor.CurrentItem is Cup cup && cup == this;
        }

    }
}