using Coffee.Characters;

namespace Coffee
{
    public class Milk : Item
    {
        public override void ResetState()
        {
        }

        public override InteractionTypes CanUse(IInteractionComponent interactor) => InteractionTypes.None;
        public override InteractionTypes CanCombine(Ingredients ingredient) => InteractionTypes.None;

        public override void Use(IInteractionComponent interactor)
        {
        }

        public override void Combine(Ingredients ingredient)
        {
        }
    }
}