using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WindowsFormsApp1.BaseStruct;

namespace WindowsFormsApp1
{
    internal abstract class RenderPassBase
    {

        protected MyRenderer Renderer = null;

        protected MyMatrix ProjectionMatrix = new MyMatrix();

        protected MyMatrix NDC2ScreenMatrix = new MyMatrix();

        public RenderPassBase(MyRenderer renderer)
        {
            Renderer = renderer;
        }

        public void SetPrespectiveProjection(float Fov, float Near, float Far, float Ratio)
        {
            float HalfFov = Fov / 2;

            double Radian = HalfFov * Math.PI / 180;

            float Tan = (float)Math.Tan(Radian);

            float HalfHeight = Tan * Near;

            float HalfWidth = HalfHeight * Ratio;

            ProjectionMatrix.XPlane = new MyFloat4(Far / (Far - Near), 0, 0, 1);
            ProjectionMatrix.YPlane = new MyFloat4(0, 2 * Near / (HalfWidth * 2), 0, 0);
            ProjectionMatrix.ZPlane = new MyFloat4(0, 0, 2 * Near / (HalfHeight * 2), 0);
            ProjectionMatrix.WPlane = new MyFloat4(-Far * Near / (Far - Near), 0, 0, 0);

        }

        public void SetOrthoProjection(float Near, float Far, float Width, float Ratio)
        {
            float Height = Width / Ratio;
            ProjectionMatrix.XPlane = new MyFloat4(1 / (Far - Near), 0, 0, 0);
            ProjectionMatrix.YPlane = new MyFloat4(0, 2 / Width, 0, 0);
            ProjectionMatrix.ZPlane = new MyFloat4(0, 0, 2 / Height, 0);
            ProjectionMatrix.WPlane = new MyFloat4(-Near / (Far - Near), 0, 0, 1);
        }

        public virtual void PreBeginpipeline() { }

        public virtual void PostBeginpipeline() { }

        public virtual void BeginPipeline(ModelRenderProxy Model) { }

        public virtual void OnSwitchAntiAliasing() { }

        public virtual void OnSetScreenSize()
        {
            /*
             * NDC 转换到屏幕空间
            *                                     Width       
            *     1              0           ---------------  
            * -1     1   --> 0       1  --> |               | 
            *    -1              1          |               | H
            *                               |               | 
            *                                ---------------  
            *   y = kx + b
            */
            NDC2ScreenMatrix.XPlane = new MyFloat4(1, 0, 0, 0);
            NDC2ScreenMatrix.YPlane = new MyFloat4(0, Renderer.ScreenSize.X / 2.0f, 0, 0);
            NDC2ScreenMatrix.ZPlane = new MyFloat4(0, 0, -Renderer.ScreenSize.Y / 2.0f, 0);
            NDC2ScreenMatrix.WPlane = new MyFloat4(0, Renderer.ScreenSize.X / 2.0f, Renderer.ScreenSize.Y / 2.0f, 1);
        }


        public bool CheckCVVPointInSidePlane(MyFloat4 CVVPos, ClipPlaneKind PlaneKind)
        {
            if (PlaneKind == ClipPlaneKind.W)
            {
                return CVVPos.W >= 0.001;
            }
            else if (PlaneKind == ClipPlaneKind.Top)
            {
                return CVVPos.Z <= CVVPos.W;
            }
            else if (PlaneKind == ClipPlaneKind.Bottom)
            {
                return -CVVPos.W <= CVVPos.Z;
            }
            else if (PlaneKind == ClipPlaneKind.Left)
            {
                return -CVVPos.W <= CVVPos.Y;
            }
            else if (PlaneKind == ClipPlaneKind.Right)
            {
                return CVVPos.Y <= CVVPos.W;
            }
            else if (PlaneKind == ClipPlaneKind.Far)
            {
                return CVVPos.X <= CVVPos.W;
            }
            else if (PlaneKind == ClipPlaneKind.Near)
            {
                return 0 <= CVVPos.X;
            }
            return true;
        }

        public void ClipWithPlane(ClipPlaneKind Kind, ref List<VertexOutput> VertexArray)
        {
            List<VertexOutput> NewVertexArray = new List<VertexOutput>(VertexArray.Count);

            int PreIndex = VertexArray.Count - 1;
            for (int i = 0; i < VertexArray.Count; ++i)
            {
                if (0 != i)
                {
                    PreIndex = i - 1;
                }
                VertexOutput PreVertexOutput = VertexArray[PreIndex];
                VertexOutput CurVertexOutput = VertexArray[i];
                MyFloat4 PrePos = PreVertexOutput.Pos;
                MyFloat4 CurPos = CurVertexOutput.Pos;
                bool bCurInFrustum = CheckCVVPointInSidePlane(CurPos, Kind);
                bool bPreInFrustum = CheckCVVPointInSidePlane(PrePos, Kind);
                if (bCurInFrustum != bPreInFrustum)
                {
                    float T = Utils.GetLerpAlphaWithCrossOnPlane(Kind, PrePos, CurPos);
                    /*插值*/
                    VertexOutput CrossVertex = new VertexOutput();
                    CrossVertex.Pos = MyFloat4.Lerp(PrePos, CurPos, T);
                    CrossVertex.Color = MyFloat3.Lerp(PreVertexOutput.Color, CurVertexOutput.Color, T);
                    CrossVertex.WorldPos = MyFloat4.Lerp(PreVertexOutput.WorldPos, CurVertexOutput.WorldPos, T);
                    CrossVertex.Depth = PreVertexOutput.Depth + (CurVertexOutput.Depth - PreVertexOutput.Depth) * T;
                    CrossVertex.Normal = MyFloat3.Lerp(PreVertexOutput.Normal, CurVertexOutput.Normal, T);
                    CrossVertex.Normal.Normalize();
                    CrossVertex.UV = MyFloat2.Lerp(PreVertexOutput.UV, CurVertexOutput.UV, T);

                    NewVertexArray.Add(CrossVertex);
                }

                if (bCurInFrustum)
                {
                    NewVertexArray.Add(CurVertexOutput);
                }
            }
            VertexArray = NewVertexArray;
        }
    }
}
