using UnityEngine;

namespace Coffee
{
    public class ExitDoor : Door
    {
        public override bool Use(IActor actor)
        {
            var currentDay = GameManager.Data.Day;
            var canLeave = GameManager.Data.HadCoffee;
            var text = canLeave ? GetSuccessText(currentDay) : GetFailureText(currentDay);
            
            GameManager.UI.ShowDialogue("Door", text);

            if (canLeave)
            {
                return base.Use(actor);
            }

            return false;
        }

        private string GetSuccessText(int day)
        {
            switch (day)
            {
                case 1:
                    return "Have a nice day!";
                case 2:
                    return "That was... daring.";
                default:
                    Debug.LogWarning("Invalid day: " + day);
                    return "I know not of this day!";
            }
        }
        
        private string GetFailureText(int day)
        {
            switch (day)
            {
                case 1:
                    return "You don't want to face the day without your coffee...";
                case 2:
                    return "Coffee before you go out into the world. You know the rules.";
                default:
                    Debug.LogWarning("Invalid day: " + day);
                    return "I know not of this day!";
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                GameManager.Gameplay.DayComplete();
            }
        }
    }
}