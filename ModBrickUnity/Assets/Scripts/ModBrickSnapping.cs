﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using ModBrick.Utility;
using System.Linq;
using TMPro;

namespace ModBrick
{
    // this class is the reverse 'stud' part, it looks for a grid to hook on to
    public class ModBrickSnapping : MonoBehaviour
    {
        [SerializeField] private ModBrickSnapVisual _visualPrefab;
        [Header("Debugging")]
        [SerializeField]
        private GameObject _debugSnapCell;
        [SerializeField] private bool _showSnapCells;
        [SerializeField] private bool _showPotentialStudTargets;
        private List<GameObject> _snapCellVisuals;

        private bool _snapped = false;
        private ModBrickInstance _modBrickInstance;
        private ModBrickSnapVisual _visual;
        private ModBrickGrid _currentGrid;


        private int _length;
        private int _width;
        private int _height;


        private List<Vector3> _allSnapCellsLocal;
        private List<Vector3> _bottomSnapCells;
        private List<ModBrickCell> _potentialGridCellsWorld;
        private ModBrickCell _potentialBestSnapCell;
        private List<Vector3I> _potentialGridCells;

        public void Init(ModBrickInstance modBrickInstance)
        {
            _modBrickInstance = modBrickInstance;
            _modBrickInstance.BrickSize.Subscribe(size =>
            {
                BrickResized();
            });
        }

        void BrickResized()
        {
            if (_visual == null)
            {
                _visual = Instantiate(_visualPrefab, transform.position, transform.rotation);
                _visual.transform.SetParent(null);
            }
            _visual.SetMesh(_modBrickInstance.BrickMesh.GetMesh());
            _visual.Show();
            SetSize();
            GenerateCells();
        }

        void Update()
        {
            if (_snapped == false)
            {
                Vector3 newPosition;
                var updatedPosition = SnapUpdate(out newPosition);
                if (updatedPosition && CanSnap(GetCellsToTake()))
                {
                    _visual.UpdatePosition(newPosition);
                }
                //_visual.gameObject.transform.rotation = transform.rotation;
            }
            else
            {
                if (_visual != null)
                {
                    Destroy(_visual.gameObject);
                }
            }
            HandleSnapCellVisuals();
        }

        private void SetSize()
        {
            _length = _modBrickInstance.BrickSize.Value.x;
            _height = _modBrickInstance.BrickSize.Value.y;
            _width = _modBrickInstance.BrickSize.Value.z;
        }

        private bool SnapUpdate(out Vector3 position)
        {
            // snap cells positions are according to brick local position
            // convert them to grid local space
            _potentialGridCellsWorld = new List<ModBrickCell>();
            _potentialGridCells = new List<Vector3I>();
            foreach (var localSnapPosition in _bottomSnapCells)
            {
                var localSnapWorldPos = transform.TransformPoint(localSnapPosition);
                RaycastHit hit;
                if (Physics.Raycast(localSnapWorldPos, Vector3.down, out hit))
                {
                    var grid = hit.collider.gameObject.GetComponent<ModBrickGrid>();
                    if (grid != null)
                    {
                        var hitPos = hit.point;
                        var hitPosGridLocal = grid.transform.InverseTransformPoint(hitPos);
                        var gridCellPos = grid.ClosestGridCell(hitPosGridLocal);
                        var taken = grid.IsTaken(gridCellPos);
                        if (!taken)
                        {
                            _potentialGridCells.Add(gridCellPos);
                            var gridCellWorldPos = grid.GridCellToWorldPos(gridCellPos);
                            var cell = new ModBrickCell((gridCellWorldPos - transform.position).magnitude, grid, gridCellWorldPos, gridCellPos);
                            cell.TubeWorldPos = localSnapWorldPos;
                            cell.TubeLocalPos = localSnapPosition;
                            _potentialGridCellsWorld.Add(cell);
                        }
                    }
                }
            }
            if (_potentialGridCellsWorld != null && _potentialGridCellsWorld.Count != 0)
            {
                position = ChooseBestSnappingPositionNew();
                return true;
            }
            else
            {
                // find closest fit
            }
            position = Vector3.zero;
            return false;
        }

        private Vector3 ChooseBestSnappingPositionNew()
        {
            var highestCellY = _potentialGridCellsWorld.Max(x => x.WorldPos.y);
            var highestCells = _potentialGridCellsWorld.Where(x => x.WorldPos.y == highestCellY).ToList();

            var total = Vector3.zero;
            foreach (var p in highestCells)
            {
                total += p.WorldPos;
            }
            var avgPosition = total / highestCells.Count;
            var avgZIndex = (float)highestCells.Select(x => ModBrickMetrics.TubeLocalToGridPos(x.TubeLocalPos.z)).Average();
            var avgXIndex = (float)highestCells.Select(x => ModBrickMetrics.TubeLocalToGridPos(x.TubeLocalPos.x)).Average();
            avgPosition.x = avgPosition.x - avgXIndex*ModBrickMetrics.Unit - ModBrickMetrics.Unit/2;
            avgPosition.y = avgPosition.y - ModBrickMetrics.ThirdHeight / 2;
            avgPosition.z = avgPosition.z - avgZIndex*ModBrickMetrics.Unit - ModBrickMetrics.Unit/2;
            return avgPosition;
        }

        private Vector3 ChooseBestSnappingPosition()
        {
            var bestSnap = _potentialGridCellsWorld.Min(x => x.Distance); // todo: check up on this
            _potentialBestSnapCell = _potentialGridCellsWorld.FirstOrDefault(x => x.Distance == bestSnap);

            _currentGrid = _potentialBestSnapCell.CellGrid;

            // get the world position of that snappable stud:
            Vector3 position = _potentialBestSnapCell.WorldPos;

            // make sure entire brick is offset according to tube/stub being snapped to:
            position.x = position.x - _potentialBestSnapCell.TubeLocalPos.x;
            position.y = position.y - ModBrickMetrics.ThirdHeight / 2;
            position.z = position.z - _potentialBestSnapCell.TubeLocalPos.z;

            return position;
        }

        private void GenerateCells()
        {
            _bottomSnapCells = new List<Vector3>();
            for (int x = 0; x < _length; x++)
            {
                for (int z = 0; z < _width; z++)
                {
                    _bottomSnapCells.Add(new Vector3(x * ModBrickMetrics.Unit + ModBrickMetrics.Unit / 2,
                                            0,
                                            z * ModBrickMetrics.Unit + ModBrickMetrics.Unit / 2));
                }
            }
        }

        private void HandleSnapCellVisuals()
        {
            if (_showSnapCells && _snapCellVisuals == null)
            {
                _snapCellVisuals = new List<GameObject>();
                ShowSnapCells();
            }
            else if (!_showSnapCells && _snapCellVisuals != null)
            {
                HideSnapCells();
                _snapCellVisuals = null;
            }
        }

        private List<ModBrickCell> GetCellsToTake()
        {
            var highestPosition = _potentialGridCellsWorld.Max(x => x.WorldPos.y);
            var highestCells = _potentialGridCellsWorld.Where(x => x.WorldPos.y == highestPosition).ToList();
            return highestCells;
        }

        private bool CanSnap(List<ModBrickCell> potentialSnapCellsSameLevel)
        {
            // if they stud count is not the same as the inverse-stud count, that means we are colliding with something
            if (_potentialGridCellsWorld.Count != _bottomSnapCells.Count)
            {
                return false;
            }
            foreach (var c in potentialSnapCellsSameLevel)
            {
                if (c.CellGrid.IsTaken(c.GridPos))
                {
                    return false;
                }
            }
            return true;
        }

        // return true if success, false if failure
        public bool Snap()
        {

            var cellsToTake = GetCellsToTake();
            if (CanSnap(cellsToTake))
            {
                var parents = new List<ModBrickInstance>();
                foreach (var c in cellsToTake)
                {
                    parents.Add(c.CellGrid.ModBrickInstance);
                    c.CellGrid.TakeSpace(c.GridPos);
                }
                parents = parents.Distinct().ToList();
                foreach(var p in parents)
                {
                    p.AddChild(_modBrickInstance);
                }
                _visual.Hide();
                _visual.transform.SetParent(null);
                transform.position = _visual.transform.position;
                transform.position = ModBrickMetrics.RoundToGrid(transform.position);
                transform.SetParent(parents.FirstOrDefault().transform);
                _modBrickInstance.SetParents(parents);
                _snapped = true;
                return true;
            }
            return false;
        }

        public void Unsnap()
        {
            _snapped = false;
        }

        private void OnDrawGizmos()
        {
            if (_showPotentialStudTargets)
            {
                foreach (var v in _potentialGridCellsWorld)
                {
                    Gizmos.DrawLine(v.WorldPos, v.TubeWorldPos);
                }
            }
        }

        private void ShowSnapCells()
        {
            foreach (var v in _bottomSnapCells)
            {
                var instance = Instantiate(_debugSnapCell, transform.TransformPoint(v), Quaternion.identity);
                instance.transform.SetParent(transform);
                _snapCellVisuals.Add(instance);
                var tmpro = instance.GetComponentInChildren<TextMeshPro>();
                tmpro.text = string.Format("X: {0} \n Z: {1}", v.x, v.z);
            }
        }

        private void HideSnapCells()
        {
            foreach (var c in _snapCellVisuals)
            {
                Destroy(c);
            }
        }
    }
}