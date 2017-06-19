using SPICA.Formats.Common;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;

using System.Collections;
using System.Collections.Generic;

namespace SPICA.Formats.CtrGfx
{
    class GfxDictionary<T> : ICustomSerialization, IEnumerable<T> where T : INamed
    {
        [Ignore] private List<GfxDictionaryNode<T>> Nodes;
        [Ignore] private List<T>                    Values;

        [Ignore] private bool TreeNeedsRebuild;

        public int Count { get { return Values.Count; } }

        public T this[int Index]
        {
            get
            {
                return Values[Index];
            }
            set
            {
                Values[Index] = value;

                TreeNeedsRebuild = true;
            }
        }

        public GfxDictionary()
        {
            Nodes  = new List<GfxDictionaryNode<T>> { new GfxDictionaryNode<T>() };
            Values = new List<T>();
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            uint MagicNumber = Deserializer.Reader.ReadUInt32();
            uint TreeLength  = Deserializer.Reader.ReadUInt32();
            uint ValuesCount = Deserializer.Reader.ReadUInt32();

            Nodes.Clear();

            for (int i = 0; i < ValuesCount + 1; i++)
            {
                GfxDictionaryNode<T> Node = Deserializer.Deserialize<GfxDictionaryNode<T>>();

                if (Nodes.Count > 0) Values.Add(Node.Value);

                Nodes.Add(Node);
            }
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            if (TreeNeedsRebuild) RebuildTree();

            Serializer.Writer.Write(IOUtils.ToUInt32("DICT"));
            Serializer.Writer.Write(Nodes.Count * 0x10 + 0xc);
            Serializer.Writer.Write(Values.Count);

            Serializer.WriteValue(Nodes);

            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        //Implementation
        public bool Contains(string Name)
        {
            return Find(Name) != -1;
        }

        public int Find(string Name)
        {
            if (Name == null) return -1;

            if (TreeNeedsRebuild) RebuildTree();

            int Output = 0;

            if (Nodes != null && Nodes.Count > 0)
            {
                Output = PatriciaTree.Traverse(Name, Nodes, out GfxDictionaryNode<T> Root);

                if (Nodes[Output].Name != Name) Output = 0;
            }

            return Output - 1;
        }

        public void Add(T Value)
        {
            Values.Add(Value);

            TreeNeedsRebuild = true;
        }

        public void Insert(int Index, T Value)
        {
            Values.Insert(Index, Value);

            TreeNeedsRebuild = true;
        }

        public void Remove(T Value)
        {
            Values.Remove(Value);

            TreeNeedsRebuild = true;
        }

        public void Clear()
        {
            Values.Clear();

            TreeNeedsRebuild = true;
        }

        private void RebuildTree()
        {
            Nodes.Clear();

            if (Values.Count > 0)
                Nodes.Add(new GfxDictionaryNode<T> { ReferenceBit = uint.MaxValue });
            else
                Nodes.Add(new GfxDictionaryNode<T>());

            int MaxLength = 0;

            foreach (T Value in Values)
            {
                if (MaxLength < Value.Name.Length)
                    MaxLength = Value.Name.Length;
            }

            foreach (T Value in Values)
            {
                GfxDictionaryNode<T> Node = new GfxDictionaryNode<T>
                {
                    Name  = Value.Name,
                    Value = Value
                };

                PatriciaTree.Insert(Nodes, Node, MaxLength);
            }

            TreeNeedsRebuild = false;
        }
    }
}
