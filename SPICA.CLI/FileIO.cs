using System;
using System.IO;
using SPICA.WinForms;
using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.Generic.COLLADA;
using SPICA.WinForms.Formats;

namespace SPICA.CLI
{
    class FileIO
    {
        public static H3D Merge(string[] FileNames, H3D Scene = null)
        {
            if (Scene == null)
            {
                Scene = new H3D();
            }

            int OpenFiles = 0;

            foreach (string FileName in FileNames)
            {
                H3DDict<H3DBone> Skeleton = null;

                if (Scene.Models.Count > 0) Skeleton = Scene.Models[0].Skeleton;

                H3D Data = FormatIdentifier.IdentifyAndOpen(FileName, Skeleton);

                if (Data != null)
                {
                    Scene.Merge(Data);
                    OpenFiles++;
                }
            }

            if (OpenFiles == 0)
            {
                //todo: improve this error message by making the format discovery return some kind of report
                Console.Write("Unsupported file format!", "Can't open file!");
            }

            return Scene;
        }

        public static void ExportDae(H3D Scene, string Filename, int[] SelectedAnimations)
        {
            int highPolyModel = 0;
            new DAE(Scene, highPolyModel, SelectedAnimations).Save(Filename);
        }

        public static void ExportTextures(H3D Scene, string path)
        {
            TextureManager.Textures = Scene.Textures;
            for (int i = 0; i < Scene.Textures.Count; i++)
            {
                string FileName = Path.Combine(path, $"{Scene.Textures[i].Name}.png");

                TextureManager.GetTexture(i).Save(FileName);
            }
        }
    }
}
