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

        [SerializeField] private int _length = 1;
        [SerializeField] private int _width = 1;
        [SerializeField] private int _height = 1;

        [HideInInspector] public int Length = -1;
        [HideInInspector] public int Width = -1;
        [HideInInspector] public int Height = -1;


        private const int _cylinderSegments = 24;

        void OnValidate()
        {
            bool remesh = false;
            if(Length != _length)
            {
                Length = _length;
                remesh = true;
            }
            if(Width != _width)
            {
                Width = _width;
                remesh = true;
            }
            if(Height != _height)
            {
                Height = _height;
                remesh = true;
            }
            if(remesh)
            {
                CalculateMesh(Length, Width, Height);
            }
        }

        public Mesh GetMesh()
        {
            return _filter.mesh;
        }

        // for standard full height, input 3
        public void CalculateMesh(int length, int width, int height)
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
            
            if (height >= 3)
            {
                AddTubes(length, width);
            }

            _mesh.vertices = _vertices.ToArray();
            _mesh.triangles = _triangles.ToArray();
            _mesh.RecalculateNormals();
            _filter.mesh = _mesh;

        }

        private void AddKnobs(int length, int width, float height)
        {
            for (int x = 0; x < length; x++)
            {
                for (int z = 0; z < width; z++)
                {
                    AddKnob(x, height, z);
                }
            }
        }

        private void AddTubes(int length, int width)
        {
            int tubesX = length - 1;
            int tubesZ = width - 1;
            if(tubesX+tubesZ < 2)
            {
                return;
            }
            for (int x = 0; x < tubesX; x++)
            {
                for (int z = 0; z < tubesZ; z++)
                {
                    AddTube(x, z);
                }
            }
        }

        private void AddTube(int x, int z)
        {
            var xPos = ModBrickMetrics.Unit + x * ModBrickMetrics.Unit;
            var zPos = ModBrickMetrics.Unit + z * ModBrickMetrics.Unit;
            var c = new Vector3(xPos, 0, zPos);
            float rO = ModBrickMetrics.TubeRadiusOuter;
            float rI = ModBrickMetrics.TubeRadiusInner;
            float h = ModBrickMetrics.TubeHeight;
            for (int i = 0; i < _cylinderSegments; i++)
            {
                float rads = (i / (float)_cylinderSegments) * Mathf.PI * 2;

                var nextI = Mathf.Repeat(i + 1, _cylinderSegments);
                float nextRads = (nextI / (float)_cylinderSegments) * Mathf.PI * 2;
                // bco = bottom current outer
                var bco = new Vector3(
                                c.x + rO * Mathf.Cos(rads),
                                c.y,
                                c.z + rO * Mathf.Sin(rads));
                var bno = new Vector3(
                                c.x + rO * Mathf.Cos(nextRads),
                                c.y,
                                c.z + rO * Mathf.Sin(nextRads));
                var bci = new Vector3(
                                c.x + rI * Mathf.Cos(rads),
                                c.y,
                                c.z + rI * Mathf.Sin(rads));
                var bni = new Vector3(
                                c.x + rI * Mathf.Cos(nextRads),
                                c.y,
                                c.z + rI * Mathf.Sin(nextRads));


                var tco = new Vector3(bco.x, bco.y + h, bco.z);
                var tno = new Vector3(bno.x, bno.y + h, bno.z);
                var tci = new Vector3(bco.x, bco.y + h, bco.z);
                var tni = new Vector3(bno.x, bno.y + h, bno.z);


                AddQuad(bno, tno, tco, bco); // outer side
                AddQuad(bci, tci, tni, bni); // inner side
                AddQuad(bni, bno, bco, bci); // bottom
            }

        }

        private void AddKnob(int x, float height, int z)
        {
            var botMid = new Vector3(x * ModBrickMetrics.Unit + ModBrickMetrics.Unit / 2, height, z * ModBrickMetrics.Unit + ModBrickMetrics.Unit / 2);
            AddBottomlessCylinder(botMid, ModBrickMetrics.KnobRadius, ModBrickMetrics.KnobHeight);
        }

        // c is the center of the BOTTOM circle
        private void AddBottomlessCylinder(Vector3 c, float r, float h)
        {
            for (int i = 0; i < _cylinderSegments; i++)
            {
                float rads = (i / (float)_cylinderSegments) * Mathf.PI * 2;

                var nextI = Mathf.Repeat(i + 1, _cylinderSegments);
                float nextRads = (nextI / (float)_cylinderSegments) * Mathf.PI * 2;

                var botCurrent = new Vector3(
                                c.x + r * Mathf.Cos(rads),
                                c.y,
                                c.z + r * Mathf.Sin(rads));
                var botNext = new Vector3(
                                c.x + r * Mathf.Cos(nextRads),
                                c.y,
                                c.z + r * Mathf.Sin(nextRads));
                var topCurrent = new Vector3(botCurrent.x, botCurrent.y + h, botCurrent.z);
                var topNext = new Vector3(botNext.x, botNext.y + h, botNext.z);

                var topC = new Vector3(c.x, c.y + h, c.z);

                AddQuad(botNext, topNext, topCurrent, botCurrent); // side
                AddTriangle(topC, topCurrent, topNext);
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
            if (!inverted)
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
            _triangles.Add(vertexIndex + 2);
            _triangles.Add(vertexIndex + 1);
        }
    }
}