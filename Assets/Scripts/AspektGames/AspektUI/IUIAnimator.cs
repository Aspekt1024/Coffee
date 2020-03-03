using System.Collections;

namespace Aspekt.UI
{
    public interface IUIAnimator
    {
        IEnumerator AnimateIn(float delay = 0f);
        IEnumerator AnimateOut(float delay = 0f);

        void SetClosed();
        void SetOpened();
    }
}