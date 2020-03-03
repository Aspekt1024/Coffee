using Rewired;
using UnityEngine;

namespace Coffee
{
    public class Actor : MonoBehaviour, IActor
    {
        #pragma warning disable 649
        [SerializeField] private int playerId = 0;
        [SerializeField] private float speed = 5f;
        [SerializeField] private Transform itemGrabPoint;
        #pragma warning restore 649
    
        private Rigidbody body;
        private Player player;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            player = ReInput.players.GetPlayer(playerId);
        }

        private void Update()
        {
//            var horizMovement = player.GetAxis(InputLabels.MoveHorizontal);
//            var vertMovement = player.GetAxis(InputLabels.MoveVertical);
//        
//            body.velocity = new Vector3(horizMovement, 0f, vertMovement) * speed;
        }

        public Item CurrentItem { get; private set; }

        public bool GiveItem(Item item)
        {
            if (CurrentItem != null) return false;
            
            CurrentItem = item;
            var currentTf = CurrentItem.transform;
            currentTf.SetParent(itemGrabPoint);
            currentTf.position = itemGrabPoint.transform.position;
            currentTf.rotation = itemGrabPoint.transform.rotation;
            
            return true;
        }

        public Item RemoveItem()
        {
            var item = CurrentItem;
            CurrentItem = null;
            return item;
        }

        public void MoveTo(Vector3 position, Vector3 lookPosition)
        {
            body.MovePosition(position);
            body.transform.LookAt(lookPosition);
        }
    }
}