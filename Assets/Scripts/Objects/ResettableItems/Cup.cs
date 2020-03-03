using UnityEngine;

namespace Coffee
{
    public class Cup : ResettableItem
    {
        #pragma warning disable 649
        [SerializeField] private CupSpawnPoint spawnPoint;
        [SerializeField] private GameObject liquid;
        #pragma warning restore 649

        private bool hasMilk;

        public bool HasCoffee { get; private set; }

        public override void ResetState()
        {
            spawnPoint.Set(this);
            HasCoffee = false;
            hasMilk = false;
            Empty();
        }

        public bool Use(IActor actor)
        {
            if (actor.CurrentItem == null && IsReady)
            {
                GameManager.Data.HadCoffee = true;
                Empty();
                return true;
            }

            if (actor.CurrentItem is Milk)
            {
                if (IsReady || hasMilk)
                {
                    return false;
                }

                var milk = actor.RemoveItem();
                milk.Destroy();
                hasMilk = true;
                return true;
            }

            return false;
        }

        public void AddCoffee()
        {
            HasCoffee = true;
            Fill();
        }

        public void Empty()
        {
            liquid.SetActive(false);
        }

        public void Fill()
        {
            liquid.SetActive(true);
        }

        private bool IsReady => HasCoffee && hasMilk;
    }
}