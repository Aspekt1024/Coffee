using System.Collections;
using System.Linq;
using UnityEngine;

namespace Coffee
{
    public class CameraManager : IManager
    {
        private Camera mainCamera;

        private Vector3 originalPos;
        private Quaternion originalRot;

        private Coroutine zoomRoutine;

        public void Init()
        {
            mainCamera = Object.FindObjectsOfType<Camera>().First(c => c.CompareTag("MainCamera"));
            originalPos = mainCamera.transform.position;
            originalRot = mainCamera.transform.rotation;
        }

        public void ResetState()
        {
            mainCamera.transform.SetPositionAndRotation(originalPos, originalRot);
        }

        public void Zoom(Transform target)
        {
            if (zoomRoutine != null) GameManager.Instance.StopCoroutine(zoomRoutine);
            zoomRoutine = GameManager.Instance.StartCoroutine(ZoomTestRoutine(target.position, target.rotation));
        }
        
        public void UnZoom()
        {
            if (zoomRoutine != null) GameManager.Instance.StopCoroutine(zoomRoutine);
            zoomRoutine = GameManager.Instance.StartCoroutine(ZoomTestRoutine(originalPos, originalRot));
        }

        private IEnumerator ZoomTestRoutine(Vector3 toPos, Quaternion toRot)
        {
            var timer = 0f;
            const float duration = 1.5f;

            var fromPos = mainCamera.transform.position;
            var fromRot = mainCamera.transform.rotation;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                var t = timer / duration;
                var ratio = t * t * (3.0f - 2.0f * t); // bezier blend

                var pos = Vector3.Lerp(fromPos, toPos, ratio);
                var rot = Quaternion.Lerp(fromRot, toRot, ratio);
                
                mainCamera.transform.SetPositionAndRotation(pos, rot);
                
                yield return null;
            }
        }
    }
}