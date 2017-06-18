namespace SPICA.Formats.CtrH3D.Animation
{
    public enum H3DTargetType : ushort
    {
        Bone,

        MaterialConstant0,
        MaterialConstant1,
        MaterialConstant2,
        MaterialConstant3,
        MaterialConstant4,
        MaterialConstant5,
        MaterialEmission,
        MaterialAmbient,
        MaterialDiffuse,
        MaterialSpecular0,
        MaterialSpecular1,
        MaterialMapper0BorderCol,
        MaterialMapper0Texture,
        MaterialMapper1BorderCol,
        MaterialMapper1Texture,
        MaterialMapper2BorderCol,
        MaterialMapper2Texture,
        MaterialBlendColor,
        MaterialTexCoord0Scale,
        MaterialTexCoord0Rot,
        MaterialTexCoord0Trans,
        MaterialTexCoord1Scale,
        MaterialTexCoord1Rot,
        MaterialTexCoord1Trans,
        MaterialTexCoord2Scale,
        MaterialTexCoord2Rot,
        MaterialTexCoord2Trans,

        LightTransform,
        LightAmbient,
        LightDiffuse,
        LightSpecular0,
        LightSpecular1,
        LightGround,
        LightSky,
        LightDirection,
        LightAttenuationStart,
        LightAttenuationEnd,
        LightInterpolationFactor,
        LightEnabled,

        CameraTransform,
        CameraTargetPos,
        CameraTwist,
        CameraUpVector,
        CameraViewRotation,
        CameraZNear,
        CameraZFar,
        CameraAspectRatio,
        CameraHeight,

        MeshNodeVisibility,

        ModelVisibility,

        FogColor
    }
}
