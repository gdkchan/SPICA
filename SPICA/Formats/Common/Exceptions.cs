using System;

namespace SPICA.Formats.Common
{
    static class Exceptions
    {
        public static Exception GetLessThanException(string Name, int Min)
        {
            return new ArgumentOutOfRangeException(Name, $"Value on {Name} is less than the minimum value {Min}!");
        }

        public static Exception GetGreaterThanException(string Name, int Max)
        {
            return new ArgumentOutOfRangeException(Name, $"Value on {Name} is greater than the maximum value {Max}!");
        }

        public static Exception GetLengthNotEqualException(string Name, int Length)
        {
            return new ArgumentOutOfRangeException(Name, $"Array {Name} length should be equal to {Length}!");
        }

        public static Exception GetTypeException(string Name, string Type)
        {
            return new ArgumentException(Name, $"The type {Type} is not valid for {Name}!");
        }

        public static Exception GetNullException(string Name)
        {
            return new ArgumentNullException(Name);
        }
    }
}
