using System.Collections.Generic;

namespace AplusCore.Runtime.Function.ADAP
{
    static class CDRConstants
    {
        #region Constants

        public const byte CDRFlag = 0x82; // by default 80, +2 for little endian (more: impexp.c 774)

        public static readonly List<short> IntegerTypes = new List<short>(){
                                                             0x4201, // B1
                                                             0x4204, // B4
                                                             0x4208, // B8
                                                             0x4902, // I2
                                                             0x4904, // I4
                                                            };

        public static readonly List<short> FloatTypes = new List<short>(){ 
                                                                   0x4504, // E4
                                                                   0x4508, // E8
                                                                  };

        public const short CDRCharShort = 0x4301; // C1
        public const short CDRBoxShort = 0x4700; // G0
        public const short CDRSymShort = 0x5301; // S1

        public static readonly byte[] CDRInt = { 0x49, 0x04 }; // I4
        public static readonly byte[] CDRChar = { 0x43, 0x01 }; // C1
        public static readonly byte[] CDRFloat = { 0x45, 0x08 }; // E8
        public static readonly byte[] CDRBox = { 0x47, 0x00 }; // G0
        public static readonly byte[] CDRSym = { 0x53, 0x01 }; // S1

        #endregion
    }
}
