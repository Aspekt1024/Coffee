using UnityEditor;
using UnityEngine;

namespace Coffee.HouseGen
{
    public class HouseEditorGrid
    {
        private const string GridPrefabPath = "Assets/Prefabs/Editor/Grid_20x20.prefab";
        
        private readonly HouseEditor editor;

        private GameObject grid;
        
        public HouseEditorGrid(HouseEditor editor)
        {
            this.editor = editor;
        }

        public void Enable()
        {
            if (grid == null)
            {
                CreateGrid();
            }
            grid.SetActive(true);
        }
        
        public void Disable()
        {
            if (grid == null) return;
            grid.SetActive(false);
        }

        private void CreateGrid()
        {
            Object.DestroyImmediate(editor.Parents.GetParent(Parents.Grid).gameObject);
            var gridPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(GridPrefabPath);
            var gridParent = editor.Parents.GetParent(Parents.Grid);
            grid = (GameObject)PrefabUtility.InstantiatePrefab(gridPrefab, gridParent);
            
            // TODO position at center of screen
            grid.transform.position = new Vector3(-2f, 0.1f, -2f);
        }
    }
}