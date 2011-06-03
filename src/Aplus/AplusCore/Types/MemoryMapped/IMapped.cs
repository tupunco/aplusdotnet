using AplusCore.Runtime;

namespace AplusCore.Types.MemoryMapped
{
    interface IMapped
    {
        void Update(AType value);
        MemoryMappedFileMode Mode { get; }
    }
}
