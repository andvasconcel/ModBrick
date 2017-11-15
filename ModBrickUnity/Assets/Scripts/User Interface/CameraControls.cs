using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ModBrick.UserInterface
{
    public class CameraControls : MonoBehaviour
    {
        [SerializeField] private Collider _centerObject;
        private Vector2 _lastMousePos;
        private float _rotationSpeed = 0.01f;
        private float _zoom = 100f;
        private Vector3 _center;
        

        private const float _zoomMin = 50f;
        private const float _zoomMax = 300f;
        private float _zoomSpeed = 10f;

        void Start()
        {
            _lastMousePos = Input.mousePosition;
        }

        // Update is called once per frame
        void Update()
        {
            SetCenter();
            HandleZoom();
            HandleRotation();
            _lastMousePos = Input.mousePosition;
        }

        void SetCenter()
        {
            _center = _centerObject.bounds.center;
        }

        void HandleRotation()
        {
            var horizontal = 0f;
            if (Input.GetMouseButton(1))
            {
                Vector2 mousePosition = Input.mousePosition;
                var delta = mousePosition - _lastMousePos;
                horizontal = delta.x;
            }
            RotateCam(horizontal);
            transform.LookAt(_center);
        }

        private void HandleZoom()
        {
            var scroll = -Input.GetAxis("Mouse ScrollWheel");
            _zoom = Mathf.Clamp(_zoom + scroll * _zoomSpeed, _zoomMin, _zoomMax);
        }

        private void RotateCam(float xAmount)
        {
            var currentAngle = CurrentAngle(_center, transform.position);
            var newAngle = currentAngle + xAmount * _rotationSpeed;
            var circlePos = CirclePosition(newAngle, _center, _zoom);
            transform.position = circlePos;
        }

        private Vector3 CirclePosition(float angle, Vector3 center, float radius)
        {
            var x = Mathf.Cos(angle) * radius + center.x;
            var z = Mathf.Sin(angle) * radius + center.z;
            return new Vector3(x, radius, z);
        }

        private float CurrentAngle(Vector3 center, Vector3 position)
        {
            return Mathf.Atan2(position.z - center.z, position.x - center.x);
        }

    }
}