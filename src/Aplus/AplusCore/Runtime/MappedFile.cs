using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.MemoryMappedFiles;
using AplusCore.Types;
using AplusCore.Types.MemoryMapped;
using System.IO;

namespace AplusCore.Runtime
{
    public class MappedFile
    {
        #region Variables

        private List<int> cellShape;
        private int cellLength = -1;

        private MemoryMappedFile memoryMappedFile;
        private MemoryMappedViewAccessor accessor;
        private long cursor;

        private static readonly byte TypePosition = 4;
        private static readonly byte RankPosition = 8;
        private static readonly byte ItemCountPosition = 12;
        private static readonly byte ShapePosition = 16;
        private static readonly byte LeadingAxesPosition = 52;
        private static readonly byte ItemPosition = 56;

        private static readonly int IntSize = 4;
        //private static readonly int doubleSize              = 8;

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

        #endregion

        #region Construction

        public MappedFile(MemoryMappedFile memoryMappedFile)
        {
            this.memoryMappedFile = memoryMappedFile;
            this.accessor = memoryMappedFile.CreateViewAccessor();
        }

        public static AType Read(MemoryMappedFile memoryMappedFile)
        {
            MappedFile mappedFile = new MappedFile(memoryMappedFile);

            return mappedFile.Rank > 0 ?
                MMAArray.Create(mappedFile) :
                MMAInteger.Create(ItemPosition, mappedFile);
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

        public int ReadInt32(long position)
        {
            return this.accessor.ReadInt32(position);
        }

        public void WriteInt32(long position, int value)
        {
            this.accessor.Write(position, value);
        }

        #endregion

        #region Write

        public void Create(AType argument)
        {
            WriteInt32(0,0);
            this.Type = argument.Type;
            this.Rank = argument.Rank;
            this.Shape = argument.Shape;

            int allItem = 1;

            for (int i = 0; i < argument.Shape.Count; i++)
            {
                allItem *= argument.Shape[i];
            }

            this.WriteInt32(ItemCountPosition, allItem);
            this.cursor = ItemPosition;

            this.LeadingAxes = argument.Rank > 0 ? argument.Shape[0] : 1;

            Write(argument);
        }

        private void Write(AType argument)
        {
            if (argument.Rank > 0)
            {
                foreach (AType item in argument)
                {
                    Write(item);
                }
            }
            else
            {
                WriteInt32(this.cursor, argument.asInteger);
                this.cursor += IntSize;
            }
        }


        #endregion

        #region Read

        public AType ReadCell(int index)
        {
            long position = ItemPosition + (index * this.CellShapeCount * IntSize);

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
                AType result = MMAInteger.Create(position, this);
                position += IntSize;
                return result;
            }
        }


        #endregion

        #region Add

        public void Add(AType argument)
        {
            this.cursor = this.ItemCount * IntSize + ItemPosition;

            Write(argument);

            this.Length = (argument.Rank > 1 ? argument.Length : 1) + this.Length;
        }

        #endregion
    }
}
