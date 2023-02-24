using System;
using System.Collections.Generic;
using WindowsFormsApp1.BaseStruct;

namespace WindowsFormsApp1
{
    internal class ShadowMappingPass : RenderPassBase
    {
        protected MyMatrix ViewMatrix;

        protected MyMatrix VPMatrix;

        protected LightBase DirectionalLight;

        protected Texture ShadowMap;

        protected IList<MyInt2> ShadowSampleOffsetArray = new List<MyInt2>();

        public ShadowMappingPass(MyRenderer Renderer) : base(Renderer)
        {
            //Renderer.ScreenSize
            SetOrthoProjection(1, 1440, 720, 2);
            //SetPrespectiveProjection(90, 1, 1250, 2);

            ShadowSampleOffsetArray.Add(new MyInt2(-1, -1));
            ShadowSampleOffsetArray.Add(new MyInt2(0, -1));
            ShadowSampleOffsetArray.Add(new MyInt2(1, -1));
            ShadowSampleOffsetArray.Add(new MyInt2(-1, 0));
            ShadowSampleOffsetArray.Add(new MyInt2(0, 0));
            ShadowSampleOffsetArray.Add(new MyInt2(1, 0));
            ShadowSampleOffsetArray.Add(new MyInt2(-1, 1));
            ShadowSampleOffsetArray.Add(new MyInt2(0, 1));
            ShadowSampleOffsetArray.Add(new MyInt2(1, 1));
        }


        public override void BeginPipeline(ModelRenderProxy Model)
        {
            if (null == DirectionalLight) return;
            var ModelMatrix = Model.GetMatrix();
            foreach(var Face in  Model.ModelData.Faces)
            {
                List<VertexOutput> Vertices = new List<VertexOutput>(3);
                foreach (var Vertex in Face.Vertices)
                {
                    var WorldPos = Vertex * ModelMatrix;
                    var Pos = WorldPos * VPMatrix;

                    VertexOutput VertexOutput = new VertexOutput();
                    VertexOutput.WorldPos = WorldPos;
                    VertexOutput.Pos = Pos;
                    Vertices.Add(VertexOutput);
                }

                /*裁剪*/
                ClipWithPlane(ClipPlaneKind.W, ref Vertices);
                ClipWithPlane(ClipPlaneKind.Near, ref Vertices);
                ClipWithPlane(ClipPlaneKind.Far, ref Vertices);
                ClipWithPlane(ClipPlaneKind.Left, ref Vertices);
                ClipWithPlane(ClipPlaneKind.Right, ref Vertices);
                ClipWithPlane(ClipPlaneKind.Top, ref Vertices);
                ClipWithPlane(ClipPlaneKind.Bottom, ref Vertices);

                /*图元装配*/
                int i0 = 0;
                for(int i = 0; i < Vertices.Count - 2; ++i)
                {
                    int i1 = i + 1;
                    int i2 = i + 2;

                    var V0 = Vertices[i0];
                    var V1 = Vertices[i1];
                    var V2 = Vertices[i2];

                    //背面剔除
                    if (Utils.IsBackFace(V0.Pos, V1.Pos, V2.Pos))
                        continue;

                    IList<VertexOutput> NewVertices = new List<VertexOutput>(3);
                    NewVertices.Add(V0);
                    NewVertices.Add(V1);
                    NewVertices.Add(V2);


                    //屏幕空间映射, 透视除法
                    List<VertexOutput> ScreenVertices = new List<VertexOutput>();
                    foreach(var CurVertex in NewVertices)
                    {
                        var Tmp = new VertexOutput(CurVertex);
                        Tmp.Depth = CurVertex.Pos.X;
                        Tmp.Pos /= Tmp.Pos.W;
                        Tmp.Pos = CurVertex.Pos * NDC2ScreenMatrix;
                        ScreenVertices.Add(Tmp);
                    }

                    V0 = ScreenVertices[0];
                    V1 = ScreenVertices[1];
                    V2 = ScreenVertices[2];

                    MyFloat2 SPos0 = new MyFloat2(V0.Pos.Y, V0.Pos.Z);
                    MyFloat2 SOos1 = new MyFloat2(V1.Pos.Y, V1.Pos.Z);
                    MyFloat2 SPos2 = new MyFloat2(V2.Pos.Y, V2.Pos.Z);
                    

                    /*
                     * 遍历三角形
                     */
                    MyInt2 Min = new MyInt2();
                    MyInt2 Max = new MyInt2();
                    Utils.GetTrangleAABB(V0.Pos, V1.Pos, V2.Pos, ref Min, ref Max);

                    for(int PixelY = Min.Y; PixelY < Max.Y; ++PixelY)
                    {
                        for(int PixelX = Min.X; PixelX < Max.X; ++PixelX)
                        {
                            float X = PixelX + 0.5f;
                            float Y = PixelY + 0.5f;
                            var XY = new MyFloat2(X, Y);

                            //重心坐标插值参数
                            MyFloat3 AlphaBetaGamma = Utils.GetBarycentricCoordinateParams(SPos0, SOos1, SPos2, XY);

                            if (!Utils.CheckPixelInTrangle(AlphaBetaGamma))
                                continue;

                            float CurDepth = AlphaBetaGamma.X * V0.Depth + AlphaBetaGamma.Y * V1.Depth + AlphaBetaGamma.Z * V2.Depth;
                            MyFloat4 ExistsDepth = ShadowMap.GetColor(PixelX, PixelY);
                            if (null == ExistsDepth || ExistsDepth.X > CurDepth)
                            {
                                ShadowMap.SetColor(PixelX, PixelY, new MyFloat3(CurDepth, CurDepth, CurDepth));
                             }
                        }
                    }
                }

            }

        }

        public MyFloat3 SampleShadowMap(MyFloat2 UV)
        {
            if (null == ShadowMap) return null;
            return ShadowMap.SampleWithNearest(UV).XYZ();
        }

        public override void PreBeginpipeline()
        {
            foreach (var Light in Renderer.GetShaderGlobal().Lights)
            {
                //TODO. 目前只实现平行光照
                if (Light.LightType == LightType.DirectionalLight)
                {
                    DirectionalLight = Light;
                    break;
                }
            }

            if (null == DirectionalLight) return;

            ShadowMap = new Texture(Renderer.ScreenSize);
            ViewMatrix = DirectionalLight.Transform.GetMatrix().GetInverse();

            VPMatrix = ViewMatrix* ProjectionMatrix;

            Renderer.GetShaderGlobal().ShadowMapping = this;
        }

        public float ShadowFactor(MyFloat4 WorldPos)
        {
            if (ShadowMap == null) return 1;
            var NDCPos = WorldPos * VPMatrix;
            var ScreenPos = NDCPos * NDC2ScreenMatrix;

            //阴影偏移
            float Offset = 0.002f;
            float NDCDpeth = NDCPos.X - Offset;

            float Depth = 0;
            //PCF阴影处理
            foreach(var SampleOffset in ShadowSampleOffsetArray)
            {
                int X = (int)ScreenPos.Y + SampleOffset.X;
                int Y = (int)ScreenPos.Z + SampleOffset.Y;
                var DepthColor = ShadowMap.GetColor(X, Y);
                if(DepthColor != null)
                {
                    if (DepthColor.X < NDCDpeth)
                    {
                        Depth += 1;
                    }
                }
            }
            Depth /= ShadowSampleOffsetArray.Count;
            return 1 - Depth;
        }

        public override void PostBeginpipeline()
        {
            if(Renderer.DisplayShadowMapping)
            {
                var CurBuffer = Renderer.GetFrameBuffer(0);
                var CustomData = ShadowMap.GetCurstomData();
                foreach(var Pair in CustomData)
                {
                    int PixelY = Pair.Key / ShadowMap.GetSize().X;
                    int PixelX = Pair.Key % ShadowMap.GetSize().X;
                    CurBuffer.AddColor(PixelX, PixelY, Pair.Value.XYZ());
                }
            }


        }
    }
}
