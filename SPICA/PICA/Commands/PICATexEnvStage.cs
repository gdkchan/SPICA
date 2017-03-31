namespace SPICA.PICA.Commands
{
    public class PICATexEnvStage
    {
        public PICATexEnvSource   Source;
        public PICATexEnvOperand  Operand;
        public PICATexEnvCombiner Combiner;
        public PICATexEnvColor    Color;
        public PICATexEnvScale    Scale;

        public bool UpdateColorBuffer;
        public bool UpdateAlphaBuffer;

        public bool IsColorPassThrough
        {
            get
            {
                return
                    Combiner.Color   == PICATextureCombinerMode.Replace    &&
                    Source.Color[0]  == PICATextureCombinerSource.Previous &&
                    Operand.Color[0] == PICATextureCombinerColorOp.Color   &&
                    Scale.Color      == PICATextureCombinerScale.One       &&
                    !UpdateColorBuffer;
            }
        }

        public bool IsAlphaPassThrough
        {
            get
            {
                return
                    Combiner.Alpha   == PICATextureCombinerMode.Replace    &&
                    Source.Alpha[0]  == PICATextureCombinerSource.Previous &&
                    Operand.Alpha[0] == PICATextureCombinerAlphaOp.Alpha   &&
                    Scale.Alpha      == PICATextureCombinerScale.One       &&
                    !UpdateAlphaBuffer;
            }
        }

        public static PICATexEnvStage Texture0
        {
            get
            {
                //Does TextureRGB * SecondaryColor, and TextureA is used unmodified
                //Note: This is meant to be used on the first stage
                PICATexEnvStage Output = new PICATexEnvStage();

                Output.Source.Color[0] = PICATextureCombinerSource.Texture0;
                Output.Source.Alpha[0] = PICATextureCombinerSource.Texture0;

                return Output;
            }
        }

        public static PICATexEnvStage PassThrough
        {
            get
            {
                //Does nothing, just pass the previous color down the pipeline
                PICATexEnvStage Output = new PICATexEnvStage();

                Output.Source.Color[0] = PICATextureCombinerSource.Previous;
                Output.Source.Alpha[0] = PICATextureCombinerSource.Previous;

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
