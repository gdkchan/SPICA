﻿using SPICA.Formats.H3D.Contents.Shader;
using SPICA.Serialization.BinaryAttributes;

namespace SPICA.Formats.H3D.Contents
{
    class H3DShaders
    {
        [PointerOf("PointerTable")]
        private uint PointerTableAddress;

        [CountOf("Shaders"), CountOf("NameTree", 1)]
        private uint Count;

        [PointerOf("NameTree")]
        private uint NameTreeAddress;

        [TargetSection("DescriptorsSection", 1)]
        public H3DTreeNode[] NameTree;

        [TargetSection("DescriptorsSection", 1), PointerOf("Shaders")]
        private uint[] PointerTable;

        [TargetSection("DescriptorsSection", 4)]
        public H3DShader[] Shaders;

        public H3DShader this[int Index]
        {
            get { return Shaders[Index]; }
            set { Shaders[Index] = value; }
        }
    }
}