using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;

namespace WindowsFormsApp1
{

    enum TextureType
    {
        sRGB,
        LinearColor,
        Normal,
    }

    enum TextureSampleType
    {
        Nearest,
        BiLinear
    }

    internal class Texture
    {
        Bitmap Img = null;
        TextureType Type = TextureType.sRGB;

        Dictionary<int, MyFloat4> CustomData = null;

        MyInt2 Size = new MyInt2();
        MyInt2 MaxIndex = new MyInt2();

        bool bCustomData = false;

        public Texture(MyInt2 ImgSize)
        {
            Size = ImgSize;
            MaxIndex.X = Size.X - 1;
            MaxIndex.Y = Size.Y - 1;
            bCustomData = true;

            CustomData = new Dictionary<int, MyFloat4>(Size.X * Size.Y);
        }

        public Texture(String Path, TextureType ImgType)
        {
            foreach (var SourcePath in MyRenderer.SourceFindPaths)
            {
                try
                {
                    Img = new Bitmap(SourcePath + Path);
                    break;
                }
                catch (Exception ex)
                {

                }
            }
            Type = ImgType;
            Size.X = Img.Width;
            Size.Y = Img.Height;

            MaxIndex.X = Size.X - 1;
            MaxIndex.Y = Size.Y - 1;
        }

        public MyInt2 GetSize()
        {
            return Size;
        }

        public Dictionary<int, MyFloat4> GetCurstomData()
        {
            return CustomData;
        }

        public MyFloat4 GetColor(int X, int Y)
        {
            if (bCustomData)
            {
                int Index = Y * Size.X + X;
                if(CustomData.ContainsKey(Index))
                    return CustomData[Index];
                return null;
            }

            Color Color = Img.GetPixel(X, Y);
            var Tmp = new MyFloat4(Color.R / 255.0f, Color.G / 255.0f, Color.B / 255.0f, Color.A / 255.0f);
            if (Type == TextureType.Normal)
            {

                //[0 - 1] -> [-1 - 1]
                Tmp.X *= 2;
                Tmp.Y *= 2;
                Tmp.Z *= 2;

                Tmp.X -= 1;
                Tmp.Y -= 1;
                Tmp.Z -= 1;
            }
            else if (Type == TextureType.LinearColor)
            {
                
            }
            else if(Type == TextureType.sRGB)
            {
                Tmp.X = (float)Math.Pow(Tmp.X, 2.2f);
                Tmp.Y = (float)Math.Pow(Tmp.Y, 2.2f);
                Tmp.Z = (float)Math.Pow(Tmp.Z, 2.2f);
            }
            return Tmp;
        }

        public void SetColor(int X, int Y, MyFloat3 NewColor)
        {
            if (bCustomData)
            {
                int Index = Y * Size.X + X;
                if (CustomData.ContainsKey(Index))
                    CustomData[Index] = new MyFloat4(NewColor.X, NewColor.Y, NewColor.Z, 1);
                else
                    CustomData.Add(Index, new MyFloat4(NewColor.X, NewColor.Y, NewColor.Z, 1));
            }
            else
            {
                Color C = Color.FromArgb((int)(NewColor.X * 255), (int)(NewColor.Y * 255), (int)(NewColor.Z * 255));
                Img.SetPixel(X, Y, C);
            }
        }

        public MyFloat4 Sample(MyFloat2 UV, TextureSampleType SampleType = TextureSampleType.Nearest)
        {
            switch(SampleType)
            {
                case TextureSampleType.BiLinear:
                    return SampleWithBiLinear(UV);
                    break;
            }

            return SampleWithNearest(UV);
        }

        public MyFloat4 SampleWithNearest(MyFloat2 UV)
        {
            //UV.X = 0.9f;
            //UV.Y = 0.9f;
            float U = UV.X * Size.X - 0.5f;
            float V = UV.Y * Size.Y - 0.5f;

            int IntU = (int)Math.Round(U);
            int IntV = (int)Math.Round(V);

            ClampMaxIndex(ref IntU, ref IntV);

            return GetColor(IntU, IntV);
        }

        public MyFloat4 SampleWithBiLinear(MyFloat2 UV)
        {
            float U = UV.X * Size.X;
            float V = UV.Y * Size.Y;

            int U0 = (int)Math.Floor(U);
            int V0 = (int)Math.Floor(V);
            ClampMaxIndex(ref U0, ref V0);

            int U1 = U0 + 1;
            int V1 = V0;
            ClampMaxIndex(ref U1, ref V1);


            int U2 = U0;
            int V2 = V0 + 1;
            ClampMaxIndex(ref U2, ref V2);

            int U3 = U0 + 1;
            int V3 = V0 + 1;
            ClampMaxIndex(ref U3, ref V3);


            var C0 = GetColor(U0, V0);
            var C1 = GetColor(U1, V1);
            var C2 = GetColor(U2, V2);
            var C3 = GetColor(U3, V3);

            var S1 = MyFloat4.Lerp(C0, C1, U - U0);
            var S2 = MyFloat4.Lerp(C2, C3, U - U0);

            return MyFloat4.Lerp(S1, S2, V - V0);
        }

        protected void ClampMaxIndex(ref int X, ref int Y)
        {
            X = (int)Utils.Clamp(X, 0, MaxIndex.X);
            Y = (int)Utils.Clamp(Y, 0, MaxIndex.Y);
        }
    }
}
