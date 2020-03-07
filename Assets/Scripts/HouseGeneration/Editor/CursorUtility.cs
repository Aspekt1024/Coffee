using UnityEditor;
using UnityEngine;

namespace Coffee.HouseGen
{
    public static class CursorUtility
    {
        public struct Result
        {
            public bool IsSuccess;
            public Vector3 RawPosition;
            public Directions Direction;
            public Vector3 GridCornerPosition;
            public Vector3 GridEdgePosition;
        }
        
        public static Result GetWorldPointFromMouse(SceneView scene, Vector2 mousepos)
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
                return new Result()
                {
                    IsSuccess = false,
                };
            }
            var pos = ray.GetPoint(startDist);
            pos.y = 0f;
            
            var cornerPos = pos;
            cornerPos.x = Mathf.Round(pos.x);
            cornerPos.z = Mathf.Round(pos.z);
            
            var xDiff = Mathf.Abs(pos.x - cornerPos.x);
            var zDiff = Mathf.Abs(pos.z - cornerPos.z);
            var direction = xDiff > zDiff ? Directions.Horizontal : Directions.Vertical;

            var edgePos = pos;
            edgePos.x = Mathf.Round(pos.x - (direction == Directions.Horizontal ? 0.5f : 0f));
            edgePos.z = Mathf.Round(pos.z - (direction == Directions.Vertical ? 0.5f : 0f));
            
            
            return new Result()
            {
                IsSuccess = true,
                RawPosition = pos,
                Direction = direction,
                GridCornerPosition = cornerPos,
                GridEdgePosition = edgePos,
            };
        }
        

    }
}