using Coffee.Characters;

namespace Coffee
{
    public enum InteractionTypes
    {
        None,
        Grab,
        Open,
    }
    
    public interface IInteractible
    {
        InteractionTypes Use(IInteractionComponent interactor);
    }
}