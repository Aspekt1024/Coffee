using UnityEditor;
using UnityEngine;

namespace Coffee.HouseGen
{
    public static class CursorUtility
    {
        public static Vector3 GetWorldPointFromMouse(SceneView scene, Vector2 mousepos)
        {
            // Convert to screen point
            var point = new Vector2(
                mousepos.x * EditorGUIUtility.pixelsPerPoint,
                scene.camera.pixelHeight - mousepos.y * EditorGUIUtility.pixelsPerPoint
            );
            
            // Convert to world point
            var plane = new Plane(Vector3.up, Vector3.zero);
            var ray = scene.camera.ScreenPointToRay(point);
            if (!plane.Raycast(ray, out float startDist))
            {
                // TODO return null value?
                return Vector3.zero;
            }
            var pos = ray.GetPoint(startDist);
            
            // Snap to grid
            pos.x = Mathf.Round(pos.x);
            pos.z = Mathf.Round(pos.z);
            
            return pos;
        }

    }
}