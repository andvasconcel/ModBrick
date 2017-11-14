using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModBrick;

namespace ModBrick.UserInterface
{
    public class SimpleControls : MonoBehaviour
    {
        [SerializeField] private ModBrickInstance _modBrickPrefab;
        [SerializeField] private float _speed;
        [SerializeField] private Collider _cameraLookTarget;
        [SerializeField] private Material _brickMaterial;
        [SerializeField] private Material _selectedMaterial;
        private Vector2 _lastMousePos;
        private ModBrickInstance _currentBrick;


        private float _rotationSpeed = 0.01f;

        private void SwitchBrick(ModBrickInstance newBrick)
        {
            if (_currentBrick != null)
            {
                _currentBrick.SetMaterial(_brickMaterial);
            }
            _currentBrick = newBrick;
            _currentBrick.SetMaterial(_selectedMaterial);
        }

        private void RotateCamera(Vector2 mousePosition)
        {
            if (_lastMousePos != null)
            {
                var delta = mousePosition - _lastMousePos;
                var horizontal = delta.x;
                var currentAngle = Mathf.Deg2Rad * transform.rotation.y;
                var newAngle = currentAngle + horizontal * _rotationSpeed;
                var circlePos = CirclePosition(newAngle, new Vector3(63, 0, 63), 100f);
                transform.position = circlePos;
                transform.rotation = Quaternion.Euler(45, Mathf.Rad2Deg*newAngle, 0);
            }
            _lastMousePos = mousePosition;
        }

        private void RotateCam()
        {
            var currentAngle = Mathf.Deg2Rad * transform.rotation.eulerAngles.y;
            Debug.Log(currentAngle);
            var newAngle = currentAngle + Time.deltaTime;
            var circlePos = CirclePosition(newAngle, new Vector3(63, 0, 63), 100f);
            transform.position = circlePos;
            //transform.LookAt(_)
        }

        private Vector3 CirclePosition(float angle, Vector3 center, float radius)
        {
            var x = Mathf.Cos(angle) * radius + center.x;
            var z = Mathf.Sin(angle) * radius + center.z;
            return new Vector3(x, radius, z);
        }


        void Update()
        {
            RotateCam();
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    ModBrickInstance brick = hit.transform.gameObject.GetComponent<ModBrickInstance>();
                    if (brick != null && brick.Selectable)
                    {
                        SwitchBrick(brick);
                    }
                }
            }

            if (Input.GetMouseButton(1))
            {
                RotateCamera(Input.mousePosition);
            }
            else
            {
                _lastMousePos = Input.mousePosition;
            }


            if (Input.GetKeyDown(KeyCode.N) && (_currentBrick == null || _currentBrick.Placed))
            {
                _currentBrick = Instantiate(_modBrickPrefab, new Vector3(0, ModBrickMetrics.FullHeight, 0), Quaternion.identity);
                _currentBrick.SetMaterial(_selectedMaterial);
            }
            if (_currentBrick != null)
            {
                if (!_currentBrick.Placed)
                {
                    DoTranslate();
                    DoRotate();
                }
                if (Input.GetKeyDown(KeyCode.P))
                {
                    PlaceBrick();
                }
                if (Input.GetKeyDown(KeyCode.F))
                {
                    FreeBrick();
                }
            }
        }

        private void DoTranslate()
        {
            var x = Input.GetAxis("Horizontal");
            var y = Input.GetKey(KeyCode.LeftControl) ? -1 : 0;
            y = Input.GetKey(KeyCode.Space) ? 1 : y;
            var z = Input.GetAxis("Vertical");
            var pos = _currentBrick.gameObject.transform.position;
            pos.x = pos.x + x * Time.deltaTime * _speed;
            pos.y = pos.y + y * Time.deltaTime * _speed;
            pos.z = pos.z + z * Time.deltaTime * _speed;

            _currentBrick.gameObject.transform.position = pos;
        }

        private void DoRotate()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                _currentBrick.Rotate();
            }
        }

        private void PlaceBrick()
        {
            if (_currentBrick.Place())
            {
                _currentBrick.SetMaterial(_brickMaterial);
                _currentBrick = null;
            }
        }

        private void FreeBrick()
        {
            if (_currentBrick.Free())
            {
                // ?
            }
        }


    }
}