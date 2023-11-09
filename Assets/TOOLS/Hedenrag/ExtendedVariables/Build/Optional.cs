using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hedenrag
{
    namespace ExVar
    {
        [Serializable]
        public struct Optional<T>
#if NET_4_6 
    : IEquatable<Optional<T>>
#endif
        {
            [SerializeField] private bool enabled;
            [SerializeField] private T value;
            public Optional(T value, bool enabled = true) : this()
            {
                this.enabled = enabled;
                this.value = value;
            }

            public Optional(Optional<T> optional) : this()
            {
                this.enabled = optional.enabled;
                this.value = optional.value;
            }

            public readonly T Value => value;
            public readonly bool IsEnabled => enabled;

            #region operators

            public override bool Equals(object obj)
            {
                return obj is Optional<T> optional && Equals(optional);
            }

            public bool Equals(Optional<T> other)
            {
                return enabled == other.enabled &&
                       EqualityComparer<T>.Default.Equals(value, other.value);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(enabled, value);
            }

            //bool
            public static bool operator true(Optional<T> optional) => optional.enabled;
            public static bool operator false(Optional<T> optional) => !optional.enabled;
            public static bool operator !(Optional<T> optional) => !optional.enabled;

#if NET_4_6
            public static bool operator ==(Optional<T> optional, Optional<T> other) { return (optional.value == (dynamic)other.value && optional.enabled == other.enabled); }
            public static bool operator !=(Optional<T> optional, Optional<T> other) { return (optional.value != (dynamic)other.value || optional.enabled != other.enabled); }
            public static bool operator ==(Optional<T> optional, T other) { return optional.value == (dynamic)other; }
            public static bool operator !=(Optional<T> optional, T other) { return optional.value != (dynamic)other; }

            //sum
            public static Optional<T> operator +(Optional<T> optional, Optional<T> other)
            {
                if (!optional) { if (other) return other; return optional; }
                if (other) { return new Optional<T>(optional.Value + (dynamic)other.Value); }
                return optional;
            }
            public static Optional<T> operator +(Optional<T> optional, T value)
            {
                if (optional.enabled) { var result = optional.value + (dynamic)value; return new Optional<T>(result, optional.enabled); }
                return new(optional);
            }
            public static T operator +(T value, Optional<T> optional)
            {
                if (optional) { return (dynamic)value + optional.Value; }
                return value;
            }

            //subtraction
            public static Optional<T> operator -(Optional<T> optional, Optional<T> other)
            {
                if (!optional) { if (other) return other; return optional; }
                if (other) { return new Optional<T>(optional.Value - (dynamic)other.Value); }
                return optional;
            }
            public static Optional<T> operator -(Optional<T> optional, T value)
            {
                if (optional) { return new Optional<T>(optional.Value - (dynamic)value, optional.enabled); }
                else return optional;
            }
            public static T operator -(T value, Optional<T> optional)
            {
                if (optional) { return value + (dynamic)optional.Value; }
                return value;
            }

            //multiply
            public static Optional<T> operator *(Optional<T> optional, Optional<T> other)
            {
                if (!optional) { if (other) return other; return optional; }
                if (other) { return new Optional<T>(optional.Value * (dynamic)other.Value); }
                return optional;
            }
            public static Optional<T> operator *(Optional<T> optional, T value)
            {
                if (optional) { return new Optional<T>(optional.value * (dynamic)value, optional.enabled); }
                return optional;
            }
            public static T operator *(T value, Optional<T> optional)
            {
                if (optional) { return value * (dynamic)optional.value; }
                return value;
            }

            //divide
            public static Optional<T> operator /(Optional<T> optional, Optional<T> other)
            {
                if (!optional) { if (other) return other; return optional; }
                if (other) { return new Optional<T>(optional.Value / (dynamic)other.Value); }
                return optional;
            }
            public static Optional<T> operator /(Optional<T> optional, T value)
            {
                if (optional) { return new Optional<T>(optional.value / (dynamic)value, optional.enabled); }
                return optional;
            }
            public static T operator /(T value, Optional<T> optional)
            {
                if (optional) { return value / (dynamic)optional.value; }
                return value;
            }

            
#endif
            #endregion
        }
    }
}