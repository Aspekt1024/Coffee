using UnityEditor;
using UnityEngine;

namespace Coffee.HouseGen
{
    public class WallGeneration
    {
        public bool IsEnabled { get; private set; }
        
        private Vector3 wallStart;
        private Vector3 wallEnd;
        
        private readonly ParentManagement parents;
        
        private enum Directions
        {
            Vertical,
            Horizontal,
        }
        
        private bool isDraggingWall;


        public WallGeneration(ParentManagement parents)
        {
            this.parents = parents;
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
            wallStart = CursorUtility.GetWorldPointFromMouse(scene, mousePosition);
            isDraggingWall = true;
        }

        private void StopDragging()
        {
            CreateWalls();
            isDraggingWall = false;
        }

        private void HandleDrag(SceneView scene, Vector2 mousePosition)
        {
            var newWallEnd = CursorUtility.GetWorldPointFromMouse(scene, mousePosition);
            if (newWallEnd == wallEnd) return;
            wallEnd = newWallEnd;
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

                var parent = parents.GetParent(Parents.GeneratedWalls);
                var wall = (GameObject)PrefabUtility.InstantiatePrefab(wallPrefab, parent);
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
            var wallParent = parents.GetParent(Parents.Walls);
            var generatedWallParent = parents.GetParent(Parents.GeneratedWalls);
            var walls = generatedWallParent.GetComponentsInChildren<Transform>();
            foreach (var wall in walls)
            {
                if (wall.name == generatedWallParent.name) continue;
                wall.SetParent(wallParent);
            }
        }

        private void DestroyWalls()
        {
            // TODO destroy walls individually... maybe keep track of them in a List?
            Object.DestroyImmediate(parents.GetParent(Parents.GeneratedWalls).gameObject);
        }
    }
}