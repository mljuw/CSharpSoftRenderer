using System.Collections.Generic;
using WindowsFormsApp1.BaseStruct;

namespace WindowsFormsApp1
{

    enum ClipPlaneKind
    {
        W, Top, Bottom, Left, Right, Far, Near
    }

    internal class BaseRenderPass : RenderPassBase
    {
        MyMatrix ViewMatrix = null;
        MyMatrix VPMatrix = null;

        public MyFloat3 WireFrameColor = new MyFloat3(0, 1.0f, 0);

        protected IList<MyFloat2> AAPixelOffset = new List<MyFloat2>(4);

        private float DrawNormalLen = 2.0f;

        public BaseRenderPass(MyRenderer Renderer) : base(Renderer)
        {
            AAPixelOffset.Add(new MyFloat2(0.5f, 0.5f));
        }

        public override void PreBeginpipeline()
        {
            ViewMatrix = Renderer.GetShaderGlobal().CameraTrans.GetMatrix().GetInverse();
            VPMatrix = ViewMatrix * ProjectionMatrix;
            base.PreBeginpipeline();
        }


        public override void BeginPipeline(ModelRenderProxy Model)
        {
            if (Renderer.DisplayShadowMapping) return;
            
            base.BeginPipeline(Model);
            MyMatrix ModelMatrix = Model.GetMatrix();
            MyMatrix Transposed = ModelMatrix.GetTransposed();
            MyMatrix ModelTransposeInverseMatrix = Transposed.GetInverse();

            //MyFloat3 Test = ModelTransposeInverseMatrix.TransportDirection(new MyFloat3(0, 1, 1).GetNormalize());

            int[] NewVertexIndexArray = new int[3];

            foreach (TrangleFace Face in Model.ModelData.Faces)
            {
                List<VertexOutput> Vertices = new List<VertexOutput>();
                for (int i = 0; i < Face.Vertices.Count; ++i)
                {
                    MyFloat3 Vertex = Face.Vertices[i];

                    //
                    var PSOutput = Model.Shader.VertexShader(Vertex, ModelMatrix, ViewMatrix, ProjectionMatrix);
                    PSOutput.UV = Face.UVs[i];
                    var Normal = Face.Normal[i];
                    Normal.Normalize();
                    PSOutput.Normal = ModelTransposeInverseMatrix.TransportDirection(Normal).GetNormalize();
                    PSOutput.Normal.Normalize();
                    Vertices.Add(PSOutput);
                }

                /*
                * 三角形裁剪
                * 1.a = 确定当前顶点是否在Cvv视锥体内
                * 2.b = 上一个顶点是否在视锥体内
                * 3.a == b 时 没有裁剪点，否则找到相交点并加入临时数组
                * 4.如果 a 为 true 加入临时数组
                */
                ClipWithPlane(ClipPlaneKind.W, ref Vertices);
                ClipWithPlane(ClipPlaneKind.Far, ref Vertices);
                ClipWithPlane(ClipPlaneKind.Near, ref Vertices);
                ClipWithPlane(ClipPlaneKind.Left, ref Vertices);
                ClipWithPlane(ClipPlaneKind.Right, ref Vertices);
                ClipWithPlane(ClipPlaneKind.Top, ref Vertices);
                ClipWithPlane(ClipPlaneKind.Bottom, ref Vertices);


                /*
                 * 图元装配/三角形装配
                 */
                for (int i = 0; i < Vertices.Count - 2; ++i)
                {
                    NewVertexIndexArray[1] = i + 1;
                    NewVertexIndexArray[2] = i + 2;

                    VertexOutput V0 = Vertices[NewVertexIndexArray[0]];
                    VertexOutput V1 = Vertices[NewVertexIndexArray[1]];
                    VertexOutput V2 = Vertices[NewVertexIndexArray[2]];

                    //背面剔除
                    if (Utils.IsBackFace(V0.Pos, V1.Pos, V2.Pos))
                    {
                        continue;
                    }

                    MyFloat3 WorldTangent = ModelMatrix.TransportDirection(Face.Tangent);
                    MyFloat3 WorldBiTangent = ModelMatrix.TransportDirection(Face.BiTangent);

                    List<VertexOutput> SSVertices = new List<VertexOutput>();
                    foreach (int Index in NewVertexIndexArray)
                    {

                        VertexOutput CurVertex = new VertexOutput(Vertices[Index]);

                        /*
                        * 透视除法
                        */
                        CurVertex.Depth = CurVertex.Pos.X;
                        CurVertex.Pos = CurVertex.Pos / CurVertex.Pos.W;

                        ///*
                        // * 屏幕空间映射
                        // *                                     Width       
                        // *     1              0           ---------------  
                        // * -1     1   --> 0       1  --> |               | 
                        // *    -1              1          |               | H
                        // *                               |               | 
                        // *                                ---------------  
                        // *   y = kx + b
                        // */
                        CurVertex.Pos = CurVertex.Pos * NDC2ScreenMatrix;

                        SSVertices.Add(CurVertex);

                        var StartPoint = new MyInt2((int)CurVertex.Pos.Y, (int)CurVertex.Pos.Z);
                        /*
                         * 画法线, 切线、副法线
                         */
                        if (Renderer.DisplayNormal)
                        {
                            var NormalEnd = CurVertex.WorldPos + CurVertex.Normal * DrawNormalLen;
                            NormalEnd.W = 1;
                            Renderer.DrawLine(StartPoint, ConvertToScreenCoordianPoint(NormalEnd, VPMatrix), new MyFloat3(0, 1, 0));
                        }
                        if (Renderer.DisplayTangent)
                        {
                            var TangentEnd = CurVertex.WorldPos + WorldTangent * DrawNormalLen;
                            TangentEnd.W = 1;
                            Renderer.DrawLine(StartPoint, ConvertToScreenCoordianPoint(TangentEnd, VPMatrix), new MyFloat3(1, 0, 0));
                        }
                        if(Renderer.DisplayBiTangent)
                        { 
                            var BiTangentEnd = CurVertex.WorldPos + WorldBiTangent * DrawNormalLen;
                            BiTangentEnd.W = 1;
                            Renderer.DrawLine(StartPoint, ConvertToScreenCoordianPoint(BiTangentEnd, VPMatrix), new MyFloat3(0, 0, 1));
                        }
                    }

                    Rasterization(Model, ModelMatrix, WorldTangent, WorldBiTangent, SSVertices[0], SSVertices[1], SSVertices[2]);
                }
                
            }
        }

        protected MyInt2 ConvertToScreenCoordianPoint(MyFloat4 WorldPos, MyMatrix VPMatrix)
        {
            MyInt2 Ret = new MyInt2();
            //投影到CVV
            WorldPos = WorldPos * VPMatrix;
            //透视除法
            WorldPos = WorldPos / WorldPos.W;
            //屏幕空间映射
            WorldPos = WorldPos * NDC2ScreenMatrix;
            Ret.X = (int)WorldPos.Y;
            Ret.Y = (int)WorldPos.Z;
            return Ret;
        }

        public virtual void Rasterization(ModelRenderProxy Model, MyMatrix ModelMatrix, MyFloat3 WorldTangent, MyFloat3 WorldBiTangent, VertexOutput V0, VertexOutput V1, VertexOutput V2)
        {
            /*
            *画线框模式
            */
            if (Model.Shader.GetShaderType() == ShaderType.WireFrame)
            {
                Renderer.DrawTangle(new MyInt2((int)V0.Pos.Y, (int)(V0.Pos.Z)), new MyInt2((int)V1.Pos.Y, (int)(V1.Pos.Z)), new MyInt2((int)V2.Pos.Y, (int)(V2.Pos.Z)), WireFrameColor);
                return;
            }

            /*
             * 获取三角形包围盒
             */
            MyInt2 Min = new MyInt2();
            MyInt2 Max = new MyInt2();
            Utils.GetTrangleAABB(V0.Pos, V1.Pos, V2.Pos, ref Min, ref Max);

            MyFloat2 V0Pos = new MyFloat2(V0.Pos.Y, V0.Pos.Z);
            MyFloat2 V1Pos = new MyFloat2(V1.Pos.Y, V1.Pos.Z);
            MyFloat2 V2Pos = new MyFloat2(V2.Pos.Y, V2.Pos.Z);

            MyFloat3 BarycentricCoordinate = new MyFloat3();
            /*
             * 遍历三角形找到所影响的像素
             */
            for (int PixelY = Min.Y; PixelY < Max.Y; ++PixelY)
            {
                for (int PixelX = Min.X; PixelX < Max.X; ++PixelX)
                {
                    IList<int> MSAAPassInfo = new List<int>();
                    for (int i = 0; i < AAPixelOffset.Count; ++i)
                    {
                        FrameBuffer CurFrameBuffer = Renderer.GetFrameBuffer(i);
                        MyFloat2 PixelOffset = AAPixelOffset[i];
                        float Y = PixelY + PixelOffset.X;
                        float X = PixelX + PixelOffset.Y;
                        MyFloat2 XY = new MyFloat2(X, Y);
                        //重心坐标插值参数
                        MyFloat3 AlphaBetaGamma = Utils.GetBarycentricCoordinateParams(V0Pos, V1Pos, V2Pos, XY);

                        if (!Utils.CheckPixelInTrangle(AlphaBetaGamma))
                            continue;

                        //透视矫正Xt
                        float Xt = Utils.CalcFluoroscopicCorrectionXt(AlphaBetaGamma, V0.Depth, V1.Depth, V2.Depth);

                        /*插值参数预计算*/
                        float AlphaP = AlphaBetaGamma.X * Xt / V0.Depth;
                        float BetaP = AlphaBetaGamma.Y * Xt / V1.Depth;
                        float GammaP = AlphaBetaGamma.Z * Xt / V2.Depth;

                        float CurDepth = V0.Depth * AlphaP + V1.Depth * BetaP + V2.Depth * GammaP;
                        //提前深度测试
                        float Depth = CurFrameBuffer.GetDepth(PixelX, PixelY);
                        if (Depth < CurDepth && Depth >= 0)
                            continue;

                        CurFrameBuffer.UpdateDepth(PixelX, PixelY, CurDepth);

                        if (Renderer.GetAntiAliasingType() == AntiAliasingType.MSAA)
                        {
                            MSAAPassInfo.Add(i);
                            continue;
                        }
                        BarycentricCoordinate.X = AlphaP;
                        BarycentricCoordinate.Y = BetaP;
                        BarycentricCoordinate.Z = GammaP;

                        MyFloat3 Color = PixelPass(Model, WorldTangent, WorldBiTangent, BarycentricCoordinate, V0, V1, V2);
                        CurFrameBuffer.AddColor(PixelX, PixelY, Color);
                    }

                    if(MSAAPassInfo.Count > 0 && Renderer.GetAntiAliasingType() == AntiAliasingType.MSAA)
                    {
                        float Y = PixelY + 0.5f;
                        float X = PixelX + 0.5f;
                        MyFloat2 XY = new MyFloat2(X, Y);
                        //重心坐标插值参数
                        MyFloat3 AlphaBetaGamma = Utils.GetBarycentricCoordinateParams(V0Pos, V1Pos, V2Pos, XY);
                        //透视矫正Xt
                        float Xt = Utils.CalcFluoroscopicCorrectionXt(AlphaBetaGamma, V0.Depth, V1.Depth, V2.Depth);

                        /*插值参数预计算*/
                        float AlphaP = AlphaBetaGamma.X * Xt / V0.Depth;
                        float BetaP = AlphaBetaGamma.Y * Xt / V1.Depth;
                        float GammaP = AlphaBetaGamma.Z * Xt / V2.Depth;
                        BarycentricCoordinate.X = AlphaP;
                        BarycentricCoordinate.Y = BetaP;
                        BarycentricCoordinate.Z = GammaP;

                        MyFloat3 Color = PixelPass(Model, WorldTangent, WorldBiTangent, BarycentricCoordinate, V0, V1, V2);
                        foreach (int Index in MSAAPassInfo)
                        {
                            FrameBuffer FrameBuffer = Renderer.GetFrameBuffer(Index);
                            FrameBuffer.AddColor(PixelX, PixelY, Color);
                        }

                    }
                }
            }
        }

        protected MyFloat3 PixelPass(ModelRenderProxy Model, MyFloat3 WorldTangent, MyFloat3 WorldBiTangent, MyFloat3 BarycentricCoordinate, VertexOutput V0, VertexOutput V1, VertexOutput V2)
        {

            float AlphaP = BarycentricCoordinate.X;
            float BetaP = BarycentricCoordinate.Y;
            float GammaP = BarycentricCoordinate.Z;

            /*
            * 插值属性
            */
            MyFloat4 PixelWorldPos = V0.WorldPos * AlphaP + V1.WorldPos * BetaP + V2.WorldPos * GammaP;
            MyFloat2 PixelUV = V0.UV * AlphaP + V1.UV * BetaP + V2.UV * GammaP;
            MyFloat3 Normal = V0.Normal * AlphaP + V1.Normal * BetaP + V2.Normal * GammaP;

            /*
            * BTN矩阵
            */

            MyMatrix BTNMatrix = new MyMatrix();
            BTNMatrix.XPlane.X = WorldTangent.X;
            BTNMatrix.XPlane.Y = WorldTangent.Y;
            BTNMatrix.XPlane.Z = WorldTangent.Z;

            BTNMatrix.YPlane.X = WorldBiTangent.X;
            BTNMatrix.YPlane.Y = WorldBiTangent.Y;
            BTNMatrix.YPlane.Z = WorldBiTangent.Z;

            BTNMatrix.ZPlane.X = Normal.X;
            BTNMatrix.ZPlane.Y = Normal.Y;
            BTNMatrix.ZPlane.Z = Normal.Z;
            return Model.Shader.FragementShader(PixelWorldPos, PixelUV, Normal, BTNMatrix, Renderer.GetShaderGlobal(), Model.ModelMaterialParams);
            //return Normal;
        }

        public override void OnSwitchAntiAliasing()
        {
            AAPixelOffset.Clear();
            switch (Renderer.GetAntiAliasingType())
            {
                case AntiAliasingType.None:
                    AAPixelOffset.Add(new MyFloat2(0.5f, 0.5f));
                    break;
                case AntiAliasingType.SSAA:
                case AntiAliasingType.MSAA:
                    AAPixelOffset.Add(new MyFloat2(0.25f, 0.25f));
                    AAPixelOffset.Add(new MyFloat2(0.75f, 0.25f));
                    AAPixelOffset.Add(new MyFloat2(0.25f, 0.75f));
                    AAPixelOffset.Add(new MyFloat2(0.75f, 0.75f));
                    break;
            }
        }

    }
}
