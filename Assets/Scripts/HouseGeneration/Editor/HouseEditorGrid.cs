using UnityEditor;
using UnityEngine;

namespace Coffee.HouseGen
{
    public class HouseEditorGrid
    {
        private readonly ParentManagement parents;

        private GameObject grid;
        
        public HouseEditorGrid(ParentManagement parents)
        {
            this.parents = parents;
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
            Object.DestroyImmediate(parents.GetParent(Parents.Grid).gameObject);
            var gridPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Editor/Grid_20x20.prefab");
            var gridParent = parents.GetParent(Parents.Grid);
            grid = (GameObject)PrefabUtility.InstantiatePrefab(gridPrefab, gridParent);
            
            // TODO position at center of screen
            grid.transform.position = new Vector3(-2f, 0.1f, -2f);
        }
    }
}