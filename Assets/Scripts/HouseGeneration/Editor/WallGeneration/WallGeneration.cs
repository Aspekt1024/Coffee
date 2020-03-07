using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Coffee.HouseGen
{
    public class WallGeneration
    {
        private const string WallPrefabPath = "Assets/Prefabs/Infrastructure/Walls/SingleWall.prefab";
        
        public bool IsEnabled { get; private set; }
        
        private readonly HouseEditor editor;
        
        private Vector3 wallStart;
        private Vector3 wallEnd;
        private bool isDraggingWall;
        
        private readonly List<Transform> walls = new List<Transform>();

        public WallGeneration(HouseEditor editor)
        {
            this.editor = editor;
        }
        
        public void Enable() => IsEnabled = true;

        public void Disable()
        {
            if (!IsEnabled) return;
            IsEnabled = false;   
            DestroyWalls();
            isDraggingWall = false;
        }

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
            if (!result.IsSuccess || result.GridCornerPosition == wallEnd) return;
            
            wallEnd = result.GridCornerPosition;
            RecalculateWalls();
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
            var wallPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(WallPrefabPath);
            if (wallPrefab == null)
            {
                Debug.LogError("Wall prefab missing: " + WallPrefabPath);
                return;
            }
            
            for (var p = startP; p < endP; p++)
            {
                var xPos = direction == Directions.Horizontal ? p : wallStart.x;
                var zPos = direction == Directions.Vertical ? p : wallStart.z;
                

                var checkPos = new Vector3(
                    xPos + (direction == Directions.Horizontal ? 0.5f : 0f),
                    0f,
                    zPos + (direction == Directions.Vertical ? 0.5f : 0f));
                
                var layers = LayerDefinitions.GetLayerMask(new[] {Layers.Interactible, Layers.Wall});
                if (Physics.CheckSphere(checkPos, 0.1f, layers)) continue;
                
                // Check corners
                
                // Check T

                var parent = editor.Parents.GetParent(Parents.GeneratedWalls);
                var wall = (GameObject)PrefabUtility.InstantiatePrefab(wallPrefab, parent);
                wall.GetComponent<Collider>().enabled = false;
                wall.name = "Generated Wall";
                
                wall.transform.position = new Vector3(xPos, 0, zPos);
                
                if (direction == Directions.Vertical)
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
    }
}