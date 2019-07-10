using System;
using System.IO;
using System.Linq;
using System.Dynamic;
using Mono.Options;
using SPICA.Formats.CtrH3D;
using YamlDotNet.Serialization;
using System.Collections.Generic;


namespace SPICA.CLI
{
    class Program
    {
        private static OptionSet options;

        static void Main(string[] args)
        {
            string here = AppDomain.CurrentDomain.BaseDirectory;

            Console.WriteLine("SPICA CLI");

            //default args
            string Pokemon = "bulbasaur";
            string MapLocation = $"{here}/data/model_bin_map.yml";
            string BinLocation = $"{here}/in";
            string TextureLocation = $"{here}/tex";
            string ModelLocation = $"{here}/out";

            options = new OptionSet() {
                { "?|h|help", "show the help", _ => HelpText()},
                { "p=|pokemon=", "desired pokemon to dump", input => Pokemon = input},
                { "map=|map-file=", "", input => MapLocation = input},
                { "in=|bin=|bin-dir=", "", input => BinLocation = input},
                { "tex=|texture=|texture-out=", "", input => TextureLocation = input},
                { "model=|model-out", "", input => ModelLocation = input},
            };

            var errors = options.Parse(args);

            if (errors.Count > 0) {
                foreach (var err in errors) {
                    Console.Error.WriteLine("Unrecognized option {0}", err);
                }
                Environment.Exit(1);
            }

            Directory.CreateDirectory(TextureLocation);
            Directory.CreateDirectory(ModelLocation);

            Console.WriteLine($"Searching for: {Pokemon}");
            string[] files = GetFileNames(Pokemon, MapLocation).Select(file => $"{BinLocation}/{file}").ToArray();

            Console.Write($"Building Scene with files:\n{string.Join("\n", files)}\n");
            H3D Scene = FileIO.Merge(files);

            Console.WriteLine($"Exporting {Scene.Textures.Count} textures");
            FileIO.ExportTextures(Scene, TextureLocation);

            int[] motions = GetMotionIndices(Scene, MotionLexicon.StandardMotion.Values);

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

        private static string[] GetFileNames(string Pokemon, string MapLocation) {
            var yaml = new DeserializerBuilder().Build();
            dynamic expando = yaml.Deserialize<ExpandoObject>(File.ReadAllText(MapLocation));
            var lookup = expando as IDictionary<string, Object>;

            List<string> files = new List<string>();
            foreach (var file in (lookup[Pokemon] as List<Object>))
                files.Add(file.ToString());

            return files.ToArray();
        }

        private static void HelpText() {
            options.WriteOptionDescriptions(Console.Out);
            Console.WriteLine("SPICA Command Line Interface");

            Environment.Exit(1);
        }
    }
}
