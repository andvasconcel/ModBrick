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
        private ModBrickInstance _currentBrick;
        [SerializeField] private Material _brickMaterial;
        [SerializeField] private Material _selectedMaterial;

        private void SwitchBrick(ModBrickInstance newBrick)
        {
            if(_currentBrick != null)
            {
                _currentBrick.SetMaterial(_brickMaterial);
            }
            _currentBrick = newBrick;
            _currentBrick.SetMaterial(_selectedMaterial);
        }

        private void RotateCamera(Vector2 mousePosition)
        {
            
        }


        void Update()
        {
            if(Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit)) 
                {
                    ModBrickInstance brick = hit.transform.gameObject.GetComponent<ModBrickInstance>();
                    if(brick != null && brick.Selectable)
                    {
                        SwitchBrick(brick);
                    }
                }
            }
            else if(Input.GetMouseButton(1))
            {
                RotateCamera(Input.mousePosition);
            }


            if (Input.GetKeyDown(KeyCode.N) && (_currentBrick == null || _currentBrick.Placed))
            {
                _currentBrick = Instantiate(_modBrickPrefab, new Vector3(0, ModBrickMetrics.FullHeight, 0), Quaternion.identity);
                _currentBrick.SetMaterial(_selectedMaterial);
            }
            if (_currentBrick != null)
            {
                if(!_currentBrick.Placed)
                {
                    DoTranslate();
				    DoRotate();
                }
                if (Input.GetKeyDown(KeyCode.P))
                {
					PlaceBrick();
                }
                if(Input.GetKeyDown(KeyCode.F))
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
			if(Input.GetKeyDown(KeyCode.R))
			{
				_currentBrick.Rotate();
			}
		}

        private void PlaceBrick()
        {
            if(_currentBrick.Place())
            {
                _currentBrick.SetMaterial(_brickMaterial);
                _currentBrick = null;
            }
        }

        private void FreeBrick()
        {
            if(_currentBrick.Free())
            {
                // ?
            }
        }


    }
}