using SPICA.Formats.Common;
using SPICA.PICA;
using SPICA.PICA.Commands;

using System.IO;

namespace SPICA.Formats.GFL2.Model
{
    public class GFLUT : INamed
    {
        public PICALUTType Type;

        private float[] _Table;

        public float[] Table
        {
            get => _Table;
            set
            {
                if (value == null)
                {
                    throw Exceptions.GetNullException("Table");
                }

                if (value.Length != 256)
                {
                    throw Exceptions.GetLengthNotEqualException("Table", 256);
                }

                _Table = value;
            }
        }

        public uint HashId { get; private set; }

        private string _Name;

        public string Name {
            get => _Name;
            set
            {
                _Name = value;

                if (_Name != null)
                {
                    GFNV1 FNV = new GFNV1();

                    FNV.Hash(_Name);

                    HashId = FNV.HashCode;
                }
                else
                {
                    HashId = 0;
                }
            }
        }

        public GFLUT()
        {
            _Table = new float[256];
        }

        public GFLUT(BinaryReader Reader, int Length) : this()
        {
            HashId = Reader.ReadUInt32();

            _Name = $"LUT_{HashId:X8}";

            Reader.BaseStream.Seek(0xc, SeekOrigin.Current);

            uint[] Commands = new uint[Length >> 2];

            for (int i = 0; i < Commands.Length; i++)
            {
                Commands[i] = Reader.ReadUInt32();
            }

            uint Index = 0;

            PICACommandReader CmdReader = new PICACommandReader(Commands);

            while (CmdReader.HasCommand)
            {
                PICACommand Cmd = CmdReader.GetCommand();

                if (Cmd.Register == PICARegister.GPUREG_LIGHTING_LUT_INDEX)
                {
                    Index =               Cmd.Parameters[0] & 0xff;
                    Type  = (PICALUTType)(Cmd.Parameters[0] >> 8);
                }
                else if (
                    Cmd.Register >= PICARegister.GPUREG_LIGHTING_LUT_DATA0 &&
                    Cmd.Register <= PICARegister.GPUREG_LIGHTING_LUT_DATA7)
                {
                    foreach (uint Param in Cmd.Parameters)
                    {
                        _Table[Index++] = (Param & 0xfff) / (float)0xfff;
                    }
                }
            }
        }

        public void Write(BinaryWriter Writer)
        {
            Writer.Write(HashId);

            Writer.BaseStream.Seek(0xc, SeekOrigin.Current);

            uint[] QuantizedValues = new uint[256];

            for (int Index = 0; Index < _Table.Length; Index++)
            {
                float Difference = 0;

                if (Index < _Table.Length - 1)
                {
                    Difference = _Table[Index + 1] - _Table[Index];
                }

                int Value = (int)(_Table[Index] * 0xfff);
                int Diff  = (int)(Difference    * 0x7ff);

                QuantizedValues[Index] = (uint)(Value | (Diff << 12)) & 0xffffff;
            }

            PICACommandWriter CmdWriter = new PICACommandWriter();

            CmdWriter.SetCommand(PICARegister.GPUREG_LIGHTING_LUT_INDEX, (uint)Type << 8);
            CmdWriter.SetCommands(PICARegister.GPUREG_LIGHTING_LUT_DATA0, false, 0xf, QuantizedValues);

            CmdWriter.WriteEnd();

            uint[] Commands = CmdWriter.GetBuffer();

            foreach (uint Cmd in Commands)
            {
                Writer.Write(Cmd);
            }
        }
    }
}
