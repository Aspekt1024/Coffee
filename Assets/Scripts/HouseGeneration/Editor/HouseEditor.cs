using System;
using UnityEditor;
using UnityEngine;

namespace Coffee.HouseGen
{
    public enum Directions
    {
        Vertical,
        Horizontal,
    }
    
    public class HouseEditor : EditorWindow
    {
        public const float TileSize = 1f;
        
        public ParentManagement Parents { get; private set; }
        
        private WallGeneration walls;
        private DoorPlacement doors;

        private HouseEditorGrid houseEditorGrid;

        private enum EditModes
        {
            None,
            Walls,
            Door,
        }

        [MenuItem("Window/HouseEditor")]
        public static void ShowWindow()
        {
            GetWindow(typeof(HouseEditor));
        }
        
        private void OnGUI()
        {
            CheckKeyPresses(Event.current);
            
            titleContent = new GUIContent("House Editor");
            if (GUILayout.Button(walls.IsEnabled ? "Cancel Build Wall" : "Build Wall"))
            {
                SetEditMode(walls.IsEnabled ? EditModes.None : EditModes.Walls);
            }
            if (GUILayout.Button(doors.IsEnabled ? "Cancel Build Door" : "Build Door"))
            {
                SetEditMode(doors.IsEnabled ? EditModes.None : EditModes.Door);
            }

        }

        private void SetEditMode(EditModes mode)
        {
            DisableAll();
            
            switch (mode)
            {
                case EditModes.None:
                    houseEditorGrid.Disable();
                    break;
                case EditModes.Walls:
                    walls.Enable();
                    houseEditorGrid.Enable();
                    break;
                case EditModes.Door:
                    doors.Enable();
                    houseEditorGrid.Enable();
                    break;
                default:
                    Debug.LogError("invalid edit mode: " + mode);
                    break;
            }
        }

        private void OnSceneGUI(SceneView scene)
        {
            var e = Event.current;
            CheckKeyPresses(e);
            walls.OnSceneGUI(scene, e);
            doors.OnSceneGUI(scene, e);
        }

        private void CheckKeyPresses(Event e)
        {
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
            {
                DisableAll();
                Repaint();
            }
        }
        
        private void DisableAll()
        {
            walls.Disable();
            doors.Disable();
            houseEditorGrid.Disable();
        }
        
        private void OnEnable()
        {
            Parents = new ParentManagement();
            walls = new WallGeneration(this);
            doors = new DoorPlacement(this);
            houseEditorGrid = new HouseEditorGrid(this);
            
            SceneView.beforeSceneGui += OnSceneGUI;
        }

        private void OnDisable()
        {
            SceneView.beforeSceneGui -= OnSceneGUI;
        }
        
    }
}