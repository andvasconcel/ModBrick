using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ModBrick
{
    public class ModBrickMesh : MonoBehaviour
    {
        private List<int> _triangles;
        private List<Vector3> _vertices;
		private Mesh _mesh;
		[SerializeField] private MeshFilter _filter;

		void Start()
		{
			Init(1,1,3);
		}

        // for standard full height, input 3
        public void Init(int length, int width, int height)
        {
            _triangles = new List<int>();
            _vertices = new List<Vector3>();
			_mesh = new Mesh();

			float h = height * ModBrickMetrics.ThirdHeight;
			float w = width * ModBrickMetrics.Unit;
			float l = length * ModBrickMetrics.Unit;
			
			var topNorthWest = new Vector3(-l/2f, h, -w/2f);
			var topNorthEast = new Vector3(l/2f, h, -w/2f);
			var topSouthWest = new Vector3(-l/2f, h, w/2f);
			var topSouthEast = new Vector3(l/2f, h, w/2f);

			var botNorthWest = new Vector3(-l/2f, 0, -w/2f);
			var botNorthEast = new Vector3(l/2f, 0, -w/2f);
			var botSouthWest = new Vector3(-l/2f, 0, w/2f);
			var botSouthEast = new Vector3(l/2f, 0, w/2f);
			
			AddBoxWithoutBottom(topNorthWest, topNorthEast, topSouthWest, topSouthEast,
				   botNorthWest, botNorthEast, botSouthWest, botSouthEast);
			
			_mesh.vertices = _vertices.ToArray();
			_mesh.triangles = _triangles.ToArray();
			_filter.mesh = _mesh;
        }

		// tnw means top north west, bse means bottom south east...
		private void AddBoxWithoutBottom(Vector3 tnw, Vector3 tne, Vector3 tsw, Vector3 tse,
						   Vector3 bnw, Vector3 bne, Vector3 bsw, Vector3 bse)
		{
			//left  : tnw, tsw, bsw, bnw
			//front : tne, tnw, bnw, bne
			//right : tse, tne, bne, bse
			//back  : tsw, tse, bse, bsw
			//top   : tnw, tne, tse, tsw
			//bot   : bne, bnw, bsw, bse

			AddQuad(tnw, tsw, bsw, bnw);
			AddQuad(tne, tnw, bnw, bne);
			AddQuad(tse, tne, bne, bse);
			AddQuad(tsw, tse, bse, bsw);
			AddQuad(tnw, tne, tse, tsw);

		}

        private void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
        {
            int vertexIndex = _vertices.Count;
            _vertices.Add(v1);
            _vertices.Add(v2);
            _vertices.Add(v3);
            _vertices.Add(v4);

			_triangles.Add(vertexIndex);
            _triangles.Add(vertexIndex + 3);
            _triangles.Add(vertexIndex + 2);
            _triangles.Add(vertexIndex + 2);
            _triangles.Add(vertexIndex + 1);
            _triangles.Add(vertexIndex);
        }
    }
}