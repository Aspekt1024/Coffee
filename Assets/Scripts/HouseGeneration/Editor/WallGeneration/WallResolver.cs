using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Coffee.HouseGen
{
    /// <summary>
    /// Describes a wall piece's geometry from the origin
    /// </summary>
    public enum WallOrientations
    {
        Up, Right,
        UpLeft, UpRight,
        DownLeft, DownRight,
    }
    
    public class WallResolver
    {
        public struct WallRef
        {
            public Vector3 Position;
            public WallOrientations Orientation;
            public GameObject Wall;
        }

        private const string WallsPath = "Assets/Prefabs/Infrastructure/Walls/";
        private const string WallPrefabPath = WallsPath + "SingleWall.prefab";
        private const string CornerWallPrefabPath = WallsPath + "CornerWall.prefab";
        private const string WallProjectionPrefabPath = WallsPath + "SingleWallProjection.prefab";
        private const string CornerWallProjectionPrefabPath = WallsPath + "CornerWallProjection.prefab";
        
        private readonly HouseEditor editor;

        private readonly GameObject wallPrefab;
        private readonly GameObject cornerWallPrefab;
        private readonly GameObject wallProjectionPrefab;
        private readonly GameObject cornerWallProjectionPrefab;
        
        private readonly List<WallRef> existingWallRefs = new List<WallRef>();
        private readonly List<WallRef> projectedWallRefs = new List<WallRef>();
        private readonly List<GameObject> wallsToReplace = new List<GameObject>();
        
        public WallResolver(HouseEditor editor)
        {
            this.editor = editor;
            wallPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(WallPrefabPath);
            cornerWallPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(CornerWallPrefabPath);
            wallProjectionPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(WallProjectionPrefabPath);
            cornerWallProjectionPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(CornerWallProjectionPrefabPath);
            
            if (wallPrefab == null)
            {
                Debug.LogError("Wall prefab missing: " + WallPrefabPath);
            }
        }

        public void CacheExistingWalls()
        {
            existingWallRefs.Clear();
            var existingWalls = editor.Parents.GetChildren(Parents.Walls);
            var doors = editor.Parents.GetChildren(Parents.Doors);
            foreach (var w in existingWalls.Concat(doors))
            {
                existingWallRefs.Add(new WallRef {
                    Position = w.position,
                    Orientation = GetWallOrientation(w), 
                    Wall = w.gameObject});
            }
        }

        private static WallOrientations GetWallOrientation(Transform wall)
        {
            if (wall.CompareTag(TagDefnitions.Wall))
            {
                return PositionUtility.IsAngleNear(wall.eulerAngles.y, 0f)
                    ? WallOrientations.Right
                    : WallOrientations.Up;
            }
            else if (wall.CompareTag(TagDefnitions.CornerWall))
            {
                var angle = wall.eulerAngles.y;
                if (PositionUtility.IsAngleNear(angle, 0f)) return WallOrientations.UpRight;
                if (PositionUtility.IsAngleNear(angle, -90f)) return WallOrientations.UpLeft;
                if (PositionUtility.IsAngleNear(angle, 90f)) return WallOrientations.DownRight;
                return WallOrientations.DownLeft;
            }
            Debug.LogError(wall.name + " is an untagged wall. Unable to determine type.");
            return WallOrientations.Right;
        }
        
        /// <summary>
        /// Resolves the projected walls for placement, returning the instantiated projections
        /// </summary>
        public void ResolveWallProjection(List<WallRef> wallsToPlace)
        {
            var newWalls = new List<WallRef>();
            ClearProjection(); // TODO re-use existing projection models and remove/hide excess

            foreach (var w in wallsToPlace)
            {
                var overlappingWall = GetWallAtPosition(w, existingWallRefs);
                if (overlappingWall.Wall != null) continue;

                var wall = w;
                wall = ResolveProjectedWallType(wall, existingWallRefs);
                newWalls.Add(wall);
            }
            
            GenerateWallProjection(newWalls);
        }

        private void GenerateWallProjection(List<WallRef> wallRefs)
        {
            var parent = editor.Parents.GetParent(Parents.GeneratedWalls);

            foreach (var wallReference in wallRefs)
            {
                var wallRef = wallReference;
                var prefab = GetPrefab(wallRef, true);
                var wall = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
                wall.name = "Generated Wall";

                SetWallPositionAndRotation(wall.transform, wallRef);
                wallRef.Wall = wall;
                projectedWallRefs.Add(wallRef);
            }
        }

        private GameObject GetPrefab(WallRef wallRef, bool isProjection)
        {
            if (wallRef.Orientation == WallOrientations.Right || wallRef.Orientation == WallOrientations.Up)
            {
                return isProjection ? wallProjectionPrefab : wallPrefab;
            }
            return isProjection ? cornerWallProjectionPrefab : cornerWallPrefab;
        }

        private static void SetWallPositionAndRotation(Transform wall, WallRef wallRef)
        {
            var pos = wallRef.Position;
            float angle = 0f;
            switch (wallRef.Orientation)
            {
                case WallOrientations.Up:
                    angle = -90f;
                    break;
                case WallOrientations.Right:
                    break;
                case WallOrientations.UpLeft:
                    angle = -90f;
                    break;
                case WallOrientations.UpRight:
                    break;
                case WallOrientations.DownLeft:
                    pos += Vector3.forward;
                    angle = 180f;
                    break;
                case WallOrientations.DownRight:
                    pos += Vector3.forward;
                    angle = 90f;
                    break;
                default:
                    Debug.LogError("invalid orientation: " + wallRef.Orientation);
                    break;
            }

            wall.position = pos;
            wall.eulerAngles = new Vector3(0f, angle, 0f);
        }

        public void ClearProjection()
        {
            foreach (var wallRef in projectedWallRefs)
            {
                Object.DestroyImmediate(wallRef.Wall);
            }
            projectedWallRefs.Clear();
            ResetWallReplacements();
        }
        
        /// <summary>
        /// Commits the placement of the projected wall to the house's infrastructure
        /// </summary>
        public void ApplyWalls()
        {
            var wallParent = editor.Parents.GetParent(Parents.Walls);
            foreach (var wallRef in projectedWallRefs)
            {
                var prefab = GetPrefab(wallRef, false);
                var newWall = (GameObject) PrefabUtility.InstantiatePrefab(prefab, wallParent);
                SetWallPositionAndRotation(newWall.transform, wallRef);
                newWall.transform.SetParent(wallParent);
                newWall.name = "Generated Wall";
                Object.DestroyImmediate(wallRef.Wall);
            }
            projectedWallRefs.Clear();

            foreach (var wall in wallsToReplace)
            {
                Object.DestroyImmediate(wall);
            }
            wallsToReplace.Clear();
        }

        private static WallRef GetWallAtPosition(WallRef wallRef, List<WallRef> existingWalls)
        {
            foreach (var wall in existingWalls)
            {
                switch (wall.Orientation)
                {
                    case WallOrientations.Up:
                        if (!IsGridPointMatch(wallRef, wall)) continue;
                        if (wallRef.Orientation == WallOrientations.Up) return wall;
                        break;
                    case WallOrientations.Right:
                        if (!IsGridPointMatch(wallRef, wall)) continue;
                        if (wallRef.Orientation == WallOrientations.Right) return wall;
                        break;
                    case WallOrientations.UpLeft:
                        if (IsGridPointMatch(wallRef, wall) && wallRef.Orientation == WallOrientations.Up) return wall;
                        if (IsGridPointMatch(wallRef, wall, 1) && wallRef.Orientation == WallOrientations.Right) return wall;
                        break;
                    case WallOrientations.UpRight:
                        if (IsGridPointMatch(wallRef, wall)) return wall;
                        break;
                    case WallOrientations.DownLeft:
                        if (IsGridPointMatch(wallRef, wall, 0, 1) && wallRef.Orientation == WallOrientations.Up) return wall;
                        if (IsGridPointMatch(wallRef, wall, 1) && wallRef.Orientation == WallOrientations.Right) return wall;
                        break;
                    case WallOrientations.DownRight:
                        if (IsGridPointMatch(wallRef, wall, 0, 1) && wallRef.Orientation == WallOrientations.Up) return wall;
                        if (IsGridPointMatch(wallRef, wall, -1) && wallRef.Orientation == WallOrientations.Right) return wall;
                        break;
                    default:
                        Debug.LogError("Invalid orientation: " + wall.Orientation);
                        break;
                }
            }
            return new WallRef { Wall = null };
        }

        private WallRef ResolveProjectedWallType(WallRef wallRef, List<WallRef> existingWalls)
        {
            var orientation = wallRef.Orientation;
            var position = wallRef.Position;
            GameObject matchedWall = null;
            
            if (wallRef.Orientation == WallOrientations.Up)
            {
                var hasMatch = false;
                foreach (var w in existingWalls)
                {
                    if (wallsToReplace.Contains(w.Wall)) continue;
                    if (w.Orientation != WallOrientations.Right) continue;
                    
                    if (IsGridPointMatch(wallRef, w))
                    {
                        if (hasMatch) return wallRef; // TODO multiple matches are ignored for now
                        hasMatch = true;
                        matchedWall = w.Wall;
                        orientation = WallOrientations.UpRight;
                    }
                    else if (IsGridPointMatch(wallRef, w, -1))
                    {
                        if (hasMatch) return wallRef;
                        hasMatch = true;
                        matchedWall = w.Wall;
                        orientation = WallOrientations.UpLeft;
                    }
                    else if (IsGridPointMatch(wallRef, w, 0, 1))
                    {
                        if (hasMatch) return wallRef;
                        hasMatch = true;
                        matchedWall = w.Wall;
                        orientation = WallOrientations.DownRight;
                    }
                    else if (IsGridPointMatch(wallRef, w, -1, 1))
                    {
                        if (hasMatch) return wallRef;
                        hasMatch = true;
                        matchedWall = w.Wall;
                        orientation = WallOrientations.DownLeft;
                    }
                }
            }
            else if (wallRef.Orientation == WallOrientations.Right)
            {
                var hasMatch = false;
                foreach (var w in existingWalls)
                {
                    if (wallsToReplace.Contains(w.Wall)) continue;
                    if (w.Orientation != WallOrientations.Up) continue;
                    
                    if (IsGridPointMatch(wallRef, w))
                    {
                        if (hasMatch) return wallRef; // TODO multiple matches are ignored for now
                        hasMatch = true;
                        matchedWall = w.Wall;
                        orientation = WallOrientations.UpRight;
                    }
                    else if (IsGridPointMatch(wallRef, w, 1))
                    {
                        if (hasMatch) return wallRef;
                        hasMatch = true;
                        matchedWall = w.Wall;
                        position += Vector3.right;
                        orientation = WallOrientations.UpLeft;
                    }
                    else if (IsGridPointMatch(wallRef, w, 0, -1))
                    {
                        if (hasMatch) return wallRef;
                        hasMatch = true;
                        matchedWall = w.Wall;
                        position += Vector3.back;
                        orientation = WallOrientations.DownRight;
                    }
                    else if (IsGridPointMatch(wallRef, w, 1, -1))
                    {
                        if (hasMatch) return wallRef;
                        hasMatch = true;
                        matchedWall = w.Wall;
                        position += new Vector3(1, 0, -1);
                        orientation = WallOrientations.DownLeft;
                    }
                }
            }
            
            // TODO check projected corner walls 

            if (matchedWall != null)
            {
                wallsToReplace.Add(matchedWall);
                matchedWall.SetActive(false);
            }

            wallRef.Position = position;
            wallRef.Orientation = orientation;
            return wallRef;
        }

        private static bool IsGridPointMatch(WallRef originWall, WallRef otherWall, float offsetX = 0f, float offsetZ = 0f)
        {
            const float ProximityThreshold = 0.01f;
            var checkPoint = originWall.Position + new Vector3(offsetX, 0f, offsetZ);
            return Vector3.Distance(checkPoint, otherWall.Position) < ProximityThreshold;
        }

        private void ResetWallReplacements()
        {
            foreach (var w in wallsToReplace)
            {
                w.SetActive(true);
            }
            wallsToReplace.Clear();
        }

    }
}