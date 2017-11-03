using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using ModBrick.Utility;

namespace ModBrick
{
    public class ModBrickInstance : MonoBehaviour
    {
        [SerializeField] private ModBrickMesh _brickMesh;
        [SerializeField] private ModBrickSnapping _brickSnap;
        public ModBrickMesh BrickMesh => _brickMesh;

        [SerializeField] private int _length = 1;
        [SerializeField] private int _height = 1;
        [SerializeField] private int _width = 1;

        [HideInInspector] public int Length = -1;
        [HideInInspector] public int Height = -1;
        [HideInInspector] public int Width = -1;

        public readonly IReactiveProperty<Vector3I> BrickSize = new ReactiveProperty<Vector3I>();

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
        }
    }
}