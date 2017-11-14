using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using ModBrick.Utility;
using ColorPickerUnity;

namespace ModBrick
{
    // handles 'transform' logic like rescaling, rotation, positioning, children/parents
    public class ModBrickInstance : MonoBehaviour
    {
        [Header("Submodules")]
        [SerializeField]
        private ModBrickMesh _brickMesh;
        [SerializeField] private ModBrickSnapping _brickSnap;

        public ModBrickMesh BrickMesh => _brickMesh;
        public ModBrickSnapping BrickSnap => _brickSnap;

        [Header("Settings")]
        [SerializeField]
        private int _length = 1;
        [SerializeField] private int _height = 1;
        [SerializeField] private int _width = 1;
        public bool Selectable = true;

        [SerializeField] private MeshRenderer _renderer;

        [HideInInspector] public int Length = -1;
        [HideInInspector] public int Height = -1;
        [HideInInspector] public int Width = -1;

        private Color _color;
        public Color Color => _color;

        private List<ModBrickInstance> _children;
        private List<ModBrickInstance> _parents;

        private bool _placed = false;
        public bool Placed => _placed;

        public readonly IReactiveProperty<Vector3I> BrickSize = new ReactiveProperty<Vector3I>();



        public void AddChild(ModBrickInstance child)
        {
            if (_children == null)
            {
                _children = new List<ModBrickInstance>();
            }
            _children.Add(child);
        }

        public void SetParents(List<ModBrickInstance> parents)
        {
            _parents = parents;
        }

        void OnValidate()
        {
            bool resize = false;
            if (Length != _length)
            {
                Length = _length;
                resize = true;
            }
            if (Width != _width)
            {
                Width = _width;
                resize = true;
            }
            if (Height != _height)
            {
                Height = _height;
                resize = true;
            }
            if (resize)
            {
                BrickSize.Value = new Vector3I(Length, Height, Width);
            }
        }

        public void Rotate()
        {
            var oldLength = _length;
            var oldWidth = _width;
            _length = oldWidth;
            _width = oldLength;
            OnValidate();
        }

        public bool Place()
        {
            if (_brickSnap.Snap())
            {
                var grid = gameObject.AddComponent<ModBrickGrid>();
                grid.SetSize(BrickSize.Value); // todo: reactive magic
                _placed = true;
                return true;
            }
            return false;
        }

        // opposite of place
        public bool Free()
        {
            if (!_placed)
            {
                return false;
            }
            if (_children != null)
            {
                foreach (var c in _children)
                {
                    c.transform.SetParent(transform);
                    c.Free();
                }
            }
            _placed = false;
            return true;
        }

        public void SetColor(Color color)
        {
            _renderer.material.color = color;
        }

        public void SetMaterial(Material m)
        {
            _renderer.material = m;
            if (_color != null)
            {
                _renderer.material.color = _color;
            }
        }

        void Awake()
        {
            BrickSize.Value = new Vector3I(_length, _height, _width);
            if (_brickMesh != null)
            {
                _brickMesh.Init(this);
            }
            if (_brickSnap != null)
            {
                _brickSnap.Init(this);
            }
            _color = ColorExtensions.RandomHueColor(1, 1);
            SetColor(_color);
        }

    }
}