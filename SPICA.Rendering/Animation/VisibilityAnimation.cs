using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Animation;

using System.Collections.Generic;

namespace SPICA.Rendering.Animation
{
    public class VisibilityAnimation : AnimationControl
    {
        private H3DPatriciaTree MeshNodeNames;

        private List<bool> MeshNodeVisibilities;

        private bool[] Visibilities;

        public VisibilityAnimation(H3DPatriciaTree MeshNodeNames, List<bool> MeshNodeVisibilities)
        {
            this.MeshNodeNames        = MeshNodeNames;
            this.MeshNodeVisibilities = MeshNodeVisibilities;

            Visibilities = new bool[MeshNodeVisibilities.Count];
        }

        public override void SetAnimations(IEnumerable<H3DAnimation> Animations)
        {
            ResetVisibilities();

            SetAnimations(Animations, MeshNodeNames);
        }

        private void ResetVisibilities()
        {
            for (int i = 0; i < MeshNodeVisibilities.Count; i++)
            {
                Visibilities[i] = MeshNodeVisibilities[i];
            }
        }

        public bool[] GetMeshVisibilities()
        {
            if (State == AnimationState.Stopped)
            {
                ResetVisibilities();
            }

            if (State != AnimationState.Playing || Elements.Count == 0)
            {
                return Visibilities;
            }

            for (int i = 0; i < Elements.Count; i++)
            {
                int Index = Indices[i];

                H3DAnimationElement Elem = Elements[i];

                if (Elem.PrimitiveType == H3DPrimitiveType.Boolean)
                {
                    bool Value = ((H3DAnimBoolean)Elem.Content).GetFrameValue((int)Frame);

                    switch (Elem.TargetType)
                    {
                        case H3DTargetType.MeshNodeVisibility: Visibilities[Index] = Value; break;
                    }
                }
            }

            return Visibilities;
        }
    }
}
