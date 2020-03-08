using UnityEditor;
using UnityEngine;

namespace Coffee.HouseGen
{
    public class DoorPlacement
    {
        public enum Modes
        {
            NormalDoor,
            ExitDoor,
        }
        
        private const string DoorPrefabPath = "Assets/Prefabs/Infrastructure/Doors/Door.prefab";
        private const string ExitDoorPrefabPath = "Assets/Prefabs/Infrastructure/Doors/ExitDoor.prefab";
        
        public bool IsEnabled { get; private set; }
        
        private readonly HouseEditor editor;

        private Modes mode;
        private Transform door;
        private GameObject replacedWall;
        
        public DoorPlacement(HouseEditor editor)
        {
            this.editor = editor;
        }

        public void Enable(Modes editMode)
        {
            mode = editMode;
            IsEnabled = true;
            CreateDoor();
        }

        public void Disable()
        {
            IsEnabled = false;
            DestroyDoor();
            
            if (replacedWall != null)
            {
                replacedWall.SetActive(true);
                replacedWall = null;
            }
        }

        public void OnSceneGUI(SceneView scene, Event e)
        {
            if (!IsEnabled) return;
            PositionDoor(scene, e.mousePosition);
            
            if (e.type == EventType.MouseDown && e.button == 0)
            {
                PlaceDoor();
                e.Use();
            }
        }

        private void PositionDoor(SceneView scene, Vector2 mousePosition)
        {
            var result = CursorUtility.GetWorldPointFromMouse(scene, mousePosition);
            if (!result.IsSuccess) return;

            door.position = result.GridEdgePosition;
            
            if (result.Direction == Directions.Horizontal)
            {
                door.eulerAngles = Vector3.zero;
            }
            else
            {
                door.eulerAngles = new Vector3(0f, -90f, 0f);
            }

            CheckOverlappingWalls(result);

        }

        private void CreateDoor()
        {
            var prefabPath = mode == Modes.NormalDoor ? DoorPrefabPath : ExitDoorPrefabPath;
            
            var doorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (doorPrefab == null)
            {
                Debug.LogError("Door prefab missing: " + prefabPath);
                return;
            }
            
            var parent = editor.Parents.GetParent(Parents.Editor);
            var doorObj = (GameObject)PrefabUtility.InstantiatePrefab(doorPrefab, parent);
            door = doorObj.transform;
            door.name = "Generated Door";
        }

        private void PlaceDoor()
        {
            if (replacedWall != null)
            {
                Object.DestroyImmediate(replacedWall);
            }
            door.SetParent(editor.Parents.GetParent(Parents.Doors));
            door = null;
            CreateDoor();
        }

        private void DestroyDoor()
        {
            if (door != null)
            {
                Object.DestroyImmediate(door.gameObject);
            }
        }

        private void CheckOverlappingWalls(CursorUtility.Result result)
        {
            var checkOffset = (result.Direction == Directions.Horizontal ? Vector3.right : Vector3.forward) * 0.5f;
            var checkPos = result.GridEdgePosition + checkOffset;

            var overlappingWall = GetOverlappingWall(checkPos);
            if (overlappingWall == null)
            {
                if (replacedWall != null)
                {
                    bool isSamePosition = Vector3.Distance(result.GridEdgePosition, replacedWall.transform.position) < 0.1f;
                    bool isSameDirection = PositionUtility.GetWallDirection(replacedWall.transform) == result.Direction;
                    if (isSameDirection && isSamePosition)
                    {
                        return;
                    }
                    replacedWall.SetActive(true);
                    replacedWall = null;
                }
                return;
            }
            
            if (replacedWall != null && overlappingWall != replacedWall)
            {
                replacedWall.SetActive(true);
            }
            replacedWall = overlappingWall;
            replacedWall.SetActive(false);
        }
        
        private GameObject GetOverlappingWall(Vector3 pos)
        {
            var colliders = new Collider[2];
            var layers = LayerUtil.GetMask(new[] {Layers.Wall, Layers.Interactible});
            
            var numColliders = Physics.OverlapSphereNonAlloc(pos, 0.1f, colliders, layers);
            for (int i = 0; i < numColliders; i++)
            {
                if (colliders[i].gameObject == door.gameObject) continue;
                return colliders[i].gameObject;
            }

            return null;
        }
    }
}