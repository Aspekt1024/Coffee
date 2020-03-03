using System;
using UnityEngine;

namespace Coffee
{
    public abstract class ResettableItem : Item, IResettableItem
    {
        private void Start()
        {
            GameManager.House.AddResettableItem(this);
        }

        public abstract void ResetState();

        public override void Destroy()
        {
            GameManager.House.RemoveResettableItem(this);
            base.Destroy();
        }
    }
}