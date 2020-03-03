using System.Collections;
using UnityEngine;

namespace Coffee
{
    public class UIManager : MonoBehaviour, IManager
    {
#pragma warning disable 649
        [SerializeField] private DialogueUI dialogue;
        [SerializeField] private BlackoutOverlay blackout;
#pragma warning restore 649
        
        public void Init()
        {
            dialogue.Init();
            blackout.Init();
        }

        public void ShowDialogue(string speaker, string message)
        {
            var details = new DialogueUI.Details(speaker, message);
            dialogue.Populate(details);
            dialogue.Open();
            GameManager.Gameplay.SetState(GameplayManager.States.Tooltip);
        }

        public IEnumerator FadeOut()
        {
            yield return StartCoroutine(blackout.OpenRoutine());
        }

        public IEnumerator FadeIn()
        {
            yield return StartCoroutine(blackout.CloseRoutine());
        }

    }
}