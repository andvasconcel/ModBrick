using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using ModBrick.Utility;
using System.Linq;

namespace ModBrick
{
    public class ModBrickSnapping : MonoBehaviour
    {
        [SerializeField] private ModBrickSnapVisual _visualPrefab;
        private bool _snapped = false;
        private ModBrickInstance _parent;
        private ModBrickSnapVisual _visual;
        private ModBrickGrid _grid;


        private int _length;
        private int _width;
        private int _height;

        private List<Vector3> _allSnapCellsLocal;
        private List<Vector3> _bottomSnapCells;
        private List<Vector3> _snapCellsSnappedWorld;
        private List<Vector3I> _snapCellsSnappedGrid;

        public void Init(ModBrickInstance parent)
        {
            _parent = parent;
            _parent.BrickSize.Subscribe(size =>
            {
                BrickResized();
            });
        }

        void BrickResized()
        {
            if (_visual == null)
            {
                _visual = Instantiate(_visualPrefab, transform.position, transform.rotation);
                _visual.transform.SetParent(transform);
            }
            _visual.SetMesh(_parent.BrickMesh.GetMesh());
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
            _length = _parent.BrickSize.Value.x;
            _height = _parent.BrickSize.Value.y;
            _width = _parent.BrickSize.Value.z;
        }

        private bool ClosestSnapPosition(out Vector3 position)
        {
            // snap cells positions are according to brick local position
            // convert them to grid local space
            _snapCellsSnappedWorld = new List<Vector3>();
            _snapCellsSnappedGrid = new List<Vector3I>();
            foreach (var v in _bottomSnapCells)
            {
                var localSnapWorldPos = transform.position + v;
                RaycastHit hit;
                if (Physics.Raycast(localSnapWorldPos, Vector3.down, out hit))
                {
                    var grid = hit.collider.gameObject.GetComponent<ModBrickGrid>();
                    //Debug.Log(hit.collider.gameObject);
                    if (grid != null)
                    {
                        var hitPos = hit.point;
                        //ar gridLocalPos = _grid.transform.InverseTransformPoint(localSnapWorldPos);
                        var gridCellPos = _grid.ClosestGridCell(hitPos);
                        var taken = _grid.IsTaken(gridCellPos);
                        if (!taken)
                        {
                            _snapCellsSnappedGrid.Add(gridCellPos);
                            var gridCellWorldPos = _grid.GridCellToWorldPos(gridCellPos);
                            _snapCellsSnappedWorld.Add(gridCellWorldPos);
                        }
                    }
                }
            }
            if (_snapCellsSnappedWorld != null && _snapCellsSnappedWorld.Count != 0)
            {
                var bestSnap = _snapCellsSnappedWorld.Min(x => (x-transform.position).magnitude); // todo: check up on this
                position = _snapCellsSnappedWorld.FirstOrDefault(x => (x-transform.position).magnitude == bestSnap);
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
            if (_grid.CanSnap(_snapCellsSnappedGrid))
            {
                _grid.TakeSpace(_snapCellsSnappedGrid, _height);
                _visual.Hide();
                transform.position = _visual.transform.position;
            }
        }
    }
}