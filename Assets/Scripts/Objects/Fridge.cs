using UnityEngine;

namespace Coffee
{
    public class Fridge : MonoBehaviour, IInteractible
    {
        #pragma warning disable 649
        [SerializeField] private GameObject milkPrefab;
        #pragma warning restore 649
        
        public bool Use(IActor actor)
        {
            if (actor.CurrentItem == null)
            {
                var milk = Instantiate(milkPrefab, transform).GetComponent<Milk>();
                actor.GiveItem(milk);
                return true;
            }

            return false;
        }
    }
}