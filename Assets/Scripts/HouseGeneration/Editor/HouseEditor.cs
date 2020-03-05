using UnityEditor;
using UnityEngine;

namespace Coffee.HouseGen
{
    public class HouseEditor : EditorWindow
    {
        private readonly WallGeneration wallGeneration = new WallGeneration();

        private const string EditorParentName = "Editor Objects";
        private Transform editorParent;
        private Transform EditorParent
        {
            get
            {
                if (editorParent != null) return editorParent;
                editorParent = ObjectHelpers.FindParent(EditorParentName);
                return editorParent;
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
            if (GUILayout.Button(wallGeneration.IsEnabled ? "Cancel Build Wall" : "Build wall"))
            {
                wallGeneration.Toggle();
            }
        }

        private void SceneGUI(SceneView sceneView)
        {
            var e = Event.current;
            wallGeneration.OnSceneGUI(sceneView, e);
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