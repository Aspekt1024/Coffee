using UnityEditor;
using UnityEngine;

namespace Coffee.HouseGen
{
    public class DoorPlacement
    {
        public bool IsEnabled { get; private set; }
        
        private readonly ParentManagement parents;
        
        public DoorPlacement(ParentManagement parents)
        {
            this.parents = parents;
        }

        public void Enable() => IsEnabled = true;
        public void Disable() => IsEnabled = false;

        public void OnSceneGUI(SceneView scene, Event e)
        {
            if (!IsEnabled) return;
        }
    }
}