using System;

namespace Coffee.Characters
{
    public interface IInteractionComponent : IActorComponent
    {
        /// <summary>
        /// Begins an interaction attempt
        /// </summary>
        bool Interact();
        
        /// <summary>
        /// Applies the interaction result
        /// </summary>
        void Apply();
        
        /// <summary>
        /// The current item held by the actor
        /// </summary>
        Item CurrentItem { get; }
        
        /// <summary>
        /// Gives the actor an item, returns true if successful
        /// </summary>
        bool ReceiveItem(Item item);
        
        /// <summary>
        /// Takes the item currently held by the actor, returns null if not holding anything
        /// </summary>
        Item RemoveItem();
    }
}