using Aspekt.UI;
using UnityEngine;

namespace Coffee
{
    public class BlackoutOverlay : UIPanel
    {
#pragma warning disable 649
        [SerializeField] private float fadeTime = 1.2f;
#pragma warning restore 649
        
        protected override IUIAnimator CreateAnimator()
        {
            return new UIFadeAnimator(canvasGroup, fadeTime);
        }
    }
}