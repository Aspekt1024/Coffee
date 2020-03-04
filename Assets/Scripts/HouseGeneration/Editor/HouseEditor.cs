using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Coffee.HouseGen
{
    public class HouseEditor : EditorWindow
    {
        private bool isBuildingWall;
        private Vector3 wallStart;
        private Vector3 wallEnd;
        private bool isDraggingWall;

        private const string WallParentName = "Walls";
        private const string GeneratedWallParentName = "Generated Walls";

        private Transform generatedWallParent;
        private Transform GeneratedWallParent
        {
            get
            {
                if (generatedWallParent != null)
                {
                    return generatedWallParent;
                }

                generatedWallParent = FindParent(GeneratedWallParentName);
                return generatedWallParent;
            }
        }


        [MenuItem("Window/HouseEditor")]
        public static void ShowWindow()
        {
            GetWindow(typeof(HouseEditor));
        }

        private void OnGUI()
        {
            titleContent = new GUIContent("House Editor");
            if (GUILayout.Button(isBuildingWall ? "Cancel Build Wall" : "Build wall"))
            {
                isBuildingWall = !isBuildingWall;
            }
        }

        private void SceneGUI(SceneView sceneView)
        {
            var e = Event.current;
            
            if (!isBuildingWall) return;
            
            if (e.type == EventType.MouseDown && e.button == 0)
            {
                wallStart = GetWorldPointFromMouse(sceneView, e.mousePosition);
                isDraggingWall = true;
                e.Use();
            }

            if (isDraggingWall)
            {
                if (e.type == EventType.MouseDrag)
                {
                    e.Use();
                    var newWallEnd = GetWorldPointFromMouse(sceneView, e.mousePosition);
                    if (newWallEnd == wallEnd) return;
                    wallEnd = newWallEnd;
                    RecalculateWalls(sceneView);
                }
            
                if (e.type == EventType.MouseUp && e.button == 0)
                {
                    CreateWalls();
                    isDraggingWall = false;
                    e.Use();
                }
                
                if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
                {
                    DestroyWalls();
                    e.Use();
                }
            }

        }

        private enum Directions
        {
            Vertical,
            Horizontal,
        }

        private void RecalculateWalls(SceneView scene)
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
            for (var p = startP; p < endP; p++)
            {
                var wallPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Walls/SingleWall.prefab");
                var wall = (GameObject)PrefabUtility.InstantiatePrefab(wallPrefab, GeneratedWallParent);
                wall.name = "Generated Wall";
                
                var xPos = direction == Directions.Horizontal ? p + 1 : wallStart.x;
                var zPos = direction == Directions.Vertical ? p : wallStart.z;
                wall.transform.position = new Vector3(xPos, 0, zPos);
                
                if (direction == Directions.Vertical)
                {
                    wall.transform.Rotate(Vector3.forward, 90f);
                }
            }
        }
        
        private void CreateWalls()
        {
            var wallParent = FindParent(WallParentName);
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
                DestroyImmediate(generatedWallParent.gameObject);
                generatedWallParent = null;
            }
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

        private static Transform FindParent(string parentName)
        {
            var parents = GameObject.FindGameObjectsWithTag(TagDefnitions.GeneratorParent);
            foreach (var parent in parents)
            {
                if (parent.name == parentName)
                {
                    return parent.transform;
                }
            }

            var newParent = new GameObject(parentName).transform;
            newParent.tag = TagDefnitions.GeneratorParent;
            return newParent;
        }
        
        private void OnEnable()
        {
            SceneView.beforeSceneGui += SceneGUI;
        }

        private void OnDisable()
        {
            SceneView.beforeSceneGui -= SceneGUI;
        }
        
    }
}