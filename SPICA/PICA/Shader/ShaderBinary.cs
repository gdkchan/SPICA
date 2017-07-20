using SPICA.Formats.Common;
using SPICA.PICA.Commands;

using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace SPICA.PICA.Shader
{
    public class ShaderBinary
    {
        public uint[]  Executable;
        public ulong[] Swizzles;

        public readonly List<ShaderProgram> Programs;

        public ShaderBinary()
        {
            Programs = new List<ShaderProgram>();
        }

        public ShaderBinary(byte[] Data) : this()
        {
            using (MemoryStream MS = new MemoryStream(Data))
            {
                BinaryReader Reader = new BinaryReader(MS);

                uint DVLBMagicNumber = Reader.ReadUInt32();
                uint DVLECount       = Reader.ReadUInt32();

                MS.Seek(DVLECount * 4, SeekOrigin.Current);

                uint DVLPPosition = (uint)MS.Position;

                uint   DVLPMagicNumber      = Reader.ReadUInt32();
                ushort DVLPVersion          = Reader.ReadUInt16();
                byte   ShaderType           = Reader.ReadByte();
                byte   OutRegsInfo          = Reader.ReadByte();
                uint   ShaderBinaryAddress  = Reader.ReadUInt32() + DVLPPosition;
                uint   ShaderBinaryCount    = Reader.ReadUInt32();
                uint   SwizzlesAddress      = Reader.ReadUInt32() + DVLPPosition;
                uint   SwizzlesCount        = Reader.ReadUInt32();
                uint   FileNamesPtrsAddress = Reader.ReadUInt32() + DVLPPosition;

                for (int i = 0; i < DVLECount; i++)
                {
                    Programs.Add(new ShaderProgram());

                    MS.Seek(8 + i * 4, SeekOrigin.Begin);
                    MS.Seek(Reader.ReadUInt32(), SeekOrigin.Begin);

                    uint DVLEPosition = (uint)MS.Position;

                    uint   DVLEMagic        = Reader.ReadUInt32();
                    ushort DVLEVersion      = Reader.ReadUInt16();
                    byte   IsGeoShader      = Reader.ReadByte();
                    byte   DebugFlags       = Reader.ReadByte();
                    uint   ExeStartOffset   = Reader.ReadUInt32();
                    uint   ExeEndOffset     = Reader.ReadUInt32();
                    ushort InputMask        = Reader.ReadUInt16();
                    ushort OutputMask       = Reader.ReadUInt16();
                    byte   GeoShaderType    = Reader.ReadByte();
                    byte   GeoShaderIndex   = Reader.ReadByte();
                    byte   GeoSubDivSize    = Reader.ReadByte();
                    byte   GeoVertexCount   = Reader.ReadByte();
                    uint   ConstTblOffset   = Reader.ReadUInt32() + DVLEPosition;
                    uint   ConstTblCount    = Reader.ReadUInt32();
                    uint   LabelTblOffset   = Reader.ReadUInt32() + DVLEPosition;
                    uint   LabelTblCount    = Reader.ReadUInt32();
                    uint   OutRegTblOffset  = Reader.ReadUInt32() + DVLEPosition;
                    uint   OutRegTblCount   = Reader.ReadUInt32();
                    uint   UniformTblOffset = Reader.ReadUInt32() + DVLEPosition;
                    uint   UniformTblCount  = Reader.ReadUInt32();
                    uint   StringsTblOffset = Reader.ReadUInt32() + DVLEPosition;
                    uint   StringsTblLength = Reader.ReadUInt32();

                    Programs[i].IsGeometryShader = IsGeoShader != 0;

                    Programs[i].MainOffset    = ExeStartOffset;
                    Programs[i].EndMainOffset = ExeEndOffset;

                    for (int ci = 0; ci < ConstTblCount; ci++)
                    {
                        MS.Seek(ConstTblOffset + ci * 0x14, SeekOrigin.Begin);

                        byte Type = (byte)Reader.ReadUInt16();
                        byte Reg  = (byte)Reader.ReadUInt16();

                        ShaderUniform Uniform = GetUniform(Programs[i], Reg, Type);

                        Uniform.IsConstant = true;

                        switch (Type)
                        {
                            case 0: //Boolean
                                ((ShaderUniformBool)Uniform).Constant = Reader.ReadByte() != 0;
                                break;

                            case 1: //Integer Vector4
                                ((ShaderUniformVec4)Uniform).Constant = ReadIVec4(Reader);
                                break;

                            case 2: //Float Vector4
                                ((ShaderUniformVec4)Uniform).Constant = ReadVec4(Reader);
                                break;
                        }
                    }

                    for (int li = 0; li < LabelTblCount; li++)
                    {
                        MS.Seek(LabelTblOffset + li * 0x10, SeekOrigin.Begin);

                        uint Id     = Reader.ReadUInt32();
                        uint Offset = Reader.ReadUInt32();
                        uint Length = Reader.ReadUInt32();

                        MS.Seek(StringsTblOffset + Reader.ReadUInt32(), SeekOrigin.Begin);

                        string Name = Reader.ReadNullTerminatedString();

                        Programs[i].Labels.Add(new ShaderLabel()
                        {
                            Id     = Id,
                            Offset = Offset,
                            Length = Length,
                            Name   = Name
                        });
                    }

                    for (int oi = 0; oi < OutRegTblCount; oi++)
                    {
                        MS.Seek(OutRegTblOffset + oi * 8, SeekOrigin.Begin);

                        ulong Value = Reader.ReadUInt64();

                        int RegId = RegId = (int)(Value >> 16) & 0xf;

                        Programs[i].OutputRegs[RegId] = new ShaderOutputReg()
                        {
                            Name = (ShaderOutputRegName)(Value & 0xf),
                            Mask = (uint)(Value >> 32) & 0xf
                        };
                    }

                    for (int ui = 0; ui < UniformTblCount; ui++)
                    {
                        MS.Seek(UniformTblOffset + ui * 8, SeekOrigin.Begin);

                        int    NameOffset = Reader.ReadInt32();
                        ushort StartReg   = Reader.ReadUInt16();
                        ushort EndReg     = Reader.ReadUInt16();

                        MS.Seek(StringsTblOffset + NameOffset, SeekOrigin.Begin);

                        string Name = Reader.ReadNullTerminatedString();

                        for (int r = StartReg; r <= EndReg; r++)
                        {
                            if (r < 0x10)
                            {
                                Programs[i].InputRegs[r] = Name;
                            }
                            else
                            {
                                ShaderUniform Uniform = GetUniform(Programs[i], r);

                                if (Uniform != null)
                                {
                                    Uniform.Name        = Name;
                                    Uniform.IsArray     = EndReg != StartReg;
                                    Uniform.ArrayIndex  = r - StartReg;
                                    Uniform.ArrayLength = (EndReg - StartReg) + 1;
                                }
                            }
                        }
                    }
                }

                MS.Seek(ShaderBinaryAddress, SeekOrigin.Begin);

                Executable = new uint[ShaderBinaryCount];

                for (int i = 0; i < Executable.Length; i++)
                {
                    Executable[i] = Reader.ReadUInt32();
                }

                Swizzles = new ulong[SwizzlesCount];

                MS.Seek(SwizzlesAddress, SeekOrigin.Begin);

                for (int i = 0; i < SwizzlesCount; i++)
                {
                    Swizzles[i] = Reader.ReadUInt64();
                }
            }
        }

        private ShaderUniform GetUniform(ShaderProgram Program, int Reg, int Type)
        {
            switch (Type)
            {
                case 0: return Program.BoolUniforms[Reg];
                case 1: return Program.IVec4Uniforms[Reg];
                case 2: return Program.Vec4Uniforms[Reg];
            }

            throw new ArgumentOutOfRangeException(nameof(Type));
        }

        private ShaderUniform GetUniform(ShaderProgram Program, int Reg)
        {
            if (Reg < 0x70)
            {
                return Program.Vec4Uniforms[Reg - 0x10];
            }
            else if (Reg < 0x74)
            {
                return Program.IVec4Uniforms[Reg - 0x70];
            }
            else if (Reg >= 0x78 && Reg < 0x88)
            {
                return Program.BoolUniforms[Reg - 0x78];
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(Reg));
            }
        }

        private Vector4 ReadIVec4(BinaryReader Reader)
        {
            return new Vector4(
                Reader.ReadByte(),
                Reader.ReadByte(),
                Reader.ReadByte(),
                Reader.ReadByte());
        }

        private Vector4 ReadVec4(BinaryReader Reader)
        {
            return new Vector4(
                PICAVectorFloat24.GetFloat24(Reader.ReadUInt32()),
                PICAVectorFloat24.GetFloat24(Reader.ReadUInt32()),
                PICAVectorFloat24.GetFloat24(Reader.ReadUInt32()),
                PICAVectorFloat24.GetFloat24(Reader.ReadUInt32()));
        }

        public void GetSwizzles(
            uint         DescIdx,
            out string   SDst,
            out string[] SSrc,
            out string[] SSrcM)
        {
            SSrc  = new string[3];
            SSrcM = new string[3];

            ulong Swizzle = Swizzles[DescIdx];

            SSrc[0] = GetSwizzle((byte)(Swizzle >>  5));
            SSrc[1] = GetSwizzle((byte)(Swizzle >> 14));
            SSrc[2] = GetSwizzle((byte)(Swizzle >> 23));

            uint Mask = (uint)Swizzle & 0xf;

            SDst = MaskSwizzle("xyzw", Mask);

            SSrcM[0] = MaskSwizzle(SSrc[0], Mask);
            SSrcM[1] = MaskSwizzle(SSrc[1], Mask);
            SSrcM[2] = MaskSwizzle(SSrc[2], Mask);
        }

        public string[] GetSrcSigns(uint DescIdx, bool HidePlus = true)
        {
            string[] Output = new string[3];

            ulong Swizzle = Swizzles[DescIdx];

            string Plus = HidePlus ? string.Empty : "+";

            Output[0] = ((Swizzle >>  4) & 1) != 0 ? "-" : Plus;
            Output[1] = ((Swizzle >> 13) & 1) != 0 ? "-" : Plus;
            Output[2] = ((Swizzle >> 22) & 1) != 0 ? "-" : Plus;

            return Output;
        }

        private string MaskSwizzle(string Swizzle, uint Mask)
        {
            string Output = string.Empty;

            if ((Mask & 8) != 0) Output += Swizzle[0];
            if ((Mask & 4) != 0) Output += Swizzle[1];
            if ((Mask & 2) != 0) Output += Swizzle[2];
            if ((Mask & 1) != 0) Output += Swizzle[3];

            return Output;
        }

        private string GetSwizzle(byte Components)
        {
            const string Comps = "xyzw";

            string Output = string.Empty;

            for (int i = 0; i < 8; i += 2)
            {
                Output = Comps[(Components >> i) & 3] + Output;
            }

            return Output;
        }
    }
}
