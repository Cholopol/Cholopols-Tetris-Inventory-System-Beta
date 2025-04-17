using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
            public static List<Vector2Int> RotatePoints(List<Vector2Int> points, Dir dir)
            {
                List<Vector2Int> rotatedPoints = new();

                foreach (var point in points)
                {
                    int x = point.x;
                    int y = point.y;

                    switch (dir)
                    {
                        case Dir.Left:
                            rotatedPoints.Add(new Vector2Int(-y, x));
                            break;
                        case Dir.Up:
                            rotatedPoints.Add(new Vector2Int(-x, -y));
                            break;
                        case Dir.Right:
                            rotatedPoints.Add(new Vector2Int(y, -x));
                            break;
                        case Dir.Down:
                            rotatedPoints.Add(new Vector2Int(x, y));
                            break;
                        default:
                            Debug.LogWarning("Unsupported rotation angle!");
                            return points;
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

            public static bool IsRotated(Dir dir)
            {
                return dir == Dir.Left || dir == Dir.Right;
            }

        }

        #endregion

        #region Get Object By InstanceID
        /// <summary>
        /// Manage the global caching system of objects through InstanceID
        /// </summary>
        public static class InstanceIDManager
        {
            // Main dictionary: stores the mapping between InstanceID and objects
            private static Dictionary<int, Object> _instanceIDMap =
                new Dictionary<int, Object>();

            // Auxiliary dictionary: prevent objects from being registered repeatedly (optional)
            private static Dictionary<Object, int> _reverseLookupMap =
                new Dictionary<Object, int>();

            // Thread safe lock (if multi-threaded operation is required)
            private static readonly object _lock = new object();

            /// <summary>
            /// Register objects to the caching system
            /// </summary>
            public static void Register(Object obj)
            {
                if (obj == null) return;

                lock (_lock)
                {
                    int instanceID = obj.GetInstanceID();

                    // If the object is already registered, remove the old record first
                    if (_reverseLookupMap.ContainsKey(obj))
                    {
                        int oldID = _reverseLookupMap[obj];
                        _instanceIDMap.Remove(oldID);
                        _reverseLookupMap.Remove(obj);
                    }

                    // Add new Record
                    _instanceIDMap[instanceID] = obj;
                    _reverseLookupMap[obj] = instanceID;
                }
            }

            /// <summary>
            /// Cancel Object
            /// </summary>
            public static void Unregister(Object obj)
            {
                if (obj == null) return;

                lock (_lock)
                {
                    if (_reverseLookupMap.TryGetValue(obj, out int instanceID))
                    {
                        _instanceIDMap.Remove(instanceID);
                        _reverseLookupMap.Remove(obj);
                    }
                }
            }

            /// <summary>
            /// Retrieve objects through InstanceID
            /// </summary>
            public static T GetObject<T>(int instanceID) where T : Object
            {
                lock (_lock)
                {
                    if (_instanceIDMap.TryGetValue(instanceID, out Object obj))
                    {
                        // Automatically clear destroyed objects
                        if (obj == null)
                        {
                            _instanceIDMap.Remove(instanceID);
                            _reverseLookupMap.Remove(obj);
                            return null;
                        }
                        Debug.Log(obj.GetType().ToString() + obj.name);
                        return obj as T;
                    }
                    return null;
                }
            }

            /// <summary>
            /// Clear all caches (e.g. called during scene switching)
            /// </summary>
            public static void Clear()
            {
                lock (_lock)
                {
                    _instanceIDMap.Clear();
                    _reverseLookupMap.Clear();
                }
            }
        }
        #endregion

        public static class TetrisItemUtilities
        {
            //public static bool IsItemLocationOnGrid(this TetrisItem item)
            //{
            //    if (item.CurrentInventoryContainer is TetrisItemGrid)
            //    {
            //        return true;
            //    }
            //    else
            //    {
            //        return false;
            //    }
            //}

            public static bool TryStackItems(TetrisItem source, TetrisItem target)
            {
                if (!source.TryGetItemComponent<StackableComponent>(out var srcStack)) return false;
                if (!target.TryGetItemComponent<StackableComponent>(out var tarStack)) return false;

                return srcStack.TryMergeStack(tarStack);
            }

            /// <summary>
            /// Force Updating targetUIObject PointerEnter event
            /// </summary>
            /// <param name="targetUIObject"></param>
            public static void TriggerPointerEnter(GameObject targetUIObject)
            {
                IPointerEnterHandler handler = targetUIObject.GetComponent<IPointerEnterHandler>();
                if (handler != null)
                {
                    PointerEventData eventData = new PointerEventData(EventSystem.current);
                    eventData.pointerEnter = targetUIObject;
                    eventData.position = Input.mousePosition;

                    handler.OnPointerEnter(eventData);
                }
            }

        }

    }

}