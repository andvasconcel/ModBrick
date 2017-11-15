using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModBrick;

namespace ModBrick.UserInterface
{
    public class BrickControls : MonoBehaviour
    {
        [SerializeField] private ModBrickInstance _modBrickPrefab;
        [SerializeField] private float _speed;
        [SerializeField] private float _speedMultiplier = 1.5f;
        [SerializeField] private Collider _cameraLookTarget;
        [SerializeField] private Material _brickMaterial;
        [SerializeField] private Material _selectedMaterial;
        [SerializeField] private Camera _cam;
        private ModBrickInstance _currentBrick;

        private void SwitchBrick(ModBrickInstance newBrick)
        {
            if (_currentBrick != null)
            {
                _currentBrick.SetMaterial(_brickMaterial);
            }
            _currentBrick = newBrick;
            _currentBrick.SetMaterial(_selectedMaterial);
        }

        void Update()
        {
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

            if (Input.GetKeyDown(KeyCode.N) && (_currentBrick == null || _currentBrick.Placed))
            {
                _currentBrick = Instantiate(_modBrickPrefab, new Vector3(0, ModBrickMetrics.FullHeight, 0), Quaternion.identity);
                _currentBrick.SetMaterial(_selectedMaterial);
            }
            if (_currentBrick != null)
            {
                HandleBrickMovement();
            }
            GetXAxis();
        }

        private void HandleBrickMovement()
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

        private void DoTranslate()
        {
            var x = GetXAxis();
            var y = Input.GetKey(KeyCode.LeftControl) ? -1 : 0;
            y = Input.GetKey(KeyCode.Space) ? 1 : y;
            var z = GetZAxis();
            var pos = _currentBrick.gameObject.transform.position;
            var mult = Input.GetKey(KeyCode.LeftShift) ? _speedMultiplier : 1;
            pos.x = pos.x + x * Time.deltaTime * _speed * mult;
            pos.y = pos.y + y * Time.deltaTime * _speed * mult;
            pos.z = pos.z + z * Time.deltaTime * _speed * mult;

            _currentBrick.gameObject.transform.position = pos;
        }
        private const float FullRot = 2 * Mathf.PI;

        private float GetXAxis()
        {
            var angle = Mathf.Repeat(Mathf.Deg2Rad * _cam.transform.rotation.eulerAngles.y, FullRot);
            // -45 to 45 degrees
            if (angle >= 0.875f * FullRot || angle < 0.125f * FullRot)
            {
                return Input.GetAxis("Horizontal");
            }
            else if (angle >= 0.125f * FullRot && angle < 0.375 * FullRot)
            {
                return Input.GetAxis("Vertical");
            }
            else if (angle >= 0.375 * FullRot && angle < 0.625 * FullRot)
            {
                return -Input.GetAxis("Horizontal");
            }
            else if (angle >= 0.625 * FullRot && angle < 0.875f * FullRot)
            {
                return -Input.GetAxis("Vertical");
            }
            return 0;
        }

        private float GetZAxis()
        {
            var angle = Mathf.Repeat(Mathf.Deg2Rad * _cam.transform.rotation.eulerAngles.y, FullRot);
            // -45 to 45 degrees
            if (angle >= 0.875f * FullRot || angle < 0.125f * FullRot)
            {
                return Input.GetAxis("Vertical");
            }
            else if (angle >= 0.125f * FullRot && angle < 0.375 * FullRot)
            {
                return -Input.GetAxis("Horizontal");
            }
            else if (angle >= 0.375 * FullRot && angle < 0.625 * FullRot)
            {
                return -Input.GetAxis("Vertical");
            }
            else if (angle >= 0.625 * FullRot && angle < 0.875f * FullRot)
            {
                return Input.GetAxis("Horizontal");
            }
            return 0;
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