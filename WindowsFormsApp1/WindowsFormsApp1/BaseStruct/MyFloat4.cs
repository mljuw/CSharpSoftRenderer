using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    internal class MyFloat4
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        public MyFloat4()
        {

        }

        public MyFloat4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public MyFloat4(MyFloat4 CopyThis)
        {
            X = CopyThis.X;
            Y = CopyThis.Y;
            Z = CopyThis.Z;
            W = CopyThis.W;
        }

        public static MyFloat4 operator +(MyFloat4 A, MyFloat4 B)
        {
            MyFloat4 Ret = new MyFloat4();
            Ret.X = A.X + B.X;
            Ret.Y = A.Y + B.Y;
            Ret.Z = A.Z + B.Z;
            Ret.W = A.W + B.W;
            return Ret;
        }


        public static MyFloat4 operator +(MyFloat4 A, MyFloat3 B)
        {
            MyFloat4 Ret = new MyFloat4();
            Ret.X = A.X + B.X;
            Ret.Y = A.Y + B.Y;
            Ret.Z = A.Z + B.Z;
            return Ret;
        }

        public static MyFloat4 operator -(MyFloat4 A, MyFloat4 B)
        {
            MyFloat4 Ret = new MyFloat4();
            Ret.X = A.X - B.X;
            Ret.Y = A.Y - B.Y;
            Ret.Z = A.Z - B.Z;
            Ret.W = A.W - B.W;
            return Ret;
        }

        public static MyFloat4 operator *(MyFloat4 A, MyFloat4 B)
        {
            MyFloat4 Ret = new MyFloat4();
            Ret.X = A.X * B.X;
            Ret.Y = A.Y * B.Y;
            Ret.Z = A.Z * B.Z;
            Ret.W = A.W * B.W;
            return Ret;
        }


        public static MyFloat4 operator *(MyFloat4 A, float B)
        {
            MyFloat4 Ret = new MyFloat4();
            Ret.X = A.X * B;
            Ret.Y = A.Y * B;
            Ret.Z = A.Z * B;
            Ret.W = A.W * B;
            return Ret;
        }


        public static MyFloat4 operator /(MyFloat4 A, MyFloat4 B)
        {
            MyFloat4 Ret = new MyFloat4();
            Ret.X = A.X / B.X;
            Ret.Y = A.Y / B.Y;
            Ret.Z = A.Z / B.Z;
            Ret.W = A.W / B.W;
            return Ret;
        }
        public static MyFloat4 operator /(MyFloat4 A, float B)
        {
            MyFloat4 Ret = new MyFloat4();
            Ret.X = A.X / B;
            Ret.Y = A.Y / B;
            Ret.Z = A.Z / B;
            Ret.W = A.W / B;
            return Ret;
        }


        public static MyFloat4 operator *(MyFloat4 Vector, MyMatrix Mat)
        {
            MyFloat4 Ret = new MyFloat4();
            Ret.X = Vector.X * Mat.XPlane.X + Vector.Y * Mat.YPlane.X + Vector.Z * Mat.ZPlane.X + Vector.W * Mat.WPlane.X;
            Ret.Y = Vector.X * Mat.XPlane.Y + Vector.Y * Mat.YPlane.Y + Vector.Z * Mat.ZPlane.Y + Vector.W * Mat.WPlane.Y;
            Ret.Z = Vector.X * Mat.XPlane.Z + Vector.Y * Mat.YPlane.Z + Vector.Z * Mat.ZPlane.Z + Vector.W * Mat.WPlane.Z;
            Ret.W = Vector.X * Mat.XPlane.W + Vector.Y * Mat.YPlane.W + Vector.Z * Mat.ZPlane.W + Vector.W * Mat.WPlane.W;
            return Ret;
        }

        public MyFloat3 XYZ()
        {
            MyFloat3 Ret = new MyFloat3();
            Ret.X = X;
            Ret.Y = Y;
            Ret.Z = Z;
            return Ret;
        }

        public Color ToColor()
        {
            return Color.FromArgb((int)X * 255, (int)Y * 255, (int)Z * 255);
        }

        public static MyFloat4 Lerp(MyFloat4 A, MyFloat4 B, float Alpha)
        {
            MyFloat4 Ret = new MyFloat4();
            Ret = A + ((B - A) * Alpha);
            return Ret;
        }

        public static float DotProduct(MyFloat4 V1, MyFloat4 V2)
        {
            return V1.X * V2.X + V1.Y * V2.Y + V1.Z * V2.Z + V1.W * V2.W;
        }

        public static MyFloat3 CrossProduct(MyFloat4 V1, MyFloat4 V2)
        {
            float X = V1.Y * V2.Z - V2.X * V1.Z;
            float Y = V1.Z * V2.X - V2.Z * V1.X;
            float Z = V1.X * V2.Y - V2.X * V1.Y;
            return new MyFloat3(X, Y , Z);
        }

        public static MyFloat3 GetNormalize(MyFloat4 V)
        {
            return V.GetNormalize();
        }

        public MyFloat3 GetNormalize()
        {
            float l = (float)Len(this);
            MyFloat3 Ret = new MyFloat3();
            Ret.X = X / l;
            Ret.Y = Y / l;
            Ret.Z = Z / l;
            return Ret;
        }


        public static double Len(MyFloat4 V)
        {
            return Math.Sqrt(V.X * V.X + V.Y * V.Y + V.Z * V.Z);
        }

      
    }

}
