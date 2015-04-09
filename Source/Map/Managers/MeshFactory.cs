using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class MeshFactory {

    private static MeshFactory inst;

    public static MeshFactory Instance
    {
        get {
            if (inst == null)
                inst = new MeshFactoryImp();
            return inst;
        }
    }

    public abstract void Generate(CellProperties properties);
    public abstract Mesh getMesh();
    public abstract Texture2D getTexture2D();
    public abstract FaceNoSC[] getFaces();

    private class MeshFactoryImp : MeshFactory {

        private Mesh mesh;
        private Texture2D texture;
        private FaceNoSC[] faces;
        private string UVHash;
        private string TextureHash;

        public override void Generate(CellProperties properties)
        {
            faces = regenerateFaces(properties.faces, properties.height, properties.top, properties.width, properties.orientation);
            texture = getTextureAndGenerateUVs(faces);
            mesh =  generateMeshFromFaces(faces);
        }

        public override Mesh getMesh()
        {
            return mesh;
        }

        public override Texture2D getTexture2D()
        {
            return texture;
        }

        public override FaceNoSC[] getFaces()
        {
            return faces;
        }


        private Mesh generateMeshFromFaces(FaceNoSC[] faces)
        {

            ArrayList triangles = new ArrayList();
            ArrayList uvs = new ArrayList();
            ArrayList finalVertices = new ArrayList();

            long partialHash = 0;

            foreach (FaceNoSC f in faces)
            {
                triangles.AddRange(f.Triangles);
                foreach (int vertex in f.VertexIndex)
                    finalVertices.Add(f.SharedVertex[vertex]);
                foreach (Vector2 uv in f.Uvs)
                {
                    uvs.Add(uv);
                    partialHash += 1000000L * Mathf.RoundToInt(1000000f * uv.x) + Mathf.RoundToInt(1000000f * uv.y);
                }
               // uvs.AddRange(f.Uvs);
            }

            UVHash = partialHash + "";

            Mesh auxMesh = new Mesh();

            auxMesh.vertices = finalVertices.ToArray(typeof(Vector3)) as Vector3[];
            auxMesh.triangles = triangles.ToArray(typeof(int)) as int[];
            auxMesh.RecalculateNormals();
            auxMesh.uv = uvs.ToArray(typeof(Vector2)) as Vector2[];
            auxMesh.name = "Dynamic Cell";
            auxMesh.RecalculateBounds();

            return auxMesh;
        }

        private FaceNoSC[] regenerateFaces(FaceNoSC[] faces, float height, CellTopType cellTop, float cellWidth, int CellTopRotation)
        {

            List<FaceNoSC> tmpFaces = new List<FaceNoSC>();
            List<Vector3> finalVertexList = new List<Vector3>();

            bool hasMediumTop = height != ((float)((int)height));
            int numVert = ((int)height + 1) * 4 + ((hasMediumTop) ? 4 : 0);

            Vector3[] vertices = new Vector3[numVert + ((cellTop != CellTopType.flat) ? 2 : 0)];
            float down = - height;
            switch (cellTop) {
                case CellTopType.plane: down -= 1f; break;
                case CellTopType.midPlane: down -= 0.5f; break;
            }

            // INDEXES FOR TOP FACE
            int vertTopLeft = vertices.Length - 4; int verTopRight = vertices.Length - 3;
            int vertBotLeft = vertices.Length - 1; int vertBotRight = vertices.Length - 2;

            //MAIN VARS FOR VERTICES
            float halfWidth = (cellWidth / 2.0f);
            Vector3 vectorHeight = new Vector3(0, cellWidth, 0);

            // BASE VERTICES
            vertices[0] = new Vector3(-halfWidth, 0, -halfWidth); vertices[1] = new Vector3(halfWidth, 0, -halfWidth);
            vertices[2] = new Vector3(halfWidth, 0, halfWidth); vertices[3] = new Vector3(-halfWidth, 0, halfWidth);
            if (height >= 1) vertices[4] = vertices[0] + vectorHeight;
            else if (hasMediumTop) vertices[4] = vertices[0] + vectorHeight * 0.5f;

            FaceNoSC last = null;

            //MAIN LATERAL FACE GENERATOR
            for (int i = 4; i < numVert; i++)
            {
                int cutted = (i % 4 == 3) ? 1 : 0;
                if (i + 1 < numVert)
                    vertices[i + 1] = vertices[i - 3] + vectorHeight * ((hasMediumTop && (i + 1) >= numVert - 4) ? 0.5f : 1f);

                last = selectFaceFor(faces, tmpFaces);
                tmpFaces.Add(createFace(new int[4] { i - 4, i - 3 - (4 * cutted), i + 1 - (4 * cutted), i }, last, finalVertexList, vertices));
            }

            //EXTRA FACES GENERATOR
            if (cellTop != CellTopType.flat)
            {
                float aumHeight = (cellTop == CellTopType.midPlane) ? 0.5f : 1f;

                int topBotLeft = numVert - (4 - (CellTopRotation + 0) % 4),
                    topBotRight = numVert - (4 - (CellTopRotation + 1) % 4),
                    topTopRight = numVert - (4 - (CellTopRotation + 2) % 4),
                    topTopLeft = numVert - (4 - (CellTopRotation + 3) % 4);

                vertices[numVert] = vertices[topBotRight] + vectorHeight * aumHeight;
                vertices[numVert + 1] = vertices[topTopRight] + vectorHeight * aumHeight;

                //NEW TOP FACE
                int[] topFaceIndexes = new int[4] { topTopLeft, numVert + 1, numVert, topBotLeft };
                vertBotLeft = topFaceIndexes[CellTopRotation];
                vertTopLeft = topFaceIndexes[(3 + CellTopRotation) % 4];
                verTopRight = topFaceIndexes[(2 + CellTopRotation) % 4];
                vertBotRight = topFaceIndexes[(1 + CellTopRotation) % 4];

                // Lado Derecho
                last = selectFaceFor(faces, tmpFaces);
                tmpFaces.Add(createFace(new int[3] { numVert, topBotLeft, topBotRight }, last, finalVertexList, vertices));

                //Lado Izquierdo
                last = selectFaceFor(faces, tmpFaces);
                tmpFaces.Add(createFace(new int[3] { numVert + 1, topTopRight, topTopLeft }, last, finalVertexList, vertices));

                //Parte de atras
                last = selectFaceFor(faces, tmpFaces);
                tmpFaces.Add(createFace(new int[4] { topBotRight, topTopRight, numVert + 1, numVert }, last, finalVertexList, vertices));
            }

            //TOP FACE GENERATOR
            if (faces != null && faces.Length >= 1) last = faces[faces.Length - 1];
            tmpFaces.Add(createFace(new int[4] { vertBotLeft, vertTopLeft, verTopRight, vertBotRight }, last, finalVertexList, vertices));

            return tmpFaces.ToArray();//..ToArray(typeof(Face)) as Face[];

        }

        private FaceNoSC selectFaceFor(FaceNoSC[] faces, List<FaceNoSC> tmpFaces)
        {
            FaceNoSC selected = null;

            if (faces != null)
            {
                if (faces.Length - 1 > tmpFaces.Count)
                { selected = faces[tmpFaces.Count]; }
                else if (tmpFaces.Count - 4 >= 0)
                { selected = tmpFaces[tmpFaces.Count - 4]; }
            }

            return selected;
        }

        private FaceNoSC createFace(int[] indexes, FaceNoSC copy, List<Vector3> vertexList, Vector3[] vertices)
        {

            FaceNoSC f = new FaceNoSC();
            f.FinalVertexList = vertexList;
            f.SharedVertex = vertices;
            f.VertexIndex = indexes;
            f.regenerateTriangles();

            if (copy != null)
            {
                //			f.getAtribsFrom(copy);
                f.Texture = copy.Texture;
                f.TextureMapping = copy.TextureMapping;
            }

            return f;
        }

        private Texture2D getTextureAndGenerateUVs(FaceNoSC[] faces)
        {

            // BASE TEXTURE ATLAS
            Texture2D TextureAtlas = new Texture2D(1, 1);
            TextureAtlas.anisoLevel = 0;
            TextureAtlas.filterMode = FilterMode.Point;

            //RECOPILATING TEXTURES
            Texture2D[] AllCubeTextures = new Texture2D[faces.Length];
            for (int i = 0; i < faces.Length; i++)
                AllCubeTextures[i] = (faces[i] as FaceNoSC).Texture;

            Rect[] posTexturas = TextureAtlas.PackTextures(AllCubeTextures, 0);

            for (int i = 0; i < faces.Length; i++)
                faces[i].regenerateUVs(posTexturas[i]);

            int partialHash = 0;
            Color[] pixeles = TextureAtlas.GetPixels();
            int incr = (pixeles.Length > 1000)? (int) pixeles.Length / 1000 : 1;
            for(int i = 0; i<pixeles.Length; i = i+incr){
                partialHash += pixeles[i].GetHashCode();
            }


            TextureHash = partialHash + "";

            return TextureAtlas;
        }
    }

}
