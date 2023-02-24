using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WindowsFormsApp1.Shader
{
    internal class PBRShader : ShaderBase
    {
        protected MyFloat3 MinMetalic = new MyFloat3(0.04f, 0.04f, 0.04f);
        public PBRShader()
        {
            
        }

        public override ShaderType GetShaderType()
        {
            return ShaderType.PBR;
        }

        public override MyFloat3 FragementShader(MyFloat4 WorldPos, MyFloat2 UV, MyFloat3 Normal, MyMatrix BTNMatrix, ShaderGlobal Global, MaterialParams MatParams)
        {
            /*
             * Lo(X, Wo) = Le(X, Wo) + f[ Li(X, Wi) * BRDF(X, Wi -> Wo) * (N dot L) * dWi ]
             * BRDF = kd * f_diffuse() + ks * f_specular() ; ks + kd = 1; kd = (1 - ks) * (1 - Metalic)
             * f_diffuse = BaseColor / PI
             * f_specular = (D * G * F) / ( 4(V dot H) * (L dot N) ); ks = F
             * k = Alpha / 2; Alpha = Roughness^2
             * F = F0 + (1 - F0) * (1-(V dot H))^5 ; F0 = lerp((0.04, 0.04, 0.04), BaseColor, Metalic)
             * G = Schlich_Beckmenn(N, L) * Schlich_Beckmenn(N, V) ; Schlich_Beckmenn(N, X) = (N dot X) / ( (N dot X) * (1 - k) + k
             * D = Alpha^2 / (PI * ( (N dot H)^2 * (Alpha^2 - 1) + 1)^2
             */

            MyFloat3 TextureNormal = MatParams.GetNormal(UV, Normal);
            Normal = BTNMatrix.TransportDirection(TextureNormal).GetNormalize();

            MyFloat3 V = (Global.CameraTrans.GetLoc() - WorldPos).GetNormalize();

            if (MyFloat3.DotProduct(V, Normal) <= 0) return new MyFloat3();

            MyFloat3 BaseColor = MatParams.PBRParams.GetBaseColor(UV);
            MyFloat3 Emissive = MatParams.PBRParams.GetEmissive(UV);
            float Shiness = MatParams.PBRParams.Shiness;

            float AO = MatParams.PBRParams.GetAO(UV).X;
            float Metalic = MatParams.PBRParams.GetMetalicr(UV).X;
            float Roughness = MatParams.PBRParams.GetRoughness(UV).X;

            if (MatParams.PBRParams.IsCombinedTexture())
            {
                MyFloat4 CombinedColor = MatParams.PBRParams.SampleWithCombinedTexture(UV, TextureSampleType.BiLinear);
                Roughness = CombinedColor.X;
                Metalic = CombinedColor.Y;
                Shiness = CombinedColor.Z;

                //Roughness = 0.4f;
                //Metalic = 1;
                //Shiness = 4;
            }


            MyFloat3 Lo = new MyFloat3();
            foreach(LightBase Light in Global.Lights)
            {

                MyFloat3 L = Light.GetLightDir(WorldPos);
                float LightAttenuationFactor = (float)Math.Pow(Light.GetAttenuationFactor(WorldPos), 2);
                float NdL = Math.Max(MyFloat3.DotProduct(Normal, L), 0);
                if (NdL > 0)
                {
                    var BRD = BRDF(BaseColor, Global, MatParams, WorldPos, Normal, V, L, Light, Metalic, Roughness, LightAttenuationFactor, Shiness);
                    Lo += (BRD * NdL);
                }
            }
            
            MyFloat3 Ret = Emissive + Global.AmbientColor * BaseColor * AO + Lo;
            return Ret;
        }

        public virtual MyFloat3 BRDF(MyFloat3 BaseColor, ShaderGlobal Global, MaterialParams MatParams, MyFloat4 WorldPos, MyFloat3 Normal, MyFloat3 V, MyFloat3 L, LightBase Light, float Metalic, float Roughness, float LightAttenuationFactor, float Shiness)
        {
            /*准备数据*/
            
            MyFloat3 H = (L + V).GetNormalize();
            float NdL = Math.Max(MyFloat3.DotProduct(Normal, L), 0);
            float NdV = Math.Max(MyFloat3.DotProduct(Normal, V), 0);
            float NdH = Math.Max(MyFloat3.DotProduct(Normal, H), 0);
            float VdH = Math.Max(MyFloat3.DotProduct(V, H), 0);
            MyFloat3 F0 = MyFloat3.Lerp(MinMetalic, BaseColor, Metalic);
            float Alpha = Roughness;
            Alpha = Alpha * Alpha;
            float SquareAlpha = Alpha * Alpha;
            var k = Alpha / 2.0f;
            var LightColor = Light.GetColor() * LightAttenuationFactor;

            /*
             * 开始计算
             */
            MyFloat3 Ks = F0 + (new MyFloat3(1, 1, 1) - F0) * (float)Math.Pow(1 - VdH, 5);
            var Kd = (new MyFloat3(1, 1, 1) - Ks) * (1 - Metalic);
            MyFloat3 Diffuse = (BaseColor / (float)Math.PI) * LightColor * Kd;

            float G = Schlich_Beckmenn(Normal, L, k) * Schlich_Beckmenn(Normal, V, k);
            
            float D = SquareAlpha / (float)(Math.PI * Math.Pow(NdH * NdH * (SquareAlpha - 1) + 1, 2));

            var TmpA = Ks * (D * G);
            var TmpB = (float)Math.Max(4.0f * (NdV * NdL), 0.00001);
            

            MyFloat3 Specular = new MyFloat3();
            if(TmpB != 0)
            {
                Specular = TmpA / TmpB;
            }
            Specular *= LightColor * (float)Math.Pow(Shiness, 2);

            MyFloat3 Ret = Diffuse + Specular;
            return Ret;
        }

        public float Schlich_Beckmenn(MyFloat3 N, MyFloat3 X, float k)
        {
            var NdX = Math.Max(MyFloat3.DotProduct(N, X), 0);
            float Ret = NdX / (NdX * (1 - k) + k);
            return Ret;
        }

    }
}
