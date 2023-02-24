using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    internal class WireFrameShader : ShaderBase
    {
        public override ShaderType GetShaderType()
        {
            return ShaderType.WireFrame;
        }
    }
}
