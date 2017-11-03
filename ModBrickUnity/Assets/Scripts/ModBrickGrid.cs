using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModBrick.Utility;

namespace ModBrick
{

    // this class is the 'stud' part of a brick
    public class ModBrickGrid : MonoBehaviour
    {
        [SerializeField] private bool _drawGridDebug;

        [SerializeField] private int _gridX = 32;
        [SerializeField] private int _gridY = 96;
        [SerializeField] private int _gridZ = 32;

        private Vector3I _brickSize;

        private float _xSize = ModBrickMetrics.Unit;
        private float _ySize = ModBrickMetrics.ThirdHeight;
        private float _zSize = ModBrickMetrics.Unit;

        private bool[,,] _grid;

        public void SetSize(Vector3I size)
        {
            _brickSize = size;
            _gridX = size.x;
            _gridY = 1; // todo: should this ever be more than 1?
            _gridZ = size.z;
        }

        void Start()
        {
            _grid = new bool[_gridX, _gridY, _gridZ];
            if (_brickSize == null)
            {
                _brickSize = new Vector3I(_gridX, _gridY, _gridZ);
            }
        }

        public Vector3I ClosestGridCell(Vector3 localPosition)
        {
            var x = Mathf.RoundToInt((localPosition.x - _xSize / 2) / _xSize);
            var y = 0;
            //var y = Mathf.RoundToInt((localPosition.y - _ySize / 2) / _ySize);
            var z = Mathf.RoundToInt((localPosition.z - _zSize / 2) / _zSize);
            x = Mathf.Clamp(x, 0, _gridX);
            //y = Mathf.Clamp(y, 0, _gridY);
            z = Mathf.Clamp(z, 0, _gridZ);
            return new Vector3I(x, y, z);
        }

        public Vector3 GridCellToWorldPos(Vector3I gridCellPos)
        {
            var x = gridCellPos.x * _xSize + _xSize / 2;
            // because bricks snap ABOVE bricks, not INSIDE, adjust the height
            var y = gridCellPos.y * _ySize + _ySize / 2 + _ySize * _brickSize.y;
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
                    if (!OutOfBounds(c))
                    {
                        TakeSpace(c);
                    }
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
            if (OutOfBounds(gridCellPos))
            {
                Debug.LogError("Attempted to take space that was out of bounds");
                return;
            }
            _grid[gridCellPos.x, gridCellPos.y, gridCellPos.z] = true;
        }

        public void FreeSpace(Vector3I gridCellPos)
        {
            if (OutOfBounds(gridCellPos))
            {
                Debug.LogError("Attempted to free space that was out of bounds");
                return;
            }
            _grid[gridCellPos.x, gridCellPos.y, gridCellPos.z] = false;
        }

        public bool IsTaken(Vector3I gridCellPoint)
        {
            if (OutOfBounds(gridCellPoint))
            {
                Debug.LogError("Attempted to check if out of bounds space was free - " + gridCellPoint);
                return true;
            }
            return _grid[gridCellPoint.x, gridCellPoint.y, gridCellPoint.z];
        }

        public bool CanSnap(List<Vector3I> gridCellPoints)
        {
            foreach (var c in gridCellPoints)
            {
                if (!OutOfBounds(c)) // if you're trying to snap here and potentially somewhere else, it's not THIS grid's problem
                {
                    if (IsTaken(c))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool OutOfBounds(Vector3I gridCellPoint)
        {
            if (gridCellPoint.x >= _gridX ||
                gridCellPoint.y >= _gridY ||
                gridCellPoint.z >= _gridZ ||
                gridCellPoint.x < 0 ||
                gridCellPoint.y < 0 ||
                gridCellPoint.z < 0)
            {
                return true;
            }
            return false;
        }

        void OnDrawGizmos()
        {
            if (_drawGridDebug)
            {
                var size = new Vector3(ModBrickMetrics.Unit, ModBrickMetrics.ThirdHeight, ModBrickMetrics.Unit);
                for (int x = 0; x < _gridX; x++)
                {
                    for (int y = 0; y < _gridY; y++)
                    {
                        for (int z = 0; z < _gridZ; z++)
                        {
                            var gridPos = new Vector3I(x, y, z);
                            var worldPos = GridCellToWorldPos(gridPos);
                            var taken = IsTaken(gridPos);
                            if (!taken)
                            {
                                DrawCube(worldPos, size);
                            }
                        }
                    }
                }
            }
        }

        void DrawCube(Vector3 center, Vector3 size)
        {
            Gizmos.DrawCube(center, size);
        }
    }
}