namespace Coffee
{
    /// <summary>
    /// Items can be held by the player
    /// </summary>
    public interface IItem
    {
        /// <summary>
        /// Destroys the item, removing it from the game
        /// </summary>
        void Destroy();
    }
}