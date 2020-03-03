using System.Collections;
using UnityEngine;

namespace Coffee
{
    public class GameplayManager : IManager
    {
        public enum States
        {
            Running,
            Paused,
            Tooltip,
        }

        public States State { get; private set; }
        
        public void Init()
        {
            State = States.Paused;
        }

        public void StartGame()
        {
            State = States.Running;
            GameManager.Instance.StartCoroutine(StartDayRoutine());
            GameManager.UI.ShowDialogue("Disembodied voice", "You've just woken up.\nYour goal is simple: leave for work.");
        }

        public void PauseForTooltip()
        {
            State = States.Tooltip;
        }

        public void SetState(States state)
        {
            State = state;
        }

        public bool IsPaused => State == States.Paused || State == States.Tooltip;
        
        public void DayComplete()
        {
            GameManager.Instance.StartCoroutine(DayCompleteRoutine());
        }

        private IEnumerator DayCompleteRoutine()
        {
            yield return new WaitForSeconds(1f);
            yield return GameManager.Instance.StartCoroutine(GameManager.UI.FadeOut());
            
            GameManager.Data.Day++;
            GameManager.Data.HadCoffee = false;
            
            yield return GameManager.Instance.StartCoroutine(StartDayRoutine());
        }

        private IEnumerator StartDayRoutine()
        {
            GameManager.House.ResetItems();
            GameManager.Character.ResetActor();
            yield return GameManager.Instance.StartCoroutine(GameManager.UI.FadeIn());
        }
    }
}