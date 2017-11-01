using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        private List<Vector3> _bottomSnapCells;
        private List<Vector3> _snapCellsSnapped;

        void Update()
        {
            if (_grid == null)
            {
                _grid = FindObjectOfType<ModBrickGrid>();
            }
            if(_bottomSnapCells == null) // todo: reactive magic
            {
                SetSize();
                GenerateCells();
            }
            if (_snapped == false)
            {
                if (_visual == null)
                {
                    _visual = Instantiate(_visualPrefab, transform.position, transform.rotation);
                    _visual.transform.SetParent(transform);
                    _visual.SetMesh(_brickMesh.GetMesh()); // todo: do some reactive magic here
                }
                var closest = ClosestSnapPosition();
                //_visual.UpdatePosition()
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

        void OnDrawGizmos()
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
        }
        

        // from width, height and length, figure out where the brick is closest to in the grid.
        // from top, go down and find a free spot
        private Vector3 ClosestSnapPosition()
        {
            // snap cells positions are according to brick local position
            // convert them to grid local space
            _snapCellsSnapped = new List<Vector3>();
            foreach(var v in _bottomSnapCells)
            {
                var worldPos = transform.position + v;
                var gridLocalPos = _grid.transform.InverseTransformPoint(worldPos);
                var gridCellPos = _grid.ClosestGridCell(gridLocalPos);
                var lowestFree = _grid.GetLowestFree((int)gridCellPos.x, (int)gridCellPos.z);
                if(lowestFree == -1)
                {
                    // cannot snap here
                }
                var lowestFreeXYZ = new Vector3(gridCellPos.x, lowestFree, gridCellPos.z);
                var gridCellWorldPos = _grid.GridCellToWorldPos(lowestFreeXYZ);
                _snapCellsSnapped.Add(gridCellWorldPos);
            }

            

            // from closest cells, go down and check if snappable (brick below) and free
            // check that all lower are able to fit here
            // if so, that should be good enough
            return Vector3.zero;
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

        void Snap()
        {

        }
    }
}