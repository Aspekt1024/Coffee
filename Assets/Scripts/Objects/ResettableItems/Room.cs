using System;
using UnityEngine;

namespace Coffee
{
    public class Room : ResettableItem
    {
        #pragma warning disable 649
        [SerializeField] private Light[] lights;
        #pragma warning restore 649

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(TagDefnitions.Player))
            {
                EnableLights(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(TagDefnitions.Player))
            {
                EnableLights(false);
            }
        }

        private void EnableLights(bool enabled)
        {
            foreach (var l in lights)
            {
                l.enabled = enabled;
            }
        }

        public override void ResetState()
        {
            EnableLights(false);
        }
    }
}