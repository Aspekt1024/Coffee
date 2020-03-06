using UnityEditor;
using UnityEngine;

namespace Coffee.HouseGen
{
    public class DoorPlacement
    {
        public bool IsEnabled { get; private set; }
        
        private readonly ParentManagement parents;

        private Transform door;
        
        public DoorPlacement(ParentManagement parents)
        {
            this.parents = parents;
        }

        public void Enable()
        {
            IsEnabled = true;
            CreateDoor();
        }

        public void Disable()
        {
            IsEnabled = false;
            DestroyDoor();
        }

        public void OnSceneGUI(SceneView scene, Event e)
        {
            if (!IsEnabled) return;
            PositionDoor(scene, e.mousePosition);
        }

        private void PositionDoor(SceneView scene, Vector2 mousePosition)
        {
            var pos = CursorUtility.GetWorldPointFromMouse(scene, mousePosition);
            pos.y = 0f;
            door.position = pos;
        }

        private void CreateDoor()
        {
            var doorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Doors/Door.prefab");
            var parent = parents.GetParent(Parents.Editor);
            var doorObj = (GameObject)PrefabUtility.InstantiatePrefab(doorPrefab, parent);
            door = doorObj.transform;
        }

        private void DestroyDoor()
        {
            if (door != null)
            {
                Object.DestroyImmediate(door.gameObject);
            }
        }
    }
}