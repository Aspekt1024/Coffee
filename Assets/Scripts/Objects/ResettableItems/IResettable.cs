namespace Coffee
{
    /// <summary>
    /// ResettableItems are intended to be automatically reset by the game management scripts
    /// </summary>
    public interface IResettable
    {
        /// <summary>
        /// Resets the state of the item
        /// </summary>
        void ResetState();
    }
}