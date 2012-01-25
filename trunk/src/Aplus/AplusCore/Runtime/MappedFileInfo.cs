using System.Runtime.InteropServices;

namespace AplusCore.Runtime
{
    internal static class MappedFileInfo
    {
        public static readonly byte TypePosition = 4;
        public static readonly byte RankPosition = 8;
        public static readonly byte ItemCountPosition = 12;
        public static readonly byte ShapePosition = 16;
        public static readonly byte LeadingAxesPosition = 52;

        public static readonly int ByteSize = Marshal.SizeOf(typeof(byte));
        public static readonly int IntSize = Marshal.SizeOf(typeof(int));
        public static readonly int DoubleSize = Marshal.SizeOf(typeof(double));

        public static readonly int HeaderSize = 14 * IntSize;
    }
}
