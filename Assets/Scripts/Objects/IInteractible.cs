using Coffee.Characters;

namespace Coffee
{
    public enum InteractionTypes
    {
        None,
        Instant,
        Grab,
        Place,
        Open,
        Drink,
        Pour,
    }
    
    public interface IInteractible
    {
        /// <summary>
        /// Attempts to the interactible and returns the interaction type. See ApplyUse() for use with animations.
        /// </summary>
        InteractionTypes Use(IInteractionComponent interactor);
        
        /// <summary>
        /// Commits the use of the interactible, applying effects. Typically used when triggered by an animation.
        /// </summary>
        void ApplyUse(IInteractionComponent interactor);
    }
}