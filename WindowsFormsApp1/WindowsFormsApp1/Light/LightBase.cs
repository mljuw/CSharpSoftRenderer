using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    enum LightType
    {
        DirectionalLight,
        PointLight,
        SpotLight,
    }
    internal class LightBase
    {
        public Transform Transform = new Transform();

        public LightType LightType { get; set; }

        MyFloat3 Color = new MyFloat3(1, 1, 1);

        public MyFloat3 GetColor()
        {
            return Color;
        }

        public void SetColor(float R, float G, float B)
        {
            Color.X = R;
            Color.Y = G;
            Color.Z = B;
        }

        virtual public float GetAttenuationFactor(MyFloat4 WorldPos)
        {
            return 1;
        }

        virtual public MyFloat3 GetLightDir(MyFloat4 WorldPos)
        {
            return (Transform.GetLoc() - WorldPos).GetNormalize();
        }

        virtual public bool InRange(MyFloat4 WorldPos)
        {
            return true;
        }
    }
}
