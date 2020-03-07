using System.Collections.Generic;
using UnityEngine;

namespace Coffee.HouseGen
{
    public enum Parents
    {
        Editor,
        GeneratedWalls,
        Walls,
        Doors,
        Grid,
    }
    
    /// <summary>
    /// Manages the object parents for the house generator editor
    /// </summary>
    public class ParentManagement
    {
        
        private const string EditorParentPath = "Editor Objects";
        private const string GeneratedWallParentPath = EditorParentPath + "/Generated Walls";
        private const string GridParentPath = EditorParentPath + "/Grid";
        
        private const string WallParentPath = "House/Infrastructure/Walls";
        private const string DoorParentPath = "House/Infrastructure/Doors";
        
        private readonly Dictionary<Parents, string> pathDict = new Dictionary<Parents, string>()
        {
            { Parents.Editor, EditorParentPath },
            { Parents.GeneratedWalls, GeneratedWallParentPath },
            { Parents.Grid, GridParentPath },
            { Parents.Walls, WallParentPath },
            { Parents.Doors, DoorParentPath },
        };

        private readonly Dictionary<Parents, Transform> transformDict = new Dictionary<Parents, Transform>();
        
        public Transform GetParent(Parents parent)
        {
            if (!transformDict.ContainsKey(parent))
            {
                transformDict.Add(parent, FindParent(pathDict[parent]));
            }
            
            if (transformDict[parent] != null) return transformDict[parent];
            
            var tf = FindParent(pathDict[parent]);
            transformDict[parent] = tf;
            return tf;
        }

        public List<Transform> GetChildren(Parents parent)
        {
            var parentTf = GetParent(parent);
            var children = new List<Transform>();
            foreach (Transform tf in parentTf)
            {
                if (tf.parent == parentTf)
                {
                    children.Add(tf);
                }
            }
            return children;
        }
        
        private static Transform FindParent(string parentPath)
        {
            var objectNames = parentPath.Split('/');
            var parents = GameObject.FindGameObjectsWithTag(TagDefnitions.GeneratorParent);
            
            for (int index = objectNames.Length - 1; index >= 0; index--)
            {
                foreach (var parent in parents)
                {
                    if (parent.name == objectNames[index])
                    {
                        return CreateHierarchyAndReturnTop(parent.transform, objectNames, index + 1);
                    }
                }
            }
            return CreateHierarchyAndReturnTop(null, objectNames, 0);
        }

        private static Transform CreateHierarchyAndReturnTop(Transform root, string[] hierarchy, int fromIndex)
        {
            for (int index = fromIndex; index < hierarchy.Length; index++)
            {
                var newTf = new GameObject(hierarchy[index]).transform;
                newTf.tag = TagDefnitions.GeneratorParent;
                if (root != null)
                {
                    newTf.SetParent(root);
                }
                root = newTf;
            }
            return root;
        }
    }
}