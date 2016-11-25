using System;

namespace SPICA.Renderer.GUI
{
    [Flags]
    public enum GUIDockMode
    {
        Center = 0,

        Top = 1,
        Left = 2,
        Bottom = 4,
        Right = 8,

        TopLeft = Top | Left,
        TopRight = Top | Right,
        BottomLeft = Bottom | Left,
        BottomRight = Bottom | Right
    }
}
