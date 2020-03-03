using System.Collections.Generic;
using UnityEngine;

namespace Coffee
{
    public class HouseManager : MonoBehaviour, IManager
    {
        private readonly List<ResettableItem> resettableItems = new List<ResettableItem>();
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
        }

        public void AddResettableItem(ResettableItem item)
        {
            resettableItems.Add(item);
        }

        public void RemoveResettableItem(ResettableItem item)
        {
            resettableItems.Remove(item);
        }

        /// <summary>
        /// Adds a temporary item which is to be cleaned up before reloading the scene
        /// </summary>
        public void AddTemporaryItem()
        {
            
        }

        public void RemoveTemporaryItem()
        {
            
        }
    }
}