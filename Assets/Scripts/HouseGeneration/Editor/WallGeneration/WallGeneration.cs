using UnityEditor;
using UnityEngine;

namespace Coffee.HouseGen
{
    public class WallGeneration
    {
        private Vector3 wallStart;
        private Vector3 wallEnd;

        private const string WallParentName = "Walls";
        private const string GeneratedWallParentName = "Generated Walls";
        
        private enum Directions
        {
            Vertical,
            Horizontal,
        }
        
        private Transform generatedWallParent;
        private Transform GeneratedWallParent
        {
            get
            {
                if (generatedWallParent != null) return generatedWallParent;
                generatedWallParent = ObjectHelpers.FindParent(GeneratedWallParentName);
                return generatedWallParent;
            }
        }

        public bool IsEnabled { get; private set; }
        private bool isDraggingWall;

        public void Toggle() => IsEnabled = !IsEnabled;

        public void OnSceneGUI(SceneView scene, Event e)
        {
            if (!IsEnabled) return;
            
            if (e.type == EventType.MouseDown && e.button == 0)
            {
                StartDragging(scene, e.mousePosition);
                e.Use();
            }

            if (!isDraggingWall) return;
            
            switch (e.type)
            {
                case EventType.MouseDrag:
                    HandleDrag(scene, e.mousePosition);
                    e.Use();
                    break;
                case EventType.MouseUp:
                    if (e.button == 0)
                    {
                        StopDragging();
                        e.Use();
                    }
                    break;
                case EventType.KeyDown:
                    if (e.keyCode == KeyCode.Escape)
                    {
                        Cancel();
                        e.Use();
                    }
                    break;
            }
        }

        private void StartDragging(SceneView scene, Vector2 mousePosition)
        {
            wallStart = GetWorldPointFromMouse(scene, mousePosition);
            isDraggingWall = true;
        }

        private void StopDragging()
        {
            CreateWalls();
            isDraggingWall = false;
        }

        private void Cancel()
        {
            DestroyWalls();
            isDraggingWall = false;
        }

        private void HandleDrag(SceneView scene, Vector2 mousePosition)
        {
            var newWallEnd = GetWorldPointFromMouse(scene, mousePosition);
            if (newWallEnd == wallEnd) return;
            wallEnd = newWallEnd;
            RecalculateWalls();
        }
        
        private static Vector3 GetWorldPointFromMouse(SceneView scene, Vector2 mousepos)
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
                Debug.LogWarning("Failed to hit plane");
                return Vector3.zero;
            }
            var pos = ray.GetPoint(startDist);
            
            // Snap to grid
            pos.x = Mathf.Round(pos.x);
            pos.z = Mathf.Round(pos.z);
            
            return pos;
        }
        
        

        private void RecalculateWalls()
        {
            var xDiff = Mathf.Abs(wallEnd.x - wallStart.x);
            var zDiff = Mathf.Abs(wallEnd.z - wallStart.z);

            var direction = zDiff > xDiff ? Directions.Vertical : Directions.Horizontal;
            var startP = direction == Directions.Horizontal ? wallStart.x : wallStart.z;
            var endP = direction == Directions.Horizontal ? wallEnd.x : wallEnd.z;

            if (endP < startP)
            {
                var tmp = startP;
                startP = endP;
                endP = tmp;
            }
            
            DestroyWalls();
            var wallPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Walls/SingleWall.prefab");
            for (var p = startP; p < endP; p++)
            {
                var xPos = direction == Directions.Horizontal ? p : wallStart.x;
                var zPos = direction == Directions.Vertical ? p : wallStart.z;
                

                var checkPos = new Vector3(
                    xPos + (direction == Directions.Horizontal ? 0.5f : 0f),
                    0f,
                    zPos + (direction == Directions.Vertical ? 0.5f : 0f));
                
                var layers = 1 << LayerMask.NameToLayer("Wall");
                if (Physics.CheckSphere(checkPos, 0.1f, layers)) continue;
                
                // Check corners
                
                // Check T
                
                var wall = (GameObject)PrefabUtility.InstantiatePrefab(wallPrefab, GeneratedWallParent);
                wall.name = "Generated Wall";
                
                wall.transform.position = new Vector3(xPos, 0, zPos);
                
                if (direction == Directions.Vertical)
                {
                    wall.transform.Rotate(Vector3.back, 90f);
                }
            }
        }
        
        private void CreateWalls()
        {
            var wallParent = ObjectHelpers.FindParent(WallParentName);
            var walls = GeneratedWallParent.GetComponentsInChildren<Transform>();
            foreach (var wall in walls)
            {
                if (wall.name == GeneratedWallParentName) continue;
                wall.SetParent(wallParent);
            }
        }

        private void DestroyWalls()
        {
            if (generatedWallParent != null)
            {
                Object.DestroyImmediate(generatedWallParent.gameObject);
                generatedWallParent = null;
            }
        }
    }
}