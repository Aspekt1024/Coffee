using Coffee.Characters;
using UnityEngine;

namespace Coffee
{
    /// <summary>
    /// Items can be held by the player
    /// </summary>
    public interface IItem
    {
        Transform Transform { get; }
        
        /// <summary>
        /// The ingredient produced by this item
        /// </summary>
        Ingredients Ingredient { get; }
        
        /// <summary>
        /// Resets the item to its default state
        /// </summary>
        void ResetState();

        /// <summary>
        /// Positions the item's model in the held state
        /// </summary>
        void SetHeldState();

        /// <summary>
        /// Positions the item's model in the placed state
        /// </summary>
        void SetPlacedState();
        
        /// <summary>
        /// Destroys the item, removing it from the game
        /// </summary>
        void Destroy();

        /// <summary>
        /// Checks if the item can be used by the interactor, returning the type of interaction
        /// </summary>
        InteractionTypes CanUse(IInteractionComponent interactor);

        /// <summary>
        /// Checks if the item can be Combined with the given ingredient, returning the type of interaction
        /// </summary>
        InteractionTypes CanCombine(Ingredients ingredient);
        
        /// <summary>
        /// Uses the item.
        /// </summary>
        void Use(IInteractionComponent interactor);
        
        /// <summary>
        /// Combines the item with the given ingredient
        /// </summary>
        void Combine(Ingredients ingredient);
    }
}