using System;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{

    internal class MyFloat3
    {
        public float X;
        public float Y;
        public float Z;

        public MyFloat3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public MyFloat3(float x, float y)
        {
            X = x;
            Y = y;
        }

        public MyFloat3()
        { }

        public MyFloat3(MyFloat3 Color)
        {
            X = Color.X;
            Y = Color.Y;
            Z = Color.Z;
        }

        public static MyFloat3 operator +(MyFloat3 A, MyFloat3 B)
        {
            MyFloat3 Ret = new MyFloat3();
            Ret.X = A.X + B.X;
            Ret.Y = A.Y + B.Y;
            Ret.Z = A.Z + B.Z;
            return Ret;
        }

        public static MyFloat3 operator -(MyFloat3 A, MyFloat3 B)
        {
            MyFloat3 Ret = new MyFloat3();
            Ret.X = A.X - B.X;
            Ret.Y = A.Y - B.Y;
            Ret.Z = A.Z - B.Z;
            return Ret;
        }

        public static MyFloat3 operator -(MyFloat3 A, MyFloat4 B)
        {
            MyFloat3 Ret = new MyFloat3();
            Ret.X = A.X - B.X;
            Ret.Y = A.Y - B.Y;
            Ret.Z = A.Z - B.Z;
            return Ret;
        }

        public static MyFloat3 operator *(MyFloat3 A, MyFloat3 B)
        {
            MyFloat3 Ret = new MyFloat3();
            Ret.X = A.X * B.X;
            Ret.Y = A.Y * B.Y;
            Ret.Z = A.Z * B.Z;
            return Ret;
        }
        public static MyFloat3 operator *(MyFloat3 A, float B)
        {
            MyFloat3 Ret = new MyFloat3();
            Ret.X = A.X * B;
            Ret.Y = A.Y * B;
            Ret.Z = A.Z * B;
            return Ret;
        }

        public static MyFloat3 operator /(MyFloat3 A, MyFloat3 B)
        {
            MyFloat3 Ret = new MyFloat3();
            Ret.X = A.X / B.X;
            Ret.Y = A.Y / B.Y;
            Ret.Z = A.Z / B.Z;
            return Ret;
        }
        public static MyFloat3 operator /(MyFloat3 A, float B)
        {
            MyFloat3 Ret = new MyFloat3();
            Ret.X = A.X / B;
            Ret.Y = A.Y / B;
            Ret.Z = A.Z / B;
            return Ret;
        }
        public static MyFloat3 Lerp(MyFloat3 A, MyFloat3 B, float Alpha)
        {
            MyFloat3 Ret = new MyFloat3();
            Ret = A + ((B - A) * Alpha);
            return Ret;
        }
        public double Len()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public void Clamp(float Min, float Max, bool bCycle = false)
        {
            X = Utils.Clamp(X, Min, Max, bCycle);
            Y = Utils.Clamp(Y, Min, Max, bCycle);
            Z = Utils.Clamp(Z, Min, Max, bCycle);
        }

        public static MyFloat3 GetNormalize(MyFloat3 V)
        {
            return V.GetNormalize();
        }

        public MyFloat3 GetNormalize()
        {
            MyFloat3 Ret = new MyFloat3(X, Y, Z);
            Ret.Normalize();
            return Ret;
        }

        public void Normalize()
        {
            float Len = (float)this.Len();
            X = X / Len;
            Y = Y / Len;
            Z = Z / Len;
        }

        public static float DotProduct(MyFloat3 V1, MyFloat3 V2)
        {
            return V1.X * V2.X + V1.Y * V2.Y + V1.Z * V2.Z;
        }

        public static MyFloat3 CrossProduct(MyFloat3 V1, MyFloat3 V2)
        {
            float X = V1.Y * V2.Z - V1.Z * V2.Y;
            float Y = V1.Z * V2.X - V1.X * V2.Z;
            float Z = V1.X * V2.Y - V1.Y * V2.X;
            return new MyFloat3(X, Y, Z);
        }

        public static MyFloat4 operator *(MyFloat3 Vector, MyMatrix Matrix)
        {
            return new MyFloat4(Vector.X, Vector.Y, Vector.Z, 1) * Matrix;
        }

        public Color ToColor()
        {
            return Color.FromArgb((int)(X * 255), (int)(Y * 255), (int)(Z * 255));
        }

    }
}
