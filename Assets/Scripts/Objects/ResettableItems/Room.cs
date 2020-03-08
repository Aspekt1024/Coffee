using System;
using UnityEngine;

namespace Coffee
{
    public class Room : Resettable
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

        private void EnableLights(bool isEnabled)
        {
            foreach (var l in lights)
            {
                l.enabled = isEnabled;
            }
        }

        public override void ResetState()
        {
            EnableLights(false);
        }
    }
}