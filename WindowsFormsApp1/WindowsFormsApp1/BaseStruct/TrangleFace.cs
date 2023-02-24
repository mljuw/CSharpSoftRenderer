using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.BaseStruct
{
    internal class TrangleFace
    {
        public List<int> Indices = new List<int>();

        public List<MyFloat2> UVs = new List<MyFloat2>();

        public List<MyFloat3> Vertices = new List<MyFloat3>();

        public List<MyFloat3> Normal = new List<MyFloat3>();

        public MyFloat3 Tangent = new MyFloat3();

        public MyFloat3 BiTangent = new MyFloat3();
    }
}
