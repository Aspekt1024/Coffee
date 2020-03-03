using UnityEngine;

namespace Coffee
{
    public abstract class Item : MonoBehaviour, IItem
    {
        public virtual void Destroy()
        {
            Destroy(gameObject);
        }
    }
}