namespace Coffee.Characters
{
    public interface IMovementController : IActorComponent
    {
        void Move(float xAxis, float zAxis);
    }
}