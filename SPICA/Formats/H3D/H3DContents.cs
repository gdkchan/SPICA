using SPICA.Formats.H3D.Contents;

namespace SPICA.Formats.H3D
{
    class H3DContents
    {
        public H3DModels Models;
        public H3DMaterials Materials;
        public H3DShaders Shaders;
        public H3DTextures Textures;
        public H3DLUTs LUTs;
        public H3DLights Lights;
        public H3DCameras Cameras;
        public H3DFogs Fogs;
        public H3DAnimations SkeletalAnimations;
        public H3DAnimations MaterialAnimations;
        public H3DAnimations VisibilityAnimations;
        public H3DAnimations LightAnimations;
        public H3DAnimations CameraAnimations;
        public H3DAnimations FogAnimations;
        public H3DScenes Scenes;
    }
}
