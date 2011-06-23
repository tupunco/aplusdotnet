using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

using AplusCore.Types;
using AplusCore.Types.MemoryMapped;

namespace AplusCore.Runtime
{
    /// <summary>
    /// Represent a memory-mapped file.
    /// </summary>
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
        private MemoryMappedFileMode memoryMappedFileMode;

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

        private static readonly int HeaderSize = 14 * IntSize;

        #endregion

        #region Properties

        public MemoryMappedFileMode Mode
        {
            get { return this.memoryMappedFileMode; }
        }

        /// <summary>
        /// Read and set Rank in the memory-mapped file.
        /// </summary>
        public int Rank
        {
            get { return this.ReadInt32(RankPosition); }
            set { this.WriteInt32(RankPosition, value); }
        }

        /// <summary>
        /// Read and set Type.
        /// </summary>
        public ATypes Type
        {
            get { return (ATypes)this.ReadInt32(TypePosition); }
            set { this.WriteInt32(TypePosition, (int)value); }
        }

        /// <summary>
        /// Read and set Shape.
        /// </summary>
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

        /// <summary>
        /// Read the Shape without the first axes length.
        /// </summary>
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

        /// <summary>
        /// Length of the Shape without the first axes length.
        /// </summary>
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

        /// <summary>
        /// Read and set of the */Shape
        /// </summary>
        public int ItemCount
        {
            get { return this.ReadInt32(ItemCountPosition); }
            set { this.WriteInt32(ItemCountPosition, value); }
        }

        /// <summary>
        /// Read and set the leading axes size (_items modify this value)
        /// </summary>
        public int LeadingAxes
        {
            get { return this.ReadInt32(LeadingAxesPosition); }
            set { this.WriteInt32(LeadingAxesPosition, value); }
        }

        /// <summary>
        /// Read and set Length 
        /// </summary>
        public int Length
        {
            get { return ReadInt32(ShapePosition); }

            set
            {
                WriteInt32(ShapePosition, value);
                this.ItemCount = value * this.CellShapeCount;
            }
        }

        /// <summary>
        /// It gives back the Size depends on the type (AFloat,AInteger,AChar)
        /// </summary>
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

        public MappedFile(MemoryMappedFile memoryMappedFile, MemoryMappedFileMode memoryMappedFileMode = MemoryMappedFileMode.ReadAndWrite)
        {
            this.memoryMappedFile = memoryMappedFile;
            this.memoryMappedFileMode = memoryMappedFileMode;
            this.accessor = memoryMappedFileMode == MemoryMappedFileMode.LocalWrite ?
                memoryMappedFile.CreateViewAccessor(0, 0, MemoryMappedFileAccess.CopyOnWrite) : 
                memoryMappedFile.CreateViewAccessor();
        }

        #endregion

        #region Destruction

        /// <summary>
        /// Release the memory-mapped file.
        /// </summary>
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

        /// <summary>
        /// Read char from the given position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public char ReadChar(long position)
        {
            return Convert.ToChar(this.accessor.ReadByte(position));
        }

        /// <summary>
        /// Read integer from the given position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public int ReadInt32(long position)
        {
            return this.accessor.ReadInt32(position);
        }

        /// <summary>
        /// Read double from the given position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public double ReadDouble(long position)
        {
            return this.accessor.ReadDouble(position);
        }

        /// <summary>
        /// Write integer to the given position.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="value"></param>
        public void WriteInt32(long position, int value)
        {
            this.accessor.Write(position, value);
        }

        /// <summary>
        /// Write double to the given position.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="value"></param>
        public void WriteDouble(long position, double value)
        {
            this.accessor.Write(position, value);
        }

        /// <summary>
        /// Write char to the given position.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="value"></param>
        public void WriteChar(long position, char value)
        {
            this.accessor.Write(position, Convert.ToByte(value));
        }

        #endregion

        #region Write

        /// <summary>
        /// Compute the size of a memory-mapped file for argument.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public static long ComputeSize(AType argument)
        {
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

            return HeaderSize + size * AllItem(argument);
        }

        /// <summary>
        /// Write argument to the memory-mapped file.
        /// </summary>
        /// <param name="argument"></param>
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

        /// <summary>
        /// Recursive procedure to write all item to the memory-mapped file.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="position"></param>
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

        /// <summary>
        /// Write value to the position as integer.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="value"></param>
        private void WriteInteger(long position, AType value)
        {
            WriteInt32(position, value.asInteger);
        }

        /// <summary>
        /// Write value to the position as float.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="value"></param>
        private void WriteFloat(long position, AType value)
        {
            WriteDouble(position, value.asFloat);
        }

        /// <summary>
        /// Write value to the position as char.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="value"></param>
        private void WriteChar(long position, AType value)
        {
            WriteChar(position, value.asChar);
        }

        #endregion

        #region Read

        /// <summary>
        /// Bind a memory-mapped file to a MappedFile instance.
        /// </summary>
        /// <param name="memoryMappedFile"></param>
        /// <param name="memoryMappedFileMode"></param>
        /// <returns></returns>
        public static AType Read(MemoryMappedFile memoryMappedFile, MemoryMappedFileMode memoryMappedFileMode)
        {
            MappedFile mappedFile = new MappedFile(memoryMappedFile, memoryMappedFileMode);

            return mappedFile.Rank > 0 ?
                MMAArray.Create(mappedFile) :
                mappedFile.Reader(ItemPosition);
        }

        /// <summary>
        /// Create AArray cell from the index place.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public AType ReadCell(int index)
        {
            long position = ItemPosition + (index * this.CellShapeCount * this.Size);

            return CreateAArray(this.CellShape, ref position);
        }

        /// <summary>
        /// Create AArray with the given shape. 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="position"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Create MMAInteger, what is store the position (to be able to read integer from memory)
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private AType ReadMMAInteger(long position)
        {
            return MMAInteger.Create(position, this);
        }

        /// <summary>
        /// Create MMFloat, what is store the position (to be able to read float from memory)
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private AType ReadMMAFloat(long position)
        {
            return MMAFloat.Create(position, this);
        }

        /// <summary>
        /// Create MMChar, what is store the position (to be able to read char from memory)
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private AType ReadMMAChar(long position)
        {
            return MMAChar.Create(position, this);
        }

        #endregion

        #region Add

        /// <summary>
        /// Compute the new size of the memory-mapped file.
        /// </summary>
        /// <param name="newLength"></param>
        /// <returns></returns>
        public int ComputeNewSize(int newLength)
        {
            return HeaderSize + IntSize * this.CellShapeCount * newLength;
        }

        /// <summary>
        /// Append argument to the memory-mapped file.
        /// </summary>
        /// <param name="argument"></param>
        public void Add(AType argument)
        {
            if((this.Length + 1) > this.LeadingAxes)
            {
                throw new Error.Length("Not enough allocated memory");
            }

            long position = this.ItemCount * this.Size + ItemPosition;

            Write(argument, ref position);

            this.Length = (argument.Rank > 1 ? argument.Length : 1) + this.Length;
        }

        #endregion
    }
}
