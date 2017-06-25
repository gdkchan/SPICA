using SPICA.Formats.Common;

namespace SPICA.Formats.CtrGfx.Model
{
    public class GfxMeshNodeVisibility : INamed
    {
        private string _Name;

        public string Name
        {
            get => _Name;
            set => _Name = value ?? throw Exceptions.GetNullException("Name");
        }

        public bool IsVisible;
    }
}
