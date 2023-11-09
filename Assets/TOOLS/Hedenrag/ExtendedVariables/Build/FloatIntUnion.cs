using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Hedenrag
{
    namespace ExVar
    {
        [StructLayout(LayoutKind.Explicit)]
        public struct FloatIntUnion
        {
            [FieldOffset(0)]
            public UInt32 UInt32Bits;
            [FieldOffset(0)]
            public float FloatValue;
            [FieldOffset(0)]
            public Int32 IntValue;
            public bool IntBool { get { return UInt32Bits == 0; } }

            public FloatIntUnion(float value)
            {
                UInt32Bits = 0;
                IntValue = 0;
                FloatValue = value;
            }

            public FloatIntUnion(uint value)
            {
                FloatValue = 0f;
                IntValue = 0;
                UInt32Bits = value;
            }

            public FloatIntUnion(int value)
            {
                FloatValue = 0f;
                UInt32Bits = 0;
                IntValue = value;
            }

            public static string GetByteCode(uint num)
            {
                return new FloatIntUnion(num).ByteCode;
            }
            public static string GetByteCode(float num)
            {
                return new FloatIntUnion(num).ByteCode;
            }
            public static string GetByteCode(int num)
            {
                return new FloatIntUnion(num).ByteCode;
            }
            public string ByteCode
            {
                get
                {
                    return Convert.ToString(UInt32Bits, 2);
                }
            }

            public static implicit operator FloatIntUnion(float f) => new FloatIntUnion(f);
            public static implicit operator FloatIntUnion(int i) => new FloatIntUnion(i);
            public static implicit operator FloatIntUnion(uint i) => new FloatIntUnion(i);
            public static implicit operator float(FloatIntUnion fIU) => fIU.FloatValue;
            public static implicit operator int(FloatIntUnion fIU) => fIU.IntValue;
            public static implicit operator uint(FloatIntUnion fIU) => fIU.UInt32Bits;
            public static explicit operator bool(FloatIntUnion fIU) => fIU.IntBool;
        }
    }
}