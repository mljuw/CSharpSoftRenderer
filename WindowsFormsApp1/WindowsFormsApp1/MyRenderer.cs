using System;
using System.Collections.Generic;
using System.Drawing;
using WindowsFormsApp1.BaseStruct;

namespace WindowsFormsApp1
{

    enum AntiAliasingType
    {
        None,
        SSAA,
        MSAA
    }

    internal class MyRenderer
    {

        private IList<FrameBuffer> FrameBufferArray = new List<FrameBuffer>(4);

        public MyInt2 ScreenSize = new MyInt2();

        private IList<ModelRenderProxy> RenderModels = new List<ModelRenderProxy>();

        ShaderGlobal ShaderGlobal = new ShaderGlobal();

        protected List<RenderPassBase> RenderPassArray = new List<RenderPassBase>();

        private AntiAliasingType AntiAliasing;

        private bool bAntiAliasingChangeChanged = false;

        public static List<String> SourceFindPaths = new List<string>() 
        {
            System.Environment.CurrentDirectory,
            System.Environment.CurrentDirectory + "\\..\\..\\..\\.."
        };

        public bool DisplayShadowMapping { get; set; }

        public bool DisplayNormal { get; set; }
        public bool DisplayTangent { get; set; }
        public bool DisplayBiTangent { get; set; }

        ShadowMappingPass ShadowPass;
        BaseRenderPass BasePass;

        public MyRenderer()
        {
            BasePass = new BaseRenderPass(this);
            ShadowPass = new ShadowMappingPass(this);
            DisplayNormal = false;
            RenderPassArray.Add(ShadowPass);
            RenderPassArray.Add(BasePass);
            InitFrameBuffer();

        }

        public void SetScreenSize(int Width, int Height)
        {
            ScreenSize.X = Width;
            ScreenSize.Y = Height;

            foreach (RenderPassBase Pass in RenderPassArray)
            {
                Pass.OnSetScreenSize();
            }
        }

        public void SetAntiAlising(AntiAliasingType NewType)
        {
            if (AntiAliasing == NewType) return;
            AntiAliasing = NewType;
            bAntiAliasingChangeChanged = true;
        }

        public AntiAliasingType GetAntiAliasingType()
        {
            return AntiAliasing;
        }

        public void SetPrespectiveProjection(float Fov, float Near, float Far, float Ratio)
        {
            BasePass.SetPrespectiveProjection(Fov, Near, Far, Ratio);
        }

        public List<ModelRenderProxy> AddModel(string FilePath, ShaderType InShaderType, Transform ModelTransform, MaterialParams InMatParams)
        {
            List<ModelRenderProxy> Ret = new List<ModelRenderProxy>();

            ObjMesh Mesh = null;
            foreach (var SourcePath in SourceFindPaths)
            {
                try
                {
                    Mesh = new ObjMesh();
                    Mesh.LoadFromObj(SourcePath + FilePath);
                    break;
                }
                catch (Exception ex)
                {
                    
                }
            }
            

            //var objLoaderFactory = new ObjLoaderFactory();
            //var objLoader = objLoaderFactory.Create();
            //var fileStream = new FileStream(FilePath, FileMode.Open);
            //var Result = objLoader.Load(fileStream);

            foreach (var Part in Mesh.listObjParts)
            {
                var ModelProxy = new ModelRenderProxy(new MyModelData(Part), InShaderType, ModelTransform, InMatParams);
                RenderModels.Add(ModelProxy);
                Ret.Add(ModelProxy);
            }

            return Ret;
        }
        
        public LightBase AddLight(LightBase NewLight)
        {
            ShaderGlobal.Lights.Add(NewLight);
            return NewLight;
        }

        public ShaderGlobal GetShaderGlobal()
        {
            return ShaderGlobal;
        }

        public void SetCameraTrans(MyFloat3 Pos, MyFloat3 Rot)
        {
            ShaderGlobal.CameraTrans.SetLocation(Pos);
            ShaderGlobal.CameraTrans.SetRotation(Rot);
        }

        public void DrawTangle(List<MyInt2> TranglePoints, MyFloat3 Color)
        {
            if (TranglePoints.Count < 3) return;
            DrawLine(TranglePoints[2], TranglePoints[0], Color);
            DrawLine(TranglePoints[0], TranglePoints[1], Color);
            DrawLine(TranglePoints[1], TranglePoints[2], Color);
        }

        public void DrawTangle(MyInt2 V0, MyInt2 V1, MyInt2 V2, MyFloat3 Color)
        {
           
            DrawLine(V2, V0, Color);
            DrawLine(V0, V1, Color);
            DrawLine(V1, V2, Color);
        }

        /* Bresenham 画线算法 */
        public void DrawLine(MyInt2 Start, MyInt2 End, MyFloat3 Color)
        {
            var FrameBuffer = GetFrameBuffer(0);

            MyInt2 A = new MyInt2(Start.X, Start.Y);
            MyInt2 B = new MyInt2(End.X, End.Y);

            A.X = (int)Utils.Clamp(A.X, 0, ScreenSize.X);
            A.Y = (int)Utils.Clamp(A.Y, 0, ScreenSize.Y);


            B.X = (int)Utils.Clamp(B.X, 0, ScreenSize.X);
            B.Y = (int)Utils.Clamp(B.Y, 0, ScreenSize.Y);


            int XSign = 1;
            int YSign = 1;
            if (A.X > B.X)
            {
                MyInt2 Tmp = A;
                A = B;
                B = Tmp;
            }

            if(A.Y > B.Y)
            {
                YSign = -1;
            }

            MyInt2 Diff = B - A;
            Diff.X = Math.Abs(Diff.X);
            Diff.Y = Math.Abs(Diff.Y);

            bool bKMoreThenOne = false;
            if(Diff.Y > Diff.X)
            {
                bKMoreThenOne = true;
                //交换XY
                int Tmp = Diff.X;
                Diff.X = Diff.Y;
                Diff.Y = Tmp;
            }

            int Dy2 = 2 * Diff.Y;
            int Dx2 = 2 * Diff.X;

            int E = 0;
            for (int i = 0; i < Diff.X; ++i)
            {
                if (E > Diff.Y)
                {
                    E = E - Dx2;
                    if (bKMoreThenOne)
                        A.X += XSign;
                    else
                        A.Y += YSign;
                }
                
                FrameBuffer.AddColor(A.X, A.Y, Color);

                E += Dy2;
                if (bKMoreThenOne)
                    A.Y += YSign;
                else
                    A.X += XSign;
            }
        }

        protected void ClearBuffer()
        {
            foreach(FrameBuffer Buffer in FrameBufferArray)
            {
                Buffer.ClearBuffer(ScreenSize.X, ScreenSize.Y);
            }
        }

        protected void InitFrameBuffer()
        {
            int FrameBufferCount = 1;
            switch(AntiAliasing)
            {
                case AntiAliasingType.SSAA:
                case AntiAliasingType.MSAA:
                    FrameBufferCount = 4;
                    break;
            }
            
            for(int i = FrameBufferArray.Count; i < FrameBufferCount; ++i)
            {
                FrameBufferArray.Add(new FrameBuffer());
            }
            for(int i = 0; i < FrameBufferArray.Count - FrameBufferCount; ++i)
            {
                FrameBufferArray.RemoveAt(FrameBufferArray.Count - 1);
            }
            bAntiAliasingChangeChanged = false;
            foreach (RenderPassBase Pass in RenderPassArray)
            {
                Pass.OnSwitchAntiAliasing();
            }
        }


        public FrameBuffer GetFrameBuffer(int BufferIndex) { return FrameBufferArray[BufferIndex]; }

        public int GetFrameBufferCount() { return FrameBufferArray.Count;  }

        public void BeginPipeline()
        {
            if(bAntiAliasingChangeChanged)
            {
                InitFrameBuffer();
            }
            ClearBuffer();

            foreach (RenderPassBase Pass in RenderPassArray)
            {
                Pass.PreBeginpipeline();
            }

            foreach(ModelRenderProxy Model in RenderModels)
            {
                foreach (RenderPassBase Pass in RenderPassArray)
                {
                    Pass.BeginPipeline(Model);
                }
            }

            foreach (RenderPassBase Pass in RenderPassArray)
            {
                Pass.PostBeginpipeline();
            }
        }

        public void Render(Graphics G)
        {
            //G.Clear(Color.White);
            G.Clear(Color.Black);
            //G.Clear(Color.FromArgb(100, 0, 0));

            foreach (KeyValuePair<int, MyFloat3> Item in FrameBufferArray[0].ColorBuffer)
            {
                MyInt2 Pos = FrameBufferArray[0].IndexToSize(Item.Key);
                MyFloat3 TmpColor = Item.Value;
                for(int i = 1; i < FrameBufferArray.Count; ++i)
                {
                    if(FrameBufferArray[i].ColorBuffer.ContainsKey(Item.Key))
                        TmpColor += FrameBufferArray[i].ColorBuffer[Item.Key];
                }
                TmpColor /= FrameBufferArray.Count;
                TmpColor.Clamp(0, 1);
                Color PixelColor = Color.FromArgb((int)(TmpColor.X * 255), (int)(TmpColor.Y * 255), (int)(TmpColor.Z * 255));
                G.FillRectangle(new SolidBrush(PixelColor), Pos.X, Pos.Y, 1, 1);
               
            }

            

        }

    }
}
