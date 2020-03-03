using System.Collections;
using UnityEngine;

namespace Aspekt.UI
{
    public interface IUIPanel
    {

        /// <summary>
        /// Initialises the UI Panel
        /// </summary>
        void Init();
        
        /// <summary>
        /// Opens the UI panel using its animation
        /// </summary>
        void Open(float delay = 0f);
        
        /// <summary>
        /// Closes the UI panel using its animation
        /// </summary>
        void Close(float delay = 0f);
        
        /// <summary>
        /// Same as Open() but can be used synchronously in a coroutine
        /// </summary>
        IEnumerator OpenRoutine(float delay = 0f);
        
        /// <summary>
        /// Same as Close() but can be used synchronously in a coroutine
        /// </summary>
        IEnumerator CloseRoutine(float delay = 0f);
        
        /// <summary>
        /// Opens the UI panel immediately (no animation)
        /// </summary>
        void OpenImmediate();
        
        /// <summary>
        /// Closes the UI panel immediately (no animation)
        /// </summary>
        void CloseImmediate();
    }
}