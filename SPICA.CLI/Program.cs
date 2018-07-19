using System;

using SPICA.Formats.CtrH3D;
using System.Collections.Generic;

namespace SPICA.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("SPICA CLI");

            //todo: args. look into Mono.Options again
            //take in a pokemon name and discover the ~9 file names via a simple map

            //fake args (only 4)
            string Pokemon = "Bulbasaur";
            string[] files = new string[] { "B:/pokemon/decompressed-0-9-4/00001.bin", "B:/pokemon/decompressed-0-9-4/00002.bin", "B:/pokemon/decompressed-0-9-4/00003.bin", "B:/pokemon/decompressed-0-9-4/00004.bin", "B:/pokemon/decompressed-0-9-4/00005.bin", "B:/pokemon/decompressed-0-9-4/00006.bin", "B:/pokemon/decompressed-0-9-4/00007.bin", "B:/pokemon/decompressed-0-9-4/00008.bin", "B:/pokemon/decompressed-0-9-4/00009.bin" };
            string TextureLocation = "B:/pokemon/tex";
            string ModelLocation = "B:/pokemon/dae";

            Console.WriteLine($"Searching for: {Pokemon}");

            Console.Write($"Building Scene with files:\n{string.Join("\n", files)}\n");
            H3D Scene = FileIO.Merge(files);

            Console.WriteLine($"Exporting {Scene.Textures.Count} textures");
            FileIO.ExportTextures(Scene, TextureLocation);

            int[] motions = GetMotionIndices(Scene, Pokedex.StandardMotion.Values);

            Console.WriteLine($"exporting {ModelLocation}/{Pokemon}.dae model with {motions.Length} motions");
            FileIO.ExportDae(Scene, $"{ModelLocation}/{Pokemon}.dae", motions);

            Console.WriteLine($"completed exports for: {Pokemon}");
        }

        public static int[] GetMotionIndices(H3D Scene, IEnumerable<string> MotionNames)
        {
            List<string> motionNames = new List<string>(MotionNames);
            List<int> targets = new List<int>();

            for (int i = 0; i < Scene.SkeletalAnimations.Count; i++)
            {
                if (motionNames.Contains(Scene.SkeletalAnimations[i].Name))
                {
                    targets.Add(i);
                }
            }
            return targets.ToArray();
        }
    }
}
