﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModBrick.Utility;

namespace ModBrick
{
    public class ModBrickGrid : MonoBehaviour
    {
        [SerializeField] private bool _drawGridDebug;

        [SerializeField] private int _x = 32;
        [SerializeField] private int _y = 96;
        [SerializeField] private int _z = 32;

        private float _xSize = ModBrickMetrics.Unit;
        private float _ySize = ModBrickMetrics.ThirdHeight;
        private float _zSize = ModBrickMetrics.Unit;

        private bool[,,] _grid;

        void Start()
        {
            _grid = new bool[_x, _y, _z];
        }

        public Vector3I ClosestGridCell(Vector3 localPosition)
        {
            var x = Mathf.RoundToInt((localPosition.x - _xSize / 2) / _xSize);
            var y = Mathf.RoundToInt((localPosition.y - _ySize / 2) / _ySize);
            var z = Mathf.RoundToInt((localPosition.z - _zSize / 2) / _zSize);
            return new Vector3I(x, y, z);
        }

        public Vector3 GridCellToWorldPos(Vector3I gridCellPos)
        {
            var x = gridCellPos.x * _xSize + _xSize / 2;
            var y = gridCellPos.y * _ySize + _ySize / 2;
            var z = gridCellPos.z * _zSize + _zSize / 2;
            var localPos = new Vector3(x, y, z);
            return transform.TransformPoint(localPos);
        }

        public void TakeSpace(List<Vector3I> gridCellpointsXZ, int heightLevels)
        {
            var baseY = (int)gridCellpointsXZ[0].y;
            for (int y = baseY; y < baseY + heightLevels; y++)
            {
                foreach (var c in gridCellpointsXZ)
                {
                    TakeSpace(c);
                }
            }
        }

        public void TakeSpace(List<Vector3I> gridCellPoints)
        {
            foreach (var c in gridCellPoints)
            {
                TakeSpace(c);
            }
        }

        public void TakeSpace(Vector3I gridCellPos)
        {
            if (OutOfBounds(gridCellPos.x, gridCellPos.y, gridCellPos.z))
            {
                Debug.LogError("Attempted to take space that was out of bounds");
                return;
            }
            _grid[gridCellPos.x, gridCellPos.y, gridCellPos.z] = true;
        }

        public void FreeSpace(Vector3I gridCellPos)
        {
            if (OutOfBounds(gridCellPos.x, gridCellPos.y, gridCellPos.z))
            {
                Debug.LogError("Attempted to free space that was out of bounds");
                return;
            }
             _grid[gridCellPos.x, gridCellPos.y, gridCellPos.z] = false;
        }

        public bool IsTaken(int x, int y, int z)
        {
            if (OutOfBounds(x, y, z))
            {
                Debug.LogError("Attempted to check if out of bounds space was free");
                return true;
            }
            return _grid[x, y, z];
        }

        public bool CanSnap(List<Vector3I> gridCellPoints)
        {
            foreach (var c in gridCellPoints)
            {
                //Debug.Log((int)c.x + " - " + (int)c.y + " - " + (int)c.z);
                if (IsTaken(c.x, c.y, c.z))
                {
                    return false;
                }
            }
            return true;
        }

		// soon obsolete
        public int GetLowestFree(int x, int z)
        {
            if (OutOfBounds(x, 0, z))
            {
                return -1; // out of grid
            }
            for (int y = 0; y < _y; y++)
            {
                if (_grid[x, y, z] == false)
                {
                    return y;
                }
            }
            return -1; // no free
        }

        private bool OutOfBounds(int x, int y, int z)
        {
            if (x >= _x || y >= _y || z >= _z || x < 0 || y < 0 || z < 0)
            {
                return true;
            }
            return false;
        }
        //private Color _free = new Color(0, 1, 0, 0.1f);
        //private Color _taken = new Color(1, 0, 0, 0.1f);

        void OnDrawGizmos()
        {
            if (_drawGridDebug)
            {
                var size = new Vector3(ModBrickMetrics.Unit, ModBrickMetrics.ThirdHeight, ModBrickMetrics.Unit);
                for (int x = 0; x < _x; x++)
                {
                    for (int y = 0; y < _y; y++)
                    {
                        for (int z = 0; z < _z; z++)
                        {
                            var worldPos = transform.TransformPoint(new Vector3(
                                x * size.x + size.x / 2,
                                y * size.y + size.y / 2,
                                z * size.z + size.z / 2));
                            var taken = IsTaken(x, y, z);
                            if (!taken)
                            {
                                DrawCube(worldPos, size/*, taken ? _taken : _free*/);
                            }
                        }
                    }
                }
            }
        }

        void DrawCube(Vector3 center, Vector3 size/*, Color color*/)
        {
            Gizmos.DrawCube(center, size);
        }
    }
}