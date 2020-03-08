namespace Coffee
{
    public abstract class ResettableItem : Item, IResettableItem
    {
        private void Start()
        {
            if (GameManager.Instance == null) return;
            GameManager.House.AddResettableItem(this);
        }

        public abstract void ResetState();

        public override void Destroy()
        {
            if (GameManager.Instance == null) return;
            GameManager.House.RemoveResettableItem(this);
            base.Destroy();
        }
    }
}