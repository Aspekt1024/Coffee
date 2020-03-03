using System.Collections;
using UnityEngine;

namespace Aspekt.UI
{
    public class UISlideAnimator : IUIAnimator
    {
        private readonly RectTransform rectTf;
        private readonly Vector2 closedPos;
        private readonly Vector2 openedPos;
        
        private const float AnimationDuration = 0.2f;
        private readonly float animationDistance;

        private Rect rect;
        
        public UISlideAnimator(Vector2 closedPos, Vector2 openedPos, RectTransform rectTf)
        {
            this.closedPos = closedPos;
            this.openedPos = openedPos;
            this.rectTf = rectTf;

            animationDistance = Vector2.Distance(closedPos, openedPos);
        }
        
        public IEnumerator AnimateIn(float delay = 0)
        {
            yield return new WaitForSecondsRealtime(delay);

            var startPos = rectTf.anchoredPosition;
            float dist = Vector2.Distance(startPos, openedPos);
            float animTime = AnimationDuration * (dist / animationDistance);
            
            float animStartTime = Time.unscaledTime;
            while (Time.unscaledTime < animStartTime + animTime)
            {
                rectTf.anchoredPosition = Vector2.Lerp(startPos, openedPos, (Time.unscaledTime - animStartTime) / animTime);;
                yield return null;
            }
            
            SetOpened();
        }

        public IEnumerator AnimateOut(float delay = 0)
        {
            yield return new WaitForSecondsRealtime(delay);

            var startPos = rectTf.anchoredPosition;
            float dist = Vector2.Distance(startPos, closedPos);
            float animTime = AnimationDuration * (dist / animationDistance);
            
            float animStartTime = Time.unscaledTime;
            while (Time.unscaledTime < animStartTime + animTime)
            {
                rectTf.anchoredPosition = Vector2.Lerp(startPos, closedPos, (Time.unscaledTime - animStartTime) / animTime);;
                yield return null;
            }
            
            SetClosed();
        }

        public void SetClosed()
        {
            rectTf.anchoredPosition = closedPos;
        }

        public void SetOpened()
        {
            rectTf.anchoredPosition = openedPos;
        }
    }
}