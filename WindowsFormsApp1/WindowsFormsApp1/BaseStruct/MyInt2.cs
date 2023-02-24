using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    internal class MyInt2
    {
        public int X;
        public int Y;

        public MyInt2()
        {

        }

        public MyInt2(int InX, int IntY)
        {
            X = InX;
            Y = IntY;
        }

        public MyInt2 Rotator(float Degree)
        {
            MyInt2 Ret = new MyInt2();
            double Radian = (double)Degree * Math.PI / 180;
            Ret.X = (int)(X * Math.Cos(Radian) - Y * Math.Sin(Radian));
            Ret.Y = (int)(X * Math.Sin(Radian) + Y * Math.Cos(Radian));

            return Ret;
        }


        public static MyInt2 operator +(MyInt2 A, MyInt2 B)
        {
            MyInt2 Ret = new MyInt2();
            Ret.X = A.X + B.X;
            Ret.Y = A.Y + B.Y;
            return Ret;
        }
        public static MyInt2 operator -(MyInt2 A, MyInt2 B)
        {
            MyInt2 Ret = new MyInt2();
            Ret.X = A.X - B.X;
            Ret.Y = A.Y - B.Y;
            return Ret;
        }

        public static MyInt2 operator *(MyInt2 A, MyInt2 B)
        {
            MyInt2 Ret = new MyInt2();
            Ret.X = A.X * B.X;
            Ret.Y = A.Y * B.Y;
            return Ret;
        }
        public static MyInt2 operator /(MyInt2 A, MyInt2 B)
        {
            MyInt2 Ret = new MyInt2();
            Ret.X = A.X / B.X;
            Ret.Y = A.Y / B.Y;
            return Ret;
        }

        public void Clamp(float Min, float Max, bool bCycle = false)
        {
            X = (int)Utils.Clamp(X, Min, Max, bCycle);
            Y = (int)Utils.Clamp(Y, Min, Max, bCycle);
        }

    }
}
