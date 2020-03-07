using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Coffee.HouseGen
{
    public class WallGeneration
    {
        private const string WallCursorName = "WallCursor";
        private const string WallCursorPrefabPath = "Assets/Prefabs/Editor/" + WallCursorName + ".prefab";
        
        public bool IsEnabled { get; private set; }
        
        private readonly HouseEditor editor;
        private readonly WallResolver wallResolver; 

        private Transform wallCursor;
        private Vector3 cursorGridPos;
        private Vector3 wallStart;
        private Vector3 wallEnd;
        private Directions wallDirection;
        private bool isDraggingWall;

        public WallGeneration(HouseEditor editor)
        {
            this.editor = editor;
            wallResolver = new WallResolver(editor);
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
            wallResolver.ClearProjection();
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
            wallEnd = wallStart;
            isDraggingWall = true;
            wallResolver.CacheExistingWalls();
        }

        private void StopDragging()
        {
            if (!isDraggingWall) return;
            wallResolver.ApplyWalls();
            isDraggingWall = false;
        }

        private void HandleDrag(SceneView scene, Vector2 mousePosition)
        {
            var result = CursorUtility.GetWorldPointFromMouse(scene, mousePosition);
            if (!result.IsSuccess || result.GridCornerPosition == cursorGridPos) return;
            
            cursorGridPos = result.GridCornerPosition;
            RecalculateWalls();
        }
        
        private void RecalculateWalls()
        {
            CalculateWallEnd();
            
            var startP = wallDirection == Directions.Horizontal ? wallStart.x : wallStart.z;
            var endP = wallDirection == Directions.Horizontal ? cursorGridPos.x : cursorGridPos.z;

            if (endP < startP)
            {
                var tmp = startP;
                startP = endP;
                endP = tmp;
            }

            var wallsToPlace = new List<WallResolver.WallRef>();
            for (var p = startP; p < endP; p++)
            {
                var xPos = wallDirection == Directions.Horizontal ? p : wallStart.x;
                var zPos = wallDirection == Directions.Vertical ? p : wallStart.z;
                
                wallsToPlace.Add(new WallResolver.WallRef {
                    Position = new Vector3(xPos, 0f, zPos),
                    Orientation = wallDirection == Directions.Horizontal ? WallOrientations.Right : WallOrientations.Up,
                });
            }

            wallResolver.ResolveWallProjection(wallsToPlace);
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