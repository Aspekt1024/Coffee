using System.Collections;
using UnityEngine;

namespace Aspekt.UI
{
    public class UIFadeAnimator : IUIAnimator
    {
        private readonly CanvasGroup canvasGroup;
        private readonly float fadeTime;
        
        public UIFadeAnimator(CanvasGroup canvasGroup, float fadeTime = 0.1f)
        {
            this.canvasGroup = canvasGroup;
            this.fadeTime = fadeTime;
        }
        
        public IEnumerator AnimateIn(float delay = 0f)
        {
            yield return new WaitForSecondsRealtime(delay);
            
            float startAlpha = canvasGroup.alpha;
            float ft = fadeTime * (1f - startAlpha) / 1f;
            
            float animStartTime = Time.unscaledTime;
            while (Time.unscaledTime < animStartTime + ft)
            {
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, (Time.unscaledTime - animStartTime) / ft);;
                yield return null;
            }
        }

        public IEnumerator AnimateOut(float delay = 0f)
        {
            yield return new WaitForSecondsRealtime(delay);
            
            float startAlpha = canvasGroup.alpha;
            float ft = fadeTime * startAlpha;
            
            float animStartTime = Time.unscaledTime;
            while (Time.unscaledTime < animStartTime + ft)
            {
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, (Time.unscaledTime - animStartTime) / ft);;
                yield return null;
            }
        }

        public void SetClosed()
        {
            canvasGroup.alpha = 0f;
        }

        public void SetOpened()
        {
            canvasGroup.alpha = 1f;
        }
    }
}