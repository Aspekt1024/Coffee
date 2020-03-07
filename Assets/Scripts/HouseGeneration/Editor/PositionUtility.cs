using UnityEngine;

namespace Coffee.HouseGen
{
    public static class PositionUtility
    {
        public static Directions GetWallDirection(Transform wall)
        {
            var wallAngle = ClampAngle(wall.eulerAngles.y);
            return IsAngleNear(wallAngle, 0f) ? Directions.Horizontal : Directions.Vertical;
        }

        /// <summary>
        /// Clamps the angle, in degrees, to be within 0f (inclusive) and 360f (exclusive)
        /// </summary>
        public static float ClampAngle(float angle)
        {
            while (angle < 0f)
            {
                angle += 360f;
            }
            while (angle >= 360f)
            {
                angle -= 360f;
            }
            return angle;
        }

        /// <summary>
        /// Determines if an angle is close to the specified angle. The angle should be between 0 and 360 degrees (see ClampAngle)
        /// </summary>
        public static bool IsAngleNear(float angle, float value, float threshold = 1f)
        {
            float difference = angle - value;
            return Mathf.Abs(difference) < threshold || Mathf.Abs(difference) > 360f - threshold;
        }

        /// <summary>
        /// Returns the position of a tile for a given x,y coordinate (in TILE_SIZE units), relative to the origin transform
        /// </summary>
        public static Vector3 GetRotatedCoord(Transform originTf, int x, int y)
        {
            var dir = Quaternion.Euler(originTf.eulerAngles) * new Vector3(x, 0f, y) * HouseEditor.TileSize;
            return dir + originTf.transform.position;
        }
    }
}