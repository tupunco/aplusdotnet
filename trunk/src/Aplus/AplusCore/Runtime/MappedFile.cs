using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

using AplusCore.Types;
using AplusCore.Types.MemoryMapped;

namespace AplusCore.Runtime
{
    public class MappedFile
    {
        #region Variables

        delegate void WriteAType(long position, AType value);
        delegate AType ReadAType(long position);

        private List<int> cellShape;
        private int cellLength = -1;
        private ReadAType reader;
        private WriteAType writer;
        private int size = -1;

        private MemoryMappedFile memoryMappedFile;
        private MemoryMappedViewAccessor accessor;

        private static readonly byte TypePosition           = 4;
        private static readonly byte RankPosition           = 8;
        private static readonly byte ItemCountPosition      = 12;
        private static readonly byte ShapePosition          = 16;
        private static readonly byte LeadingAxesPosition    = 52;
        private static readonly byte ItemPosition = 56;

        private static readonly int ByteSize        = Marshal.SizeOf(typeof(byte));
        private static readonly int IntSize         = Marshal.SizeOf(typeof(int));
        private static readonly int DoubleSize      = Marshal.SizeOf(typeof(double));

        #endregion

        #region Properties

        public int Rank
        {
            get { return this.ReadInt32(RankPosition); }
            set { this.WriteInt32(RankPosition, value); }
        }

        public ATypes Type
        {
            get { return (ATypes)this.ReadInt32(TypePosition); }
            set { this.WriteInt32(TypePosition, (int)value); }
        }

        public List<int> Shape
        {
            get
            {
                List<int> result = new List<int>();

                result.Add(this.Length);
                result.AddRange(this.CellShape);

                return result;
            }

            set
            {
                long position = ShapePosition;

                for (int i = 0; i < 9; i++)
                {
                    this.WriteInt32(position, i < value.Count ? value[i] : 0);
                    position += IntSize;
                }
            }
        }

        private List<int> CellShape
        {
            get
            {
                if (this.cellShape == null)
                {
                    this.cellShape = new List<int>();
                    int position = ShapePosition + IntSize;

                    for (int i = 1; i < this.Rank; i++)
                    {
                        this.cellShape.Add(ReadInt32(position));
                        position += IntSize;
                    }
                }

                return this.cellShape;
            }
        }

        private int CellShapeCount
        {
            get
            {
                if (this.cellLength == -1)
                {
                    List<int> shape = this.CellShape;
                    this.cellLength = 1;

                    for (int i = 0; i < shape.Count; i++)
                    {
                        this.cellLength *= shape[i];
                    }
                }

                return this.cellLength;
            }
        }

        public int ItemCount
        {
            get { return this.ReadInt32(ItemCountPosition); }
            set { this.WriteInt32(ItemCountPosition, value); }
        }

        public int LeadingAxes
        {
            get { return this.ReadInt32(LeadingAxesPosition); }
            set { this.WriteInt32(LeadingAxesPosition, value); }
        }

        public int Length
        {
            get { return ReadInt32(ShapePosition); }

            set
            {
                WriteInt32(ShapePosition, value);
                this.ItemCount = value * this.CellShapeCount;
            }
        }

        public int Size
        {
            get
            {
                if (this.size == -1)
                {
                    switch (this.Type)
                    {
                        case ATypes.AFloat:
                            this.size = DoubleSize;
                            break;

                        case ATypes.AChar:
                            this.size = ByteSize;
                            break;
                        
                        default:
                            this.size = IntSize;
                            break;
                    }
                }

                return this.size;
            }
        }

        private ReadAType Reader
        {
            get
            {
                if (this.reader == null)
                {
                    switch (this.Type)
                    {
                        case ATypes.AFloat:
                            this.reader = new ReadAType(ReadMMAFloat);
                            break;

                        case ATypes.AChar:
                            this.reader = new ReadAType(ReadMMAChar);
                            break;

                        default:
                            this.reader = new ReadAType(ReadMMAInteger);
                            break;
                    }
                }

                return this.reader;
            }
        }

        private WriteAType Writer
        {
            get
            {
                if (this.writer == null)
                {
                    switch (this.Type)
                    {
                        case ATypes.AFloat:
                            this.writer = new WriteAType(WriteFloat);
                            break;

                        case ATypes.AChar:
                            this.writer = new WriteAType(WriteChar);
                            break;

                        default:
                            this.writer = new WriteAType(WriteInteger);
                            break;
                    }
                }

                return this.writer;
            }
        }

        #endregion

        #region Construction

        public MappedFile(MemoryMappedFile memoryMappedFile, bool localWrite = false)
        {
            this.memoryMappedFile = memoryMappedFile;
            this.accessor = localWrite ?
                memoryMappedFile.CreateViewAccessor(0, 0, MemoryMappedFileAccess.CopyOnWrite) : 
                memoryMappedFile.CreateViewAccessor();
        }

        #endregion

        #region Destruction

        public void Dispose()
        {
            this.accessor.Dispose();
            this.memoryMappedFile.Dispose();
        }

        ~MappedFile()
        {
            Dispose();
        }

        #endregion

        #region Write/Read With Accessor

        public char ReadChar(long position)
        {
            return Convert.ToChar(this.accessor.ReadByte(position));
        }

        public int ReadInt32(long position)
        {
            return this.accessor.ReadInt32(position);
        }

        public double ReadDouble(long position)
        {
            return this.accessor.ReadDouble(position);
        }

        public void WriteInt32(long position, int value)
        {
            this.accessor.Write(position, value);
        }

        public void WriteDouble(long position, double value)
        {
            this.accessor.Write(position, value);
        }

        public void WriteChar(long position, char value)
        {
            this.accessor.Write(position, Convert.ToByte(value));
        }

        #endregion

        #region Write

        public static long ComputeSize(AType argument)
        {
           //Header
            long result = 14 * IntSize;

            int size;

            switch (argument.Type)
            {
                case ATypes.AChar:
                    size = ByteSize;
                    break;
                case ATypes.AFloat:
                    size = DoubleSize;
                    break;
                default:
                    size = IntSize;
                    break;
            }

            result += (size * AllItem(argument));

            return result;
        }

        public void Create(AType argument)
        {
            WriteInt32(0,0);
            this.Type = argument.Type;
            this.Rank = argument.Rank;
            this.Shape = argument.Shape;

            this.WriteInt32(ItemCountPosition, AllItem(argument));

            this.LeadingAxes = argument.Rank > 0 ? argument.Shape[0] : 1;

            long position = ItemPosition;

            Write(argument, ref position);
        }

        private static int AllItem(AType argument)
        {
            int allItem = 1;

            for (int i = 0; i < argument.Rank; i++)
            {
                allItem *= argument.Shape[i];
            }

            return allItem;
        }

        private void Write(AType argument, ref long position)
        {
            if (argument.Rank > 0)
            {
                foreach (AType item in argument)
                {
                    Write(item, ref position);
                }
            }
            else
            {
                this.Writer(position, argument);
                position += this.Size;
            }
        }

        private void WriteInteger(long position, AType value)
        {
            WriteInt32(position, value.asInteger);
        }

        private void WriteFloat(long position, AType value)
        {
            WriteDouble(position, value.asFloat);
        }

        private void WriteChar(long position, AType value)
        {
            WriteChar(position, value.asChar);
        }

        #endregion

        #region Read

        public static AType Read(MemoryMappedFile memoryMappedFile, bool localWrite)
        {
            MappedFile mappedFile = new MappedFile(memoryMappedFile, localWrite);

            return mappedFile.Rank > 0 ?
                MMAArray.Create(mappedFile) :
                mappedFile.Reader(ItemPosition);
        }

        public AType ReadCell(int index)
        {
            long position = ItemPosition + (index * this.CellShapeCount * this.Size);

            return CreateAArray(this.CellShape, ref position);
        }

        private AType CreateAArray(List<int> shape, ref long position)
        {
            if (shape.Count > 0)
            {
                List<int> cuttedShape = shape.GetRange(1, shape.Count - 1);

                AType result = AArray.Create(this.Type);

                for (int i = 0; i < shape[0]; i++)
                {
                    result.AddWithNoUpdate(CreateAArray(cuttedShape, ref position));
                }

                result.UpdateInfo();

                return result;
            }
            else
            {
                AType result = this.Reader(position);
                position += this.Size;
                return result;
            }
        }

        private AType ReadMMAInteger(long position)
        {
            return MMAInteger.Create(position, this);
        }

        private AType ReadMMAFloat(long position)
        {
            return MMAFloat.Create(position, this);
        }

        private AType ReadMMAChar(long position)
        {
            return MMAChar.Create(position, this);
        }

        #endregion

        #region Add

        public void Add(AType argument)
        {
            long position = this.ItemCount * this.Size + ItemPosition;

            Write(argument, ref position);

            this.Length = (argument.Rank > 1 ? argument.Length : 1) + this.Length;
        }

        #endregion
    }
}
