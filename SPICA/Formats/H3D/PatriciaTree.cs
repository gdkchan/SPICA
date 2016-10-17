using SPICA.Serialization;
using SPICA.Serialization.Attributes;
using SPICA.Serialization.Serializer;

using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;

namespace SPICA.Formats.H3D
{
    class PatriciaTree : ICustomSerialization, IEnumerable<PatriciaTreeNode>
    {
        [NonSerialized]
        private List<PatriciaTreeNode> Nodes;

        [NonSerialized]
        private List<string> Names;

        public PatriciaTreeNode this[int Index]
        {
            get { return Nodes[Index]; }
            set { Nodes[Index] = value; }
        }

        public int MaxLength
        {
            get
            {
                int Max = 0;

                foreach (string Name in Names)
                {
                    if (Name.Length > Max) Max = Name.Length;
                }

                return Max;
            }
        }

        public int Count { get { return Nodes.Count; } }

        private const string DuplicateKeysEx = "Tree shouldn't contain duplicate keys!";

        public PatriciaTree()
        {
            Nodes = new List<PatriciaTreeNode>();
            Nodes.Add(new PatriciaTreeNode());

            Names = new List<string>();
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

                if (Node.Name != null) Names.Add(Node.Name);
            }
        }

        public bool Serialize(BinarySerializer Serializer)
        {
            Serializer.WriteValue(Nodes);

            return true;
        }

        public IEnumerator<PatriciaTreeNode> GetEnumerator()
        {
            return Nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        //Implementation
        public string Find(int Index)
        {
            return Nodes[Index + 1].Name;
        }

        public int Find(string Name)
        {
            int Output = 0;

            if (Nodes != null && Nodes.Count > 0)
            {
                PatriciaTreeNode Root;

                Output = Traverse(Name, out Root);

                if (Nodes[Output].Name != Name) Output = 0;
            }

            return Output - 1;
        }

        public void Add(string Name)
        {
            Names.Add(Name);
            RebuildTree();
        }

        public void Insert(int Index, string Name)
        {
            Names.Insert(Index, Name);
            RebuildTree();
        }

        public void Remove(string Name)
        {
            Names.Remove(Name);
            RebuildTree();
        }

        public void Clear()
        {
            Names.Clear();
            RebuildTree();
        }

        private void RebuildTree()
        {
            Nodes.Clear();

            if (Names.Count > 0)
                Nodes.Add(new PatriciaTreeNode { ReferenceBit = uint.MaxValue });
            else
                Nodes.Add(new PatriciaTreeNode());

            foreach (string Name in Names) Insert(Name);
        }

        private void Insert(string Name)
        {
            if (Name == null) return;

            PatriciaTreeNode New = new PatriciaTreeNode();
            PatriciaTreeNode Root;

            uint Bit = (uint)((MaxLength << 3) - 1);
            int Index = Traverse(Name, out Root);

            while (GetBit(Nodes[Index].Name, Bit) == GetBit(Name, Bit))
            {
                if (--Bit == uint.MaxValue) throw new InvalidOperationException(DuplicateKeysEx);
            }

            New.ReferenceBit = Bit;

            if (GetBit(Name, Bit))
            {
                New.LeftNodeIndex = (ushort)Traverse(Name, out Root, Bit);
                New.RightNodeIndex = (ushort)Nodes.Count;
            }
            else
            {
                New.LeftNodeIndex = (ushort)Nodes.Count;
                New.RightNodeIndex = (ushort)Traverse(Name, out Root, Bit);
            }

            New.Name = Name;

            int RootIndex = Nodes.IndexOf(Root);

            if (GetBit(Name, Root.ReferenceBit))
                Root.RightNodeIndex = (ushort)Nodes.Count;
            else
                Root.LeftNodeIndex = (ushort)Nodes.Count;

            Nodes.Add(New);

            Nodes[RootIndex] = Root;
        }

        private int Traverse(string Name, out PatriciaTreeNode Root, uint Bit = 0)
        {
            Root = Nodes[0];

            int Output = Root.LeftNodeIndex;

            PatriciaTreeNode Left = Nodes[Output];

            while (Root.ReferenceBit > Left.ReferenceBit && Left.ReferenceBit > Bit)
            {
                if (GetBit(Name, Left.ReferenceBit))
                    Output = Left.RightNodeIndex;
                else
                    Output = Left.LeftNodeIndex;

                Root = Left;
                Left = Nodes[Output];
            }

            return Output;
        }

        private bool GetBit(string Name, uint Bit)
        {
            int Position = (int)(Bit >> 3);
            int CharBit = (int)(Bit & 7);

            if (Name != null && Position < Name.Length)
                return ((Name[Position] >> CharBit) & 1) != 0;
            else
                return false;
        }
    }
}
