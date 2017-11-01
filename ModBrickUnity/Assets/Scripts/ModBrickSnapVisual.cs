using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ModBrick
{
    public class ModBrickSnapVisual : MonoBehaviour
    {
		[SerializeField] private MeshRenderer _renderer;
		[SerializeField] private MeshFilter _filter;

		public void Show()
		{
			_renderer.enabled = true;
		}

		public void Hide()
		{
			_renderer.enabled = false;
		}

		public void SetMesh(Mesh m)
		{
			_filter.mesh = m;
		}

		public void UpdatePosition(Vector3 worldPos)
		{
			gameObject.transform.position = worldPos;
		}
    }
}