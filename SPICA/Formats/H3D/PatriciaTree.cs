using SPICA.Serialization;
using SPICA.Serialization.Serializer;
using System;
using System.Collections.Generic;
using System.IO;

namespace SPICA.Formats.H3D
{
    struct PatriciaTree : ICustomSerialization
    {
        [NonSerialized]
        public List<PatriciaTreeNode> Nodes;

        public PatriciaTreeNode this[int Index]
        {
            get { return Nodes[Index]; }
            set { Nodes[Index] = value; }
        }

        public static PatriciaTree Empty
        {
            get
            {
                PatriciaTree Tree = new PatriciaTree();

                Tree.Nodes = new List<PatriciaTreeNode>();
                Tree.Nodes.Add(new PatriciaTreeNode());

                return Tree;
            }
        }

        public void Deserialize(BinaryDeserializer Deserializer)
        {
            long Posiiton = Deserializer.BaseStream.Position + 4;

            Deserializer.BaseStream.Seek(Deserializer.Reader.ReadUInt32(), SeekOrigin.Begin);

            int MaxIndex = 0;
            int Index = 0;

            Nodes = new List<PatriciaTreeNode>();

            while (Index++ <= MaxIndex)
            {
                PatriciaTreeNode Node = Deserializer.Deserialize<PatriciaTreeNode>();

                MaxIndex = Math.Max(MaxIndex, Node.LeftNodeIndex);
                MaxIndex = Math.Max(MaxIndex, Node.RightNodeIndex);

                Nodes.Add(Node);
            }

            Deserializer.BaseStream.Seek(Posiiton, SeekOrigin.Begin);
        }

        public bool Serialize(BinarySerializer Serializer)
        {
            Serializer.Contents.Values.Add(new RefValue
            {
                Position = Serializer.BaseStream.Position,
                Value = Nodes ?? Empty.Nodes
            });

            Serializer.Skip(4);

            return true;
        }
    }
}
