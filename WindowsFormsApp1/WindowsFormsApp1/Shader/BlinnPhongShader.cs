using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace WindowsFormsApp1.Shader
{
    internal class BlinnPhongShader : ShaderBase
    {
        public override MyFloat3 FragementShader(MyFloat4 WorldPos, MyFloat2 UV, MyFloat3 Normal, MyMatrix BTNMatrix, ShaderGlobal Global, MaterialParams MatParams)
        {
            //I = Ia*Ka + Ip*Kd(L dot N) + Ip*Ks(H dot N )
            //H = Normal(V + L)

            MyFloat3 Kd = new MyFloat3();
            MyFloat3 Ks = new MyFloat3();

            MyFloat3 Diffuse = MatParams.GetDiffuse(UV);
            MyFloat3 Specular = MatParams.GetSpecular(UV);
            MyFloat3 TextureNormal = MatParams.GetNormal(UV, Normal);
            Normal = BTNMatrix.TransportDirection(TextureNormal).GetNormalize();

            MyFloat3 V = (Global.CameraTrans.GetLoc() - WorldPos).GetNormalize();
            foreach (var Light in Global.Lights)
            {
                if(!Light.InRange(WorldPos))
                {
                    continue;
                }
                MyFloat3 LightDir = Light.GetLightDir(WorldPos);
                MyFloat3 H = (V + LightDir).GetNormalize();
                float Factor = Light.GetAttenuationFactor(WorldPos);
                float SqrtFactor = Factor * Factor;
                Kd += Light.GetColor() * SqrtFactor * Diffuse * Math.Max(MyFloat3.DotProduct(LightDir, Normal), 0);
                Ks += Light.GetColor() * SqrtFactor * Specular * Math.Max(MyFloat3.DotProduct(H, Normal), 0);
            }

            float ShadowFactor = 1;
            if (Global.ShadowMapping != null)
            {
                ShadowFactor = Global.ShadowMapping.ShadowFactor(WorldPos);
            }

            MyFloat3 Ret = Global.AmbientColor * MatParams.Ka + (Kd + Ks) * ShadowFactor;
            Ret.Clamp(0, 1);

            return Ret;

            //return Global.AmbientColor;
        }

        public override ShaderType GetShaderType()
        {
            return ShaderType.BlinnPhong;
        }
    }
}
