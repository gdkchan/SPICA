using OpenTK;
using OpenTK.Graphics.ES30;

using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Model.Material;
using SPICA.Formats.CtrH3D.Model.Mesh;
using SPICA.Formats.CtrH3D.Texture;

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace SPICA.Renderer
{
    public class Model
    {
        public RenderEngine Parent;

        public Matrix4 Transform;

        public List<Mesh> Meshes;

        public PatriciaList<H3DMaterial> Materials;

        private Dictionary<string, int> TextureIds;

        public Model(RenderEngine Renderer, H3D Model, int ModelIndex, int ShaderHandle)
        {
            Parent = Renderer;

            UpdateView(Transform = Matrix4.Identity);

            Meshes = new List<Mesh>();

            Materials = Model.Models[ModelIndex].Materials;

            foreach (H3DMesh Mesh in Model.Models[ModelIndex].Meshes)
            {
                Meshes.Add(new Mesh(this, Mesh, ShaderHandle));
            }

            TextureIds = new Dictionary<string, int>();

            foreach (H3DTexture Texture in Model.Textures)
            {
                int TextureId = GL.GenTexture();

                Bitmap Img = Texture.ToBitmap(true);
                Rectangle ImgRect = new Rectangle(0, 0, Img.Width, Img.Height);
                BitmapData ImgData = Img.LockBits(ImgRect, ImageLockMode.ReadOnly, Img.PixelFormat);

                GL.BindTexture(TextureTarget.Texture2D, TextureId);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexImage2D(TextureTarget2d.Texture2D, 
                    0,
                    TextureComponentCount.Rgba, 
                    Img.Width,
                    Img.Height,
                    0,
                    OpenTK.Graphics.ES30.PixelFormat.Rgba, 
                    PixelType.UnsignedByte,
                    ImgData.Scan0);

                Img.UnlockBits(ImgData);

                TextureIds.Add(Texture.Name, TextureId);
            }
        }

        public void Scale(Vector3 Scale)
        {
            UpdateView(Transform *= Matrix4.CreateScale(Scale));
        }

        public void Rotate(Vector3 Rotation)
        {
            UpdateView(Transform *= Utils.EulerRotate(Rotation));
        }

        public void Translate(Vector3 Translation)
        {
            UpdateView(Transform *= Matrix4.CreateTranslation(Translation));
        }

        public void ScaleAbs(Vector3 Scale)
        {
            UpdateView(Transform = (Transform.ClearScale() * Matrix4.CreateScale(Scale)));
        }

        public void RotateAbs(Vector3 Rotation)
        {
            UpdateView(Transform = (Transform.ClearRotation() * Utils.EulerRotate(Rotation)));
        }

        public void TranslateAbs(Vector3 Translation)
        {
            UpdateView(Transform = (Transform.ClearTranslation() * Matrix4.CreateTranslation(Translation)));
        }

        private void UpdateView(Matrix4 Mtx)
        {
            GL.UniformMatrix4(Parent.ModelMtxLocation, false, ref Transform);
        }

        public void Render()
        {
            foreach (Mesh Mesh in Meshes) Mesh.Render();
        }

        public int GetTextureId(string Name)
        {
            return TextureIds[Name];
        }
    }
}
