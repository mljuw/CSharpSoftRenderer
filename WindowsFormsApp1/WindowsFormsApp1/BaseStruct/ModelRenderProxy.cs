
using WindowsFormsApp1.Shader;

namespace WindowsFormsApp1.BaseStruct
{
    internal class ModelRenderProxy
    {
        private static int IDFactory = 0;

        public MyModelData ModelData = null;

        public ShaderType TheShaderType = ShaderType.WireFrame;

        public ShaderBase Shader = null;

        public Transform ModelTransform = null;

        public MaterialParams ModelMaterialParams = null;

        public int ID = 0;

        public ModelRenderProxy(MyModelData InModelData, ShaderType InShaderType, Transform InModelTransform, MaterialParams InMaterialParams)
        {
            ID = IDFactory++;
            ModelTransform = InModelTransform;
            ModelData = InModelData;
            TheShaderType = InShaderType;
            switch (TheShaderType)
            {
                case ShaderType.WireFrame:
                    Shader = new WireFrameShader();
                    break;
                case ShaderType.BlinnPhong:
                    Shader = new BlinnPhongShader();
                    break;
                case ShaderType.PBR:
                    Shader = new PBRShader();
                    break;
            }
            ModelMaterialParams = InMaterialParams;
        }

        public Transform GetTransform()
        {
            return ModelTransform;
        }

        public MyMatrix GetMatrix()
        {
            return ModelTransform.GetMatrix();
        }
    }
}
