using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    internal class PBRMatParams
    {
        public Texture BaseColor = null;
        public Texture Metalic = null;
        public Texture Roughness = null;
        public Texture Emissive = null;
        public Texture AO = null;
        public float Shiness = 2;

        /*
         * Roughness Metalic Specular 组成一张贴图,分别是 x, y, z
         */
        public Texture RMSCombine = null;
        //public float AmbientAbsorbance = 0.1f;

        public bool IsCombinedTexture()
        {
            return null != RMSCombine;
        }

        public MyFloat3 GetBaseColor(MyFloat2 UV, TextureSampleType SampleType = TextureSampleType.BiLinear)
        {
            if (BaseColor != null)
            {
                return BaseColor.Sample(UV, SampleType).XYZ();
            }
            return new MyFloat3(1, 1, 1);
        }

        public MyFloat3 GetMetalicr(MyFloat2 UV, TextureSampleType SampleType = TextureSampleType.BiLinear)
        {
            if (Metalic != null)
            {
                return Metalic.Sample(UV, SampleType).XYZ();
            }
            return new MyFloat3(0.04f, 0.04f, 0.04f);
        }

        public MyFloat3 GetRoughness(MyFloat2 UV, TextureSampleType SampleType = TextureSampleType.BiLinear)
        {
            if (Roughness != null)
            {
                return Roughness.Sample(UV, SampleType).XYZ();
            }
            return new MyFloat3(0.5f, 0.5f, 0.5f);
        }
        public MyFloat3 GetEmissive(MyFloat2 UV, TextureSampleType SampleType = TextureSampleType.BiLinear)
        {
            if (Emissive != null)
            {
                return Emissive.Sample(UV, SampleType).XYZ();
            }
            return new MyFloat3(0.0f, 0.0f, 0.0f);
        }
        public MyFloat3 GetAO(MyFloat2 UV, TextureSampleType SampleType = TextureSampleType.BiLinear)
        {
            if (AO != null)
            {
                return AO.Sample(UV, SampleType).XYZ();
            }
            return new MyFloat3(1.0f, 1.0f, 1.0f);
        }

        public MyFloat4 SampleWithCombinedTexture(MyFloat2 UV, TextureSampleType SampleType = TextureSampleType.BiLinear)
        {
            if (null == RMSCombine) return null;
            return RMSCombine.Sample(UV, SampleType);
        }
    }

    internal class MaterialParams
    {
        public Texture Diffuse = null;

        public Texture Specular = null;

        public Texture NormalMap = null;

        public MyFloat3 Ka = new MyFloat3(0.02f, 0.02f, 0.02f);

        public MyFloat3 SpecularColor = new MyFloat3(1, 1, 1);

        public MyFloat3 DiffuseColor = new MyFloat3(0, 0, 1);

        public PBRMatParams PBRParams = new PBRMatParams();


        public MyFloat3 GetSpecular(MyFloat2 UV, TextureSampleType SampleType = TextureSampleType.BiLinear)
        {
            if(Specular != null)
            {
                return Specular.Sample(UV, SampleType).XYZ();
            }
            return SpecularColor;
        }

        public MyFloat3 GetDiffuse(MyFloat2 UV, TextureSampleType SampleType = TextureSampleType.BiLinear)
        {
            if (Diffuse != null)
            {
                return Diffuse.Sample(UV, SampleType).XYZ();
            }
            return DiffuseColor;
        }

        public MyFloat3 GetNormal(MyFloat2 UV, MyFloat3 DefaultValue, TextureSampleType SampleType = TextureSampleType.BiLinear)
        {
            if (NormalMap != null)
            {
                return NormalMap.Sample(UV, SampleType).XYZ();

            }
            return DefaultValue;
        }
    }
}
