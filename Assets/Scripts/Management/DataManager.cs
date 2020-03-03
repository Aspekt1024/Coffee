namespace Coffee
{
    public class DataManager : IManager
    {
        public int Day { get; set; } = 1;
        public bool HadCoffee { get; set; } = false;
        
        public void Init()
        {
        }
        
        
    }
}