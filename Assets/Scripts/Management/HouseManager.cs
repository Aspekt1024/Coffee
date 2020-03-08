using System.Collections.Generic;
using UnityEngine;

namespace Coffee
{
    public class HouseManager : MonoBehaviour, IManager
    {
        private readonly List<IResettable> resettableItems = new List<IResettable>();
        private readonly List<Item> temporaryItems = new List<Item>();
        
        public void Init()
        {
        }
        
        public void ResetItems()
        {
            foreach (var item in resettableItems)
            {
                item.ResetState();
            }

            foreach (var temporaryItem in temporaryItems)
            {
                if (temporaryItem == null || temporaryItem.gameObject == null) continue;
                Destroy(temporaryItem.gameObject);
            }
            temporaryItems.Clear();
        }

        public void AddResettable(IResettable item)
        {
            resettableItems.Add(item);
        }

        public void RemoveResettable(IResettable item)
        {
            resettableItems.Remove(item);
        }

        /// <summary>
        /// Adds a temporary item which is to be cleaned up before reloading the scene
        /// </summary>
        public void AddTemporaryItem(Item item)
        {
            if (temporaryItems.Contains(item)) return;
            temporaryItems.Add(item);
        }

        public void RemoveTemporaryItem(Item item)
        {
            temporaryItems.Remove(item);
        }
    }
}