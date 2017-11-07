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

        void Start()
        {

        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.N) && _currentBrick == null)
            {
                _currentBrick = Instantiate(_modBrickPrefab, new Vector3(0, ModBrickMetrics.FullHeight, 0), Quaternion.identity);
            }
            if (_currentBrick != null)
            {
                DoTranslate();
				DoRotate();
                if (Input.GetKeyDown(KeyCode.P))
                {
					PlaceBrick();
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
				_currentBrick.gameObject.transform.Rotate(0, 90, 0);
			}
		}

        private void PlaceBrick()
        {
            if(_currentBrick.Place())
            {
                _currentBrick = null;
            }
        }


    }
}