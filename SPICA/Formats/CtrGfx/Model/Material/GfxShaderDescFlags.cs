namespace SPICA.Formats.CtrGfx.Model.Material
{
    public enum GfxShaderDescFlags
    {
        IsRigidSkinningSupported      = 1 << 0,
        IsSmoothSkinningSupported     = 1 << 1,
        IsHemiSphereLightingSupported = 1 << 2,
        IsGeometryShaderSupported     = 1 << 3
    }
}
