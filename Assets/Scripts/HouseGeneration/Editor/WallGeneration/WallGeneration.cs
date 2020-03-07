using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Coffee.HouseGen
{
    public class WallGeneration
    {
        private const string WallCursorName = "WallCursor";
        private const string WallCursorPrefabPath = "Assets/Prefabs/Editor/" + WallCursorName + ".prefab";
        private const string WallPrefabPath = "Assets/Prefabs/Infrastructure/Walls/SingleWall.prefab";
        
        public bool IsEnabled { get; private set; }
        
        private readonly HouseEditor editor;

        private Transform wallCursor;
        private Vector3 cursorGridPos;
        private Vector3 wallStart;
        private Vector3 wallEnd;
        private Directions wallDirection;
        private bool isDraggingWall;
        
        private readonly List<Transform> walls = new List<Transform>();

        public WallGeneration(HouseEditor editor)
        {
            this.editor = editor;
        }

        public void Enable()
        {
            IsEnabled = true;
            CreateCursor();
        }

        public void Disable()
        {
            if (!IsEnabled) return;
            IsEnabled = false;   
            DestroyWalls();
            isDraggingWall = false;
            Object.DestroyImmediate(wallCursor.gameObject);
        }

        public void OnSceneGUI(SceneView scene, Event e)
        {
            if (!IsEnabled) return;
            PositionCursor(scene, e.mousePosition);
            
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
            }
        }

        private void StartDragging(SceneView scene, Vector2 mousePosition)
        {
            var result = CursorUtility.GetWorldPointFromMouse(scene, mousePosition);
            if (!result.IsSuccess) return;
            wallStart = result.GridCornerPosition;
            isDraggingWall = true;
        }

        private void StopDragging()
        {
            if (!isDraggingWall) return;
            CreateWalls();
            isDraggingWall = false;
        }

        private void HandleDrag(SceneView scene, Vector2 mousePosition)
        {
            var result = CursorUtility.GetWorldPointFromMouse(scene, mousePosition);
            if (!result.IsSuccess || result.GridCornerPosition == cursorGridPos) return;
            
            cursorGridPos = result.GridCornerPosition;
            CalculateWallEnd();
            RecalculateWalls();
        }

        private void CalculateWallEnd()
        {
            var xDiff = Mathf.Abs(cursorGridPos.x - wallStart.x);
            var zDiff = Mathf.Abs(cursorGridPos.z - wallStart.z);

            wallDirection = zDiff > xDiff ? Directions.Vertical : Directions.Horizontal;
            
            wallEnd = new Vector3(
                wallDirection == Directions.Horizontal ? cursorGridPos.x : wallStart.x,
                0f,
                wallDirection == Directions.Vertical ? cursorGridPos.z : wallStart.z
                );
        }
        
        private void RecalculateWalls()
        {
            var startP = wallDirection == Directions.Horizontal ? wallStart.x : wallStart.z;
            var endP = wallDirection == Directions.Horizontal ? cursorGridPos.x : cursorGridPos.z;

            if (endP < startP)
            {
                var tmp = startP;
                startP = endP;
                endP = tmp;
            }
            
            DestroyWalls();
            var wallPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(WallPrefabPath);
            if (wallPrefab == null)
            {
                Debug.LogError("Wall prefab missing: " + WallPrefabPath);
                return;
            }
            
            for (var p = startP; p < endP; p++)
            {
                var xPos = wallDirection == Directions.Horizontal ? p : wallStart.x;
                var zPos = wallDirection == Directions.Vertical ? p : wallStart.z;
                
                var checkPos = new Vector3(
                    xPos + (wallDirection == Directions.Horizontal ? 0.5f : 0f),
                    0f,
                    zPos + (wallDirection == Directions.Vertical ? 0.5f : 0f));
                
                var layers = LayerDefinitions.GetLayerMask(new[] {Layers.Interactible, Layers.Wall});
                if (Physics.CheckSphere(checkPos, 0.1f, layers)) continue;
                
                // Check corners
                
                // Check T

                var parent = editor.Parents.GetParent(Parents.GeneratedWalls);
                var wall = (GameObject)PrefabUtility.InstantiatePrefab(wallPrefab, parent);
                wall.GetComponent<Collider>().enabled = false;
                wall.name = "Generated Wall";
                
                wall.transform.position = new Vector3(xPos, 0, zPos);
                
                if (wallDirection == Directions.Vertical)
                {
                    wall.transform.eulerAngles = new Vector3(0f, -90f, 0f);
                }
                
                walls.Add(wall.transform);
            }
        }
        
        private void CreateWalls()
        {
            var wallParent = editor.Parents.GetParent(Parents.Walls);
            var generatedWallParent = editor.Parents.GetParent(Parents.GeneratedWalls);
            foreach (var wall in walls)
            {
                if (wall.name == generatedWallParent.name) continue;
                wall.SetParent(wallParent);
                wall.GetComponent<Collider>().enabled = true;
            }
            walls.Clear();
        }

        private void DestroyWalls()
        {
            foreach (var wall in walls)
            {
                Object.DestroyImmediate(wall.gameObject);
            }
            walls.Clear();
        }

        private void CreateCursor()
        {
            if (wallCursor != null) return;
            var existingCursor = GameObject.Find(WallCursorName);
            if (existingCursor != null)
            {
                wallCursor = existingCursor.transform;
                return;
            }
            
            var cursorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(WallCursorPrefabPath);
            if (cursorPrefab == null)
            {
                Debug.LogError("Cursor prefab missing: " + WallCursorPrefabPath);
                return;
            }

            var editorParent = editor.Parents.GetParent(Parents.Editor);
            var cursor = (GameObject)PrefabUtility.InstantiatePrefab(cursorPrefab, editorParent);
            wallCursor = cursor.transform;
        }

        private void PositionCursor(SceneView scene, Vector2 mousePosition)
        {
            if (wallCursor == null) return;
            if (isDraggingWall)
            {
                wallCursor.position = wallEnd;   
            }
            else
            {
                var result = CursorUtility.GetWorldPointFromMouse(scene, mousePosition);
                if (!result.IsSuccess) return;
                wallCursor.position = result.GridCornerPosition;
            }
        }
    }
}