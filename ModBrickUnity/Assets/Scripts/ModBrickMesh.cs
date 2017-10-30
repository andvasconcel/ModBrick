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
			
			var tnw = new Vector3(-l/2f, h, -w/2f);
			var tne = new Vector3(l/2f, h, -w/2f);
			var tsw = new Vector3(-l/2f, h, w/2f);
			var tse = new Vector3(l/2f, h, w/2f);

			var bnw = new Vector3(-l/2f, 0, -w/2f);
			var bne = new Vector3(l/2f, 0, -w/2f);
			var bsw = new Vector3(-l/2f, 0, w/2f);
			var bse = new Vector3(l/2f, 0, w/2f);
			
			
			AddBottomlessBox(tnw, tne, tsw, tse,
				   bnw, bne, bsw, bse);
			InnerBox(tnw, tne, tsw, tse,
					bnw, bne, bsw, bse);

			
			_mesh.vertices = _vertices.ToArray();
			_mesh.triangles = _triangles.ToArray();
			_mesh.RecalculateNormals();
			_filter.mesh = _mesh;
			
        }

		private void InnerBox(Vector3 tnw, Vector3 tne, Vector3 tsw, Vector3 tse,
						   Vector3 bnw, Vector3 bne, Vector3 bsw, Vector3 bse)
		{
			var tnwi = new Vector3(tnw.x + ModBrickMetrics.Margin, tnw.y - ModBrickMetrics.Margin, tnw.z + ModBrickMetrics.Margin);
			var tnei = new Vector3(tne.x - ModBrickMetrics.Margin, tne.y - ModBrickMetrics.Margin, tne.z + ModBrickMetrics.Margin);
			var tswi = new Vector3(tsw.x + ModBrickMetrics.Margin, tsw.y - ModBrickMetrics.Margin, tsw.z - ModBrickMetrics.Margin);
			var tsei = new Vector3(tse.x - ModBrickMetrics.Margin, tse.y - ModBrickMetrics.Margin, tse.z - ModBrickMetrics.Margin);

			var bnwi = new Vector3(bnw.x + ModBrickMetrics.Margin, bnw.y, bnw.z + ModBrickMetrics.Margin);
			var bnei = new Vector3(bne.x - ModBrickMetrics.Margin, bne.y, bne.z + ModBrickMetrics.Margin);
			var bswi = new Vector3(bsw.x + ModBrickMetrics.Margin, bsw.y, bsw.z - ModBrickMetrics.Margin);
			var bsei = new Vector3(bse.x - ModBrickMetrics.Margin, bse.y, bse.z - ModBrickMetrics.Margin);

			AddBottomlessBox(tnwi, tnei, tswi, tsei, bnwi, bnei, bswi, bsei, true);
			AddBottomMargin(bnw, bne, bsw, bse, bnwi, bnei, bswi, bsei);
		}

		// bnwo : bot north west outer, bsei : bot south east inner
		private void AddBottomMargin(Vector3 bnwo, Vector3 bneo, Vector3 bswo, Vector3 bseo,
						   Vector3 bnwi, Vector3 bnei, Vector3 bswi, Vector3 bsei)
		{
			AddQuad(bswo, bswi, bnwi, bnwo);
			AddQuad(bnwo, bnwi, bnei, bneo);
			AddQuad(bneo, bnei, bsei, bseo);
			AddQuad(bseo, bsei, bswi, bswo);
		}

		// tnw : top north west, bse : bottom south east
		private void AddBottomlessBox(Vector3 tnw, Vector3 tne, Vector3 tsw, Vector3 tse,
						   Vector3 bnw, Vector3 bne, Vector3 bsw, Vector3 bse,
						   bool inverted = false)
		{
			//left  : tnw, tsw, bsw, bnw
			//front : tne, tnw, bnw, bne
			//right : tse, tne, bne, bse
			//back  : tsw, tse, bse, bsw
			//top   : tnw, tne, tse, tsw
			//bot   : bne, bnw, bsw, bse
			if(!inverted)
			{
				AddQuad(tnw, tsw, bsw, bnw);
				AddQuad(tne, tnw, bnw, bne);
				AddQuad(tse, tne, bne, bse);
				AddQuad(tsw, tse, bse, bsw);
				AddQuad(tnw, tne, tse, tsw);
			}
			else
			{
				AddQuad(bnw, bsw, tsw, tnw);
				AddQuad(bne, bnw, tnw, tne);
				AddQuad(bse, bne, tne, tse);
				AddQuad(bsw, bse, tse, tsw);
				AddQuad(tsw, tse, tne, tnw);
			}

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