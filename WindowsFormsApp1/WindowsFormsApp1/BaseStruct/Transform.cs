using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WindowsFormsApp1
{
    internal class Transform
    {

        MyFloat3 Rotation = new MyFloat3();
        MyFloat3 Loc = new MyFloat3();
        MyFloat3 Scale = new MyFloat3(1, 1, 1);
        MyMatrix Matrix = new MyMatrix();
        bool bDirty = true;

        public Transform()
        {

        }

        public void SetScale(float X, float Y, float Z)
        {
            Scale.X = X;
            Scale.Y = Y;
            Scale.Z = Z;
            bDirty = true;
        }

        public void SetRotation(float X, float Y, float Z)
        {
            Rotation.X = X;
            Rotation.Y = Y;
            Rotation.Z = Z;
            Rotation.Clamp(-360, 360, true);
            bDirty = true;
        }

        public void SetRotation(MyFloat3 NewRot)
        {
            Rotation = NewRot;
            Rotation.Clamp(-360, 360, true);
            bDirty = true;
        }

        public MyFloat3 GetRotation()
        {
            return new MyFloat3(Rotation);
        }

        public MyFloat3 GetForwardDir()
        {
            //TODO.待完善
            return new MyFloat3(1, 0, 0);
        }

        public MyFloat3 GetLoc()
        {
            return new MyFloat3(Loc);
        }


        public void SetLocation(MyFloat3 NewLoc)
        {
            Loc = NewLoc;
            bDirty = true;
        }
        public void SetLocation(float X, float Y, float Z)
        {
            Loc.X = X;
            Loc.Y = Y;
            Loc.Z = Z;
            bDirty = true;
        }

        public MyMatrix GetMatrix()
        {
            if(bDirty)
            {
                MyMatrix RollMatrix = new MyMatrix();
                MyMatrix PitchMatrix = new MyMatrix();
                MyMatrix YawMatrix = new MyMatrix();

                double RollRadian = Rotation.X * Math.PI / 180;
                float RollCos = (float)Math.Cos(RollRadian);
                float RollSin = (float)Math.Sin(RollRadian);

                RollMatrix.XPlane = new MyFloat4(1, 0, 0, 0);
                RollMatrix.YPlane = new MyFloat4(0, RollCos, -RollSin, 0);
                RollMatrix.ZPlane = new MyFloat4(0, RollSin, RollCos, 0);


                double PitchRadian = Rotation.Y * Math.PI / 180;
                float PitchCos = (float)Math.Cos(PitchRadian);
                float PitchSin = (float)Math.Sin(PitchRadian);

                PitchMatrix.XPlane = new MyFloat4(PitchCos, 0, PitchSin, 0);
                PitchMatrix.YPlane = new MyFloat4(0, 1, 0, 0);
                PitchMatrix.ZPlane = new MyFloat4(-PitchSin, 0, PitchCos, 0);


                double YawRadian = Rotation.Z * Math.PI / 180;
                float YawCos = (float)Math.Cos(YawRadian);
                float YawSin = (float)Math.Sin(YawRadian);

                YawMatrix.XPlane = new MyFloat4(YawCos, YawSin, 0, 0);
                YawMatrix.YPlane = new MyFloat4(-YawSin, YawCos, 0, 0);
                YawMatrix.ZPlane = new MyFloat4(0, 0, 1, 0);



                MyMatrix ScaleMatrix = new MyMatrix();
                ScaleMatrix.XPlane = new MyFloat4(Scale.X, 0, 0, 0);
                ScaleMatrix.YPlane = new MyFloat4(0, Scale.Y, 0, 0);
                ScaleMatrix.ZPlane = new MyFloat4(0, 0, Scale.Z, 0);

                MyMatrix MoveMatrix = new MyMatrix();

                MoveMatrix.WPlane = new MyFloat4(Loc.X, Loc.Y, Loc.Z, 1);
                var Tmp = ScaleMatrix * RollMatrix;
                Matrix = ScaleMatrix * RollMatrix * PitchMatrix * YawMatrix * MoveMatrix;
            }
            return Matrix;
        }

    }
}
