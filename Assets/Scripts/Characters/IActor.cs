using UnityEngine;

namespace Coffee
{
    public interface IActor
    {
        /// <summary>
        /// The current item held by the actor
        /// </summary>
        Item CurrentItem { get; }
        
        /// <summary>
        /// Gives the actor an item, returns true if successful
        /// </summary>
        bool GiveItem(Item item);
        
        /// <summary>
        /// Takes the item currently held by the actor, returns null if not holding anything
        /// </summary>
        Item RemoveItem();

        
        /// <summary>
        /// Moves the actor to the given position. The actor will attempt to be 
        /// rotated to look at the lookPosition at the end of movement.
        /// </summary>
        /// <param name="position">The position to move to</param>
        /// <param name="lookPosition">The desired position to look at when movement has ended</param>
        void MoveTo(Vector3 position, Vector3 lookPosition);
    }
}