using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace ModBrick
{
    public class ModBrickSnapping : MonoBehaviour
    {
        private bool _snapped = false;
        [SerializeField] private ModBrickMesh _brickMesh;
        [SerializeField] private ModBrickSnapVisual _visualPrefab;
        private ModBrickSnapVisual _visual;
        private ModBrickGrid _grid;


        private int _length;
        private int _width;
        private int _height;
        
        private List<Vector3> _allSnapCellsLocal;
        private List<Vector3> _bottomSnapCells;
        private List<Vector3> _snapCellsSnappedWorld;
        private List<Vector3> _snapCellsSnappedGrid;

        void Awake()
        {
            _brickMesh.CurrentMesh.Subscribe(_ =>
            {
                MeshUpdated();
            });
            MeshUpdated();
            Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe(_ =>
            {
                Snap();
            });
        }

        void MeshUpdated()
        {
            if (_visual == null)
            {
                _visual = Instantiate(_visualPrefab, transform.position, transform.rotation);
                _visual.transform.SetParent(transform);
            }
            _visual.SetMesh(_brickMesh.GetMesh());
            _visual.Show();
            SetSize();
            GenerateCells();
        }

        void Update()
        {
            if (_grid == null)
            {
                _grid = FindObjectOfType<ModBrickGrid>();
            }
            if (_snapped == false)
            {
                Vector3 newPosition;
                var updatedPosition = ClosestSnapPosition(out newPosition);
                if (updatedPosition)
                {
                    _visual.UpdatePosition(newPosition);
                }
            }
            else
            {
                if (_visual != null)
                {
                    Destroy(_visual);
                }
            }
        }

        private void SetSize()
        {
            _length = _brickMesh.Length;
            _width = _brickMesh.Width;
            _height = _brickMesh.Height;
        }

        /*void OnDrawGizmos()
        {
            if(_snapCellsSnapped == null)
            {
                return;
            }
            var size = new Vector3(ModBrickMetrics.Unit, ModBrickMetrics.ThirdHeight, ModBrickMetrics.Unit);
            foreach(var c in _snapCellsSnapped)
            {
                //var worldPos = transform.TransformPoint(c);
                Gizmos.DrawCube(c, size);
            }
        }*/


        // from width, height and length, figure out where the brick is closest to in the grid.
        // from top, go down and find a free spot
        private bool ClosestSnapPosition(out Vector3 position)
        {
            // snap cells positions are according to brick local position
            // convert them to grid local space
            _snapCellsSnappedWorld = new List<Vector3>();
            _snapCellsSnappedGrid = new List<Vector3>();
            foreach (var v in _bottomSnapCells)
            {
                var worldPos = transform.position + v;
                var gridLocalPos = _grid.transform.InverseTransformPoint(worldPos);
                var gridCellPos = _grid.ClosestGridCell(gridLocalPos);
                // from closest cells, go down and check if snappable (brick below) and free
                var lowestFree = _grid.GetLowestFree((int)gridCellPos.x, (int)gridCellPos.z);
                if (lowestFree == -1)
                {
                    // cannot snap here
                    _snapCellsSnappedWorld = null;
                    break;
                }
                var lowestFreeXYZ = new Vector3(gridCellPos.x, lowestFree, gridCellPos.z);
                _snapCellsSnappedGrid.Add(lowestFreeXYZ);
                var gridCellWorldPos = _grid.GridCellToWorldPos(lowestFreeXYZ);
                _snapCellsSnappedWorld.Add(gridCellWorldPos);
            }
            if (_snapCellsSnappedWorld != null)
            {
                position = _snapCellsSnappedWorld[0];
                position.x = position.x - ModBrickMetrics.Unit / 2;
                position.y = position.y - ModBrickMetrics.ThirdHeight / 2;
                position.z = position.z - ModBrickMetrics.Unit / 2;
                return true;
            }
            position = Vector3.zero;
            return false;
        }

        private void GenerateCells()
        {
            _bottomSnapCells = new List<Vector3>();
            for (int x = 0; x < _length; x++)
            {
                for (int z = 0; z < _width; z++)
                {
                    // in the middle of every bottom cell
                    _bottomSnapCells.Add(new Vector3(x * ModBrickMetrics.Unit + ModBrickMetrics.Unit / 2,
                                            ModBrickMetrics.ThirdHeight / 2,
                                            z * ModBrickMetrics.Unit + ModBrickMetrics.Unit / 2));
                }
            }
        }

        public void Snap()
        {
            if(_grid.CanSnap(_snapCellsSnappedGrid))
            {
                _grid.TakeSpace(_snapCellsSnappedGrid, _height);
                _visual.Hide();
                transform.position = _visual.transform.position;
            }
        }
    }
}