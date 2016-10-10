using SPICA.Serialization;

using System;
using System.Collections.Generic;

namespace SPICA.Formats.H3D
{
    class PatriciaTree : ICustomDeserializer, ICustomSerializer
    {
        [NonSerialized]
        public List<PatriciaTreeNode> Nodes;

        public PatriciaTreeNode this[int Index]
        {
            get { return Nodes[Index]; }
            set { Nodes[Index] = value; }
        }

        public PatriciaTree()
        {
            Nodes = new List<PatriciaTreeNode>();
            Nodes.Add(new PatriciaTreeNode());
        }

        public void Deserialize(BinaryDeserializer Deserializer)
        {
            int MaxIndex = 0;
            int Index = 0;

            Nodes.Clear();

            while (Index++ <= MaxIndex)
            {
                PatriciaTreeNode Node = Deserializer.Deserialize<PatriciaTreeNode>();

                MaxIndex = Math.Max(MaxIndex, Node.LeftNodeIndex);
                MaxIndex = Math.Max(MaxIndex, Node.RightNodeIndex);

                Nodes.Add(Node);
            }
        }

        public void Serialize(BinarySerializer Serializer)
        {
            foreach (PatriciaTreeNode Node in Nodes)
            {
                Serializer.WriteObject(Node);
            }
        }
    }
}
