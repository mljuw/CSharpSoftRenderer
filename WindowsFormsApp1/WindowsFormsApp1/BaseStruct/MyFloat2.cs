using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    internal class MyFloat2
    {

        public float X;
        public float Y;

        public MyFloat2()
        {

        }
        public MyFloat2(MyFloat2 Other)
        {
            X = Other.X;
            Y = Other.Y;
        }

        public MyFloat2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static MyFloat2 operator +(MyFloat2 A, MyFloat2 B)
        {
            MyFloat2 Ret = new MyFloat2();
            Ret.X = A.X + B.X;
            Ret.Y = A.Y + B.Y;
            return Ret;
        }
        public static MyFloat2 operator -(MyFloat2 A, MyFloat2 B)
        {
            MyFloat2 Ret = new MyFloat2();
            Ret.X = A.X - B.X;
            Ret.Y = A.Y - B.Y;
            return Ret;
        }

        public static MyFloat2 operator *(MyFloat2 A, MyFloat2 B)
        {
            MyFloat2 Ret = new MyFloat2();
            Ret.X = A.X * B.X;
            Ret.Y = A.Y * B.Y;
            return Ret;
        }

        public static MyFloat2 operator *(MyFloat2 A, float B)
        {
            MyFloat2 Ret = new MyFloat2();
            Ret.X = A.X * B;
            Ret.Y = A.Y * B;
            return Ret;
        }

        public static MyFloat2 operator /(MyFloat2 A, MyFloat2 B)
        {
            MyFloat2 Ret = new MyFloat2();
            Ret.X = A.X / B.X;
            Ret.Y = A.Y / B.Y;
            return Ret;
        }

        public static float DotProduct(MyFloat2 A, MyFloat2 B)
        {
            return A.X * B.X + A.Y * B.Y;
        }

        internal static MyFloat2 Lerp(MyFloat2 A, MyFloat2 B, float Alpha)
        {
            return A + (B - A) * Alpha;
        }
    }
}
