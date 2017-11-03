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
            if (Input.GetKeyDown(KeyCode.N))
            {
                _currentBrick = Instantiate(_modBrickPrefab, Vector3.zero, Quaternion.identity);
            }
            if (_currentBrick != null)
            {
                CheckForMovement();
                if (Input.GetKeyDown(KeyCode.P))
                {

                }
            }
        }

        private void CheckForMovement()
        {
            var x = Input.GetAxis("Horizontal");
            var z = Input.GetAxis("Vertical");
            var pos = _currentBrick.gameObject.transform.position;
            pos.x = pos.x + x * Time.deltaTime * _speed;
            pos.z = pos.z + z * Time.deltaTime * _speed;

            _currentBrick.gameObject.transform.position = pos; 

        }

        private void PlaceBrick()
        {
            // snap
            _currentBrick = null;
        }


    }
}