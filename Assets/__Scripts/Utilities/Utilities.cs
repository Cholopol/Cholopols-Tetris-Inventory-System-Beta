using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChosTIS
{
    public static class Utilities
    {
        #region Rotation Methods

        public static class RotationHelper
        {
            public static Dir GetNextDir(Dir dir)
            {
                return dir switch
                {
                    Dir.Down => Dir.Left,
                    Dir.Left => Dir.Up,
                    Dir.Up => Dir.Right,
                    Dir.Right => Dir.Down,
                    _ => Dir.Down
                };
            }

            public static int GetRotationAngle(Dir dir)
            {
                return dir switch
                {
                    Dir.Down => 0,
                    Dir.Left => 90,
                    Dir.Up => 180,
                    Dir.Right => 270,
                    _ => 0
                };
            }

            // Rotate clockwise 90 degrees
            public static List<Vector2Int> RotatePointsClockwise(List<Vector2Int> points)
            {
                List<Vector2Int> rotatedPoints = new();
                foreach (var point in points)
                {
                    rotatedPoints.Add(new Vector2Int(-point.y, point.x));
                }
                return rotatedPoints;
            }

            public static Vector2Int GetRotationOffset(Dir dir, int width, int height)
            {
                return dir switch
                {
                    Dir.Down => new Vector2Int(0, 0),
                    Dir.Left => new Vector2Int(width - 1, 0),
                    Dir.Up => new Vector2Int(width - 1, height - 1),
                    Dir.Right => new Vector2Int(0, height - 1),
                    _ => Vector2Int.zero
                };
            }

            // Rotation points set
            private static List<Vector2Int> RotatePoints(List<Vector2Int> points, Dir dir)
            {
                List<Vector2Int> rotatedPoints = new();

                foreach (var point in points)
                {
                    int x = point.x;
                    int y = point.y;

                    switch (dir)
                    {
                        case Dir.Left:
                            rotatedPoints.Add(new Vector2Int(-y, x)); // 顺时针90度
                            break;
                        case Dir.Up:
                            rotatedPoints.Add(new Vector2Int(-x, -y)); // 顺时针180度
                            break;
                        case Dir.Right:
                            rotatedPoints.Add(new Vector2Int(y, -x)); // 顺时针270度
                            break;
                        case Dir.Down:
                            rotatedPoints.Add(new Vector2Int(x, y)); // 顺时针360度（不旋转）
                            break;
                        default:
                            Debug.LogWarning("不支持的旋转角度！");
                            return points; // 返回原始点集
                    }
                }

                return rotatedPoints;
            }

            public static Vector2Int GetDirForwardVector(Dir dir)
            {
                switch (dir)
                {
                    default:
                    case Dir.Down: return new Vector2Int(0, -1);
                    case Dir.Left: return new Vector2Int(-1, 0);
                    case Dir.Up: return new Vector2Int(0, +1);
                    case Dir.Right: return new Vector2Int(+1, 0);
                }
            }

            public static Dir GetDir(Vector2Int from, Vector2Int to)
            {
                if (from.x < to.x)
                {
                    return Dir.Right;
                }
                else
                {
                    if (from.x > to.x)
                    {
                        return Dir.Left;
                    }
                    else
                    {
                        if (from.y < to.y)
                        {
                            return Dir.Up;
                        }
                        else
                        {
                            return Dir.Down;
                        }
                    }
                }
            }

        }
        #endregion


    }
}

