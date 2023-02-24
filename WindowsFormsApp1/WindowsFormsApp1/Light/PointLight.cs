using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.Light
{
    internal class PointLight : LightBase
    {
        protected float SourceRange = 100;
        protected float AttenuationRange = 200;

        public override float GetAttenuationFactor(MyFloat4 WorldPos)
        {
            var Diff = Transform.GetLoc() - WorldPos;
            double Distance = Diff.Len();
            if (SourceRange > Distance) return 1;
            if (Distance >= AttenuationRange) return 0;
            return (float)((AttenuationRange - Distance) / (AttenuationRange - SourceRange));
        }
        public override bool InRange(MyFloat4 WorldPos)
        {
            var Diff = Transform.GetLoc() - WorldPos;
            return Diff.Len() <= AttenuationRange;
        }

        public void SetSourceRange(float NewValue)
        {
            SourceRange = NewValue;
            if (SourceRange > AttenuationRange)
                AttenuationRange = SourceRange;
        }

        public void SetAttenuationRange(float NewValue)
        {
            AttenuationRange = NewValue;
            if (SourceRange > AttenuationRange)
                SourceRange = AttenuationRange;
        }
    }
}
