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
			Init(2,2,3);
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
			
			var tnw = new Vector3(0, h, 0);
			var tne = new Vector3(l, h, 0);
			var tsw = new Vector3(0, h, w);
			var tse = new Vector3(l, h, w);

			var bnw = new Vector3(0, 0, 0);
			var bne = new Vector3(l, 0, 0);
			var bsw = new Vector3(0, 0, w);
			var bse = new Vector3(l, 0, w);
			
			
			AddBottomlessBox(tnw, tne, tsw, tse,
				   bnw, bne, bsw, bse);
			InnerBox(tnw, tne, tsw, tse,
					bnw, bne, bsw, bse);
			AddKnobs(length, width, h);


			
			_mesh.vertices = _vertices.ToArray();
			_mesh.triangles = _triangles.ToArray();
			_mesh.RecalculateNormals();
			_filter.mesh = _mesh;
			
        }

		private void AddKnobs(int length, int width, float height)
		{
			for( int x = 0; x < length; x++)
			{
				for( int z = 0; z < width; z++)
				{
					AddKnob(x, height, z);
				}
			}
		}

		private void AddKnob(int x, float height, int z)
		{
			var botMid = new Vector3(x*ModBrickMetrics.Unit + ModBrickMetrics.Unit/2, height, z*ModBrickMetrics.Unit + ModBrickMetrics.Unit/2);
			var segments = 24;
			for(int i = 0; i < segments; i++)
			{
				var r = ModBrickMetrics.KnobRadius;
				float rads = (i/(float)segments)*Mathf.PI*2;

				var nextI = Mathf.Repeat(i+1, segments);
				float nextRads = (nextI/(float)segments)*Mathf.PI*2;

				var botCurrent = new Vector3(
								botMid.x + r * Mathf.Cos(rads),
								botMid.y,
								botMid.z + r * Mathf.Sin(rads));
				var botNext = new Vector3(
								botMid.x + r * Mathf.Cos(nextRads),
								botMid.y,
								botMid.z + r * Mathf.Sin(nextRads));
				var topCurrent = new Vector3(botCurrent.x, botCurrent.y + ModBrickMetrics.KnobHeight, botCurrent.z);
				var topNext = new Vector3(botNext.x, botNext.y + ModBrickMetrics.KnobHeight, botNext.z);

				var topMid = new Vector3(botMid.x, height + ModBrickMetrics.KnobHeight, botMid.z);

				AddQuad(botNext, topNext, topCurrent, botCurrent); // side
				AddTriangle(topMid, topCurrent, topNext);
			}
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

		private void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
		{
			int vertexIndex = _vertices.Count;
			_vertices.Add(v1);
			_vertices.Add(v2);
			_vertices.Add(v3);

			_triangles.Add(vertexIndex);
			_triangles.Add(vertexIndex+2);
			_triangles.Add(vertexIndex+1);
		}
    }
}