using System;
using UnityEngine;

namespace Coffee
{
    /// <summary>
    /// Resettable Items subscribe to be reset by the House Manager
    /// </summary>
    public abstract class Resettable : MonoBehaviour, IResettable
    {
        private void Start()
        {
            GameManager.House.AddResettable(this);
        }

        public abstract void ResetState();
        
        public void Destroy()
        {
            if (GameManager.Instance == null) return;
            GameManager.House.RemoveResettable(this);
            Destroy(gameObject);
        }

    }
}