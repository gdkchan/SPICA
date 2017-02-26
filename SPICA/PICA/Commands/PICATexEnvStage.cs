namespace SPICA.PICA.Commands
{
    public class PICATexEnvStage
    {
        public PICATexEnvSource   Source;
        public PICATexEnvOperand  Operand;
        public PICATexEnvCombiner Combiner;
        public PICATexEnvColor    Color;
        public PICATexEnvScale    Scale;

        public bool UpdateRGBBuffer;
        public bool UpdateAlphaBuffer;

        public static PICATexEnvStage Texture0SubSecColor
        {
            get
            {
                //Does TextureRGB * SecondaryColor, and TextureA is used unmodified
                //Note: This is meant to be used on the first stage
                PICATexEnvStage Output = new PICATexEnvStage();

                Output.Source.ColorSource[0] = PICATextureCombinerSource.Texture0;
                Output.Source.ColorSource[1] = PICATextureCombinerSource.FragmentSecondaryColor;
                Output.Source.AlphaSource[0] = PICATextureCombinerSource.Texture0;

                Output.Combiner.ColorCombiner = PICATextureCombinerMode.Subtract;
                Output.Combiner.AlphaCombiner = PICATextureCombinerMode.Replace;

                return Output;
            }
        }

        public static PICATexEnvStage PassThrough
        {
            get
            {
                //Does nothing, just pass the previous color down the pipeline
                PICATexEnvStage Output = new PICATexEnvStage();

                Output.Source.ColorSource[0] = PICATextureCombinerSource.Previous;
                Output.Source.AlphaSource[0] = PICATextureCombinerSource.Previous;

                return Output;
            }
        }

        public PICATexEnvStage()
        {
            Source   = new PICATexEnvSource();
            Operand  = new PICATexEnvOperand();
            Combiner = new PICATexEnvCombiner();
            Color    = new PICATexEnvColor();
            Scale    = new PICATexEnvScale();
        }
    }
}
