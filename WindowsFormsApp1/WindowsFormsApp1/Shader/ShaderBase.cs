using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{

    enum ShaderType
    {
        WireFrame,
        BlinnPhong,
        PBR,
    }

    internal class ShaderGlobal
    {
        public IList<LightBase> Lights = new List<LightBase>();

        public MyFloat3 AmbientColor = new MyFloat3(1, 1, 1);

        public Transform CameraTrans = new Transform();

        public ShadowMappingPass ShadowMapping = null;
    }

    internal class VertexOutput
    {
        public MyFloat3 Color = new MyFloat3();
        public MyFloat4 Pos = new MyFloat4();
        public MyFloat4 WorldPos = new MyFloat4();
        public MyFloat2 UV = new MyFloat2();
        public MyFloat3 Normal = new MyFloat3();
        public float Depth = 0.0f;

        public VertexOutput() { }

        public VertexOutput(VertexOutput VertexOutput)
        {
            this.Color = new MyFloat3(VertexOutput.Color);
            this.Pos = new MyFloat4(VertexOutput.Pos);
            this.WorldPos = new MyFloat4(VertexOutput.WorldPos);
            this.UV = new MyFloat2(VertexOutput.UV);
            this.Normal = new MyFloat3(VertexOutput.Normal);
            this.Depth = VertexOutput.Depth;
        }
    }

    internal abstract class ShaderBase
    {
        public virtual VertexOutput VertexShader(MyFloat3 Vertex, MyMatrix ModelMatrix, MyMatrix ViewMatrix, MyMatrix ProjectionMatrix)
        {
            VertexOutput Ret = new VertexOutput();
            Ret.WorldPos = Vertex * ModelMatrix;
            MyMatrix VP = ViewMatrix * ProjectionMatrix;
            Ret.Pos = Ret.WorldPos * VP;
            return Ret;
        }

        public virtual MyFloat3 FragementShader(MyFloat4 WorldPos, MyFloat2 UV, MyFloat3 Normal, MyMatrix BTNMatrix, ShaderGlobal Global, MaterialParams MatParams)
        {
            MyFloat3 Ret = new MyFloat3();
            return Ret;
        }

        virtual public ShaderType GetShaderType() { return ShaderType.WireFrame;  }


    }
}
