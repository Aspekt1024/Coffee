using System.Collections.Generic;
using UnityEngine;

namespace Coffee
{
    public class HouseManager : MonoBehaviour, IManager
    {
        private readonly List<IResettable> resettableItems = new List<IResettable>();
        private readonly List<IItem> temporaryItems = new List<IItem>();
        
        public void Init()
        {
        }
        
        public void ResetItems()
        {
            foreach (var temporaryItem in temporaryItems)
            {
                if (temporaryItem == null || ((Item)temporaryItem).gameObject == null) continue;
                Destroy(((Item)temporaryItem).gameObject);
            }
            temporaryItems.Clear();
            
            foreach (var item in resettableItems)
            {
                item.ResetState();
            }
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
        public void AddTemporaryItem(IItem item)
        {
            if (temporaryItems.Contains(item)) return;
            temporaryItems.Add(item);
        }

        public void RemoveTemporaryItem(IItem item)
        {
            temporaryItems.Remove(item);
        }
    }
}