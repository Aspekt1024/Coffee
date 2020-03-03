using Aspekt.UI;
using TMPro;
using UnityEngine;

namespace Coffee
{
    public class DialogueUI : UIPanel
    {
#pragma warning disable 649
        [SerializeField] private TextMeshProUGUI speaker;
        [SerializeField] private TextMeshProUGUI message;
#pragma warning restore 649

        private RectTransform rectTf;

        private void Awake()
        {
            rectTf = GetComponent<RectTransform>();
        }

        public struct Details
        {
            public readonly string Speaker;
            public readonly string Message;

            public Details(string speaker, string message)
            {
                Speaker = speaker;
                Message = message;
            }
        }
        
        public void Populate(Details details)
        {
            speaker.text = details.Speaker;
            message.text = details.Message;
        }

        public void OKClicked()
        {
            GameManager.Gameplay.SetState(GameplayManager.States.Running);
            Close();
        }
    }
}