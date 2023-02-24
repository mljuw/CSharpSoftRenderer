using System.Collections.Generic;

namespace WindowsFormsApp1.BaseStruct
{
    internal class MyModelData
    {
        public IList<TrangleFace> Faces = new List<TrangleFace>();
        public ObjPart LoadData;


        private void CalcTangent()
        {
            if (Faces.Count <= 0) return;
            var LastFace = Faces[Faces.Count - 1];
            var V0 = LastFace.Vertices[0];
            var V1 = LastFace.Vertices[1];
            var V2 = LastFace.Vertices[2];
            var UV0 = LastFace.UVs[0];
            var UV1 = LastFace.UVs[1];
            var UV2 = LastFace.UVs[2];

            /*计算切线与副法线*/
            Utils.CalcTangentAndBiTangent(V0, V1, V2, UV0, UV1, UV2, LastFace.Normal[0] ,ref LastFace.Tangent, ref LastFace.BiTangent);

        }

        public MyModelData(ObjPart InLoadData)
        {
            LoadData = InLoadData;

            for (int i = 0;i < LoadData.listTriangle.Count; ++i)
            {
                int Index = LoadData.listTriangle[i];
                if (i % 3 == 0)
                {
                    CalcTangent();
                    TrangleFace Face = new TrangleFace();
                    Faces.Add(Face);
                }
                Faces[Faces.Count - 1].Indices.Add(Index);
                Faces[Faces.Count - 1].Vertices.Add(LoadData.listVertex[Index]);
                Faces[Faces.Count - 1].Normal.Add(LoadData.listNormal[Index]);
                MyFloat2 UV = LoadData.listUV[Index];
                UV.Y = 1.0f - UV.Y;
                Faces[Faces.Count - 1].UVs.Add(UV);

            }
            CalcTangent();
        }

        //public MyFloat3 GetVertexByIndex(int Index)
        //{
        //    return LoadData.listVertex[Index];
        //}

        //public MyFloat2 GetUVByIndex(int Index)
        //{
        //    MyFloat2 Ret =  LoadData.listUV[Index];
        //    return Ret;
        //}

        //public MyFloat3 GetNormalByIndex(int Index)
        //{
        //    return LoadData.listNormal[Index];
        //}
    }
}
