using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.Light
{
    internal class DirectionalLight : LightBase
    {
        public DirectionalLight()
        {
            LightType = LightType.DirectionalLight;
        }
    }
}
