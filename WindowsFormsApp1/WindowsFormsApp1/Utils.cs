using System;
using System.Collections;
using System.Windows.Forms.VisualStyles;

namespace WindowsFormsApp1
{
    internal class Utils
    {
        static public float Clamp(float X, float Min, float Max, bool bCycle = false)
        {
            if (X > Max)
            {
                if (bCycle)
                    X = Min;
                else
                    X = Max;
            }
            else if (X < Min)
            {
                if (bCycle)
                    X = Max;
                else
                    X = Min;
            }
            return X;
        }


        static public float CalcFluoroscopicCorrectionXt(MyFloat3 AlphaBetaGamma, float AlphaDepth, float BetaDepth, float GammaDepth)
        {
            return 1 / (AlphaBetaGamma.X / AlphaDepth + AlphaBetaGamma.Y / BetaDepth + AlphaBetaGamma.Z / GammaDepth);
        }


        static public float GetLerpAlphaWithCrossOnPlane(ClipPlaneKind Kind, MyFloat4 PrePos, MyFloat4 CurPos)
        {
            float T = 0;
            if (Kind == ClipPlaneKind.W)
            {
                T = (float)((PrePos.W - 0.001) / (PrePos.W - CurPos.W));
            }
            else if (Kind == ClipPlaneKind.Top)
            {
                T = (float)((PrePos.W - PrePos.Z) / (PrePos.W - CurPos.W - (PrePos.Z - CurPos.Z)));
            }
            else if (Kind == ClipPlaneKind.Bottom)
            {
                T = (float)((PrePos.Z + PrePos.W) / (PrePos.W - CurPos.W + (PrePos.Z - CurPos.Z)));
            }
            else if (Kind == ClipPlaneKind.Left)
            {
                T = (float)((PrePos.W + PrePos.Y) / (PrePos.W - CurPos.W + (PrePos.Y - CurPos.Y)));
            }
            else if (Kind == ClipPlaneKind.Right)
            {
                T = (float)((PrePos.W - PrePos.Y) / (PrePos.W - CurPos.W - (PrePos.Y - CurPos.Y)));
            }
            else if (Kind == ClipPlaneKind.Far)
            {
                T = (float)((PrePos.W - PrePos.X) / (PrePos.W - CurPos.W - (PrePos.X - CurPos.X)));
            }
            else if (Kind == ClipPlaneKind.Near)
            {
                T = (float)(PrePos.X / (PrePos.X - CurPos.X));
            }
            return T;
        }


        static public bool IsBackFace(MyFloat4 V0, MyFloat4 V1, MyFloat4 V2)
        {
            MyFloat3 A = (V2 - V0).GetNormalize();
            MyFloat3 B = (V1 - V0).GetNormalize();

            MyFloat3 Corss = MyFloat3.CrossProduct(A, B);
            float Dot = MyFloat3.DotProduct(new MyFloat3(-1, 0, 0), Corss);
            return Dot > 0;

            //float CrossZ = (V0.Z * V1.Y - V0.Y * V1.Z) + V0.Y * V2.Z - V0.Z * V2.Y - V1.Y * V2.Z + V1.Z * V2.Y;
            //return CrossZ <= 0;
        }

        static public void GetTrangleAABB(MyFloat4 V0, MyFloat4 V1, MyFloat4 V2, ref MyInt2 Min, ref MyInt2 Max)
        {
            //List<MyFloat4> TmpList = new List<MyFloat4>(3);
            MyFloat4[] TmpList = new MyFloat4[3];
            TmpList[0] = V0;
            TmpList[1] = V1;
            TmpList[2] = V2;

            for (int i = 0; i < 3; ++i)
            {
                int X = (int)Math.Round(TmpList[i].Y);
                int Y = (int)Math.Round(TmpList[i].Z);
                if (0 == i)
                {
                    Min.X = X;
                    Min.Y = Y;
                    Max.X = X;
                    Max.Y = Y;
                }
                else
                {
                    if (X > Max.X)
                        Max.X = X;
                    if (Y > Max.Y)
                        Max.Y = Y;

                    if (X < Min.X)
                        Min.X = X;
                    if (Y < Min.Y)
                        Min.Y = Y;
                }
            }
        }

        static public void CalcTangentAndBiTangent(MyFloat3 V1, MyFloat3 V2, MyFloat3 V3, MyFloat2 UV1, MyFloat2 UV2, MyFloat2 UV3, MyFloat3 Normal, ref MyFloat3 Tangent, ref MyFloat3 BiTangent)
        {
            MyFloat3 Edge1 = V2 - V1;
            MyFloat3 Edge2 = V3 - V2;
            MyFloat2 DeltaUV1 = UV2 - UV1;
            MyFloat2 DeltaUV2 = UV3 - UV2;


            float DivNum = DeltaUV1.X * DeltaUV2.Y - DeltaUV2.X * DeltaUV1.Y;

            if(DivNum == 0)
            {
                return;
            }

            float f = 1.0f / DivNum;

            Tangent.X = f * (DeltaUV2.Y * Edge1.X - DeltaUV1.Y * Edge2.X);
            Tangent.Y = f * (DeltaUV2.Y * Edge1.Y - DeltaUV1.Y * Edge2.Y);
            Tangent.Z = f * (DeltaUV2.Y * Edge1.Z - DeltaUV1.Y * Edge2.Z);
            Tangent.Normalize();

            BiTangent = MyFloat3.CrossProduct(Normal, Tangent);
            BiTangent.Normalize();

            //BiTangent.X = f * (-DeltaUV2.X * Edge1.X - DeltaUV1.X * Edge2.X);
            //BiTangent.Y = f * (-DeltaUV2.X * Edge1.Y - DeltaUV1.X * Edge2.Y);
            //BiTangent.Z = f * (-DeltaUV2.X * Edge1.Z - DeltaUV1.X * Edge2.Z);
            //BiTangent.Normalize();

        }


        static public bool CheckPixelInTrangle(MyFloat3 AlphaBetaGamma)
        {
            float LittleNum = -0.000001f;
            return AlphaBetaGamma.X >= LittleNum && AlphaBetaGamma.Y >= LittleNum && AlphaBetaGamma.Z >= LittleNum;
        }

        static public MyFloat3 GetBarycentricCoordinateParams(MyFloat2 V1, MyFloat2 V2, MyFloat2 V3, MyFloat2 G)
        {
            float y2_y3 = V2.Y - V3.Y;
            float y_y3 = G.Y - V3.Y;
            float x2_x3 = V2.X - V3.X;
            float x1_x3 = V1.X - V3.X;
            float x_x3 = G.X - V3.X;
            float y1_y3 = V1.Y - V3.Y;

            float Numerator = (y2_y3 * x1_x3 - y1_y3 * x2_x3);

            float Alpha = (y2_y3 * x_x3 - y_y3 * x2_x3) / Numerator;
            float Beta = (y_y3 * x1_x3 - y1_y3 * x_x3) / Numerator;
            float Gamma = 1.0f - Alpha - Beta;

            return new MyFloat3(Alpha, Beta, Gamma);
        }

        static public float Clamp(float Value, float Min, float Max)
        {
            if (Value < Min)
                return Min;
            if (Value > Max)
                return Max;
            return Value;
        }

        static public float Clamp(int Value, int Min, int Max)
        {
            if (Value < Min)
                return Min;
            if (Value > Max)
                return Max;
            return Value;
        }
    }
}
