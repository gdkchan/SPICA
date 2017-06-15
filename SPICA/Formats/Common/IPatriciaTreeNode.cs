using System;
using System.Collections.Generic;
using System.Text;

namespace SPICA.Formats.Common
{
    interface IPatriciaTreeNode
    {
        uint   ReferenceBit   { get; set; }
        ushort LeftNodeIndex  { get; set; }
        ushort RightNodeIndex { get; set; }
        string Name           { get; set; }
    }
}
