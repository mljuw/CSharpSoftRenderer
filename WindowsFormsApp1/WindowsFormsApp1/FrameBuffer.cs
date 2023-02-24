using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    internal class FrameBuffer
    {
        private int ScreenWidth;
        private int ScreenHeight;

        public Dictionary<Int32, MyFloat3> ColorBuffer;
        public Dictionary<Int32, float> DepthBuffer;


        public void ClearBuffer(int InScreenWidth, int InScreenHeight)
        {
            ScreenWidth = InScreenWidth;
            ScreenHeight = InScreenHeight;

            if(ColorBuffer != null)
            {
                ColorBuffer.Clear();
            }
            else
            {
                ColorBuffer = new Dictionary<int, MyFloat3>(ScreenWidth * ScreenHeight);
            }
          
            if(DepthBuffer != null)
            {
                DepthBuffer.Clear();
            }
            else 
            {
                DepthBuffer = new Dictionary<int, float>(ScreenWidth * ScreenHeight);
            }

            
        }

        public void UpdateDepth(int X, int Y, float Depth)
        {
            int Index = Y * ScreenWidth + X;
            if (!DepthBuffer.ContainsKey(Index))
            {
                DepthBuffer.Add(Index, Depth);
            }
            else
            {
                DepthBuffer[Index] = Depth;
            }
        }

        public float GetDepth(int X, int Y)
        {
            int Index = Y * ScreenWidth + X;
            if (!DepthBuffer.ContainsKey(Index)) return -1.0f;
            return DepthBuffer[Index];
        }

        public void AddColor(int X, int Y, MyFloat3 Color)
        {
            int Index = Y * ScreenWidth + X;
            if (ColorBuffer.ContainsKey(Index))
            {
                ColorBuffer[Index] = Color;
                return;
            }
            ColorBuffer.Add(Index, Color);
        }

        public  MyInt2 IndexToSize(int Index)
        {
            MyInt2 Ret = new MyInt2();
            Ret.Y = Index / ScreenWidth;
            Ret.X = Index % ScreenWidth;
            return Ret;
        }
    }
}
