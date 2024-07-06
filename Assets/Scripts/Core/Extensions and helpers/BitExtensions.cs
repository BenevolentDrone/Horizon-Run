namespace Extensions
{
    public static class BitExtensions
    {
        public static byte UpdateBit(this byte value, byte position, bool condition)
        {
            // Update a bit at position to bitValue.
           value = condition ? value.SetBitTo1(position) : value.SetBitTo0(position);

           return value;
        }
        
        public static byte GetBitsSubSet(this byte value, byte startBit, byte endBit)
        {
            // Create a bitmask with bits from startBit to endBit set to 1
            int numberOfBits = endBit - startBit + 1;
            byte mask = (byte)((1 << numberOfBits) - 1);

            // Shift the mask to the correct position
            mask <<= startBit;

            // Apply the bitmask using a bitwise AND operation and shift the result to the right
            byte result = (byte)((value & mask) >> startBit);

            return result;
        }
        
        public static byte SetBitTo1(this byte value, byte position)
        {
            // Set a bit at position to 1.

            value = (byte)(value | (1 << position));
            
            return value;
        }

        public static byte SetBitTo0(this byte value, byte position)
        {
            // Set a bit at position to 0.
            value = (byte)(value & ~(1 << position));
            
            return value;
        }

        public static bool IsBitSetTo1(this byte value, byte position)
        {
            // Return whether bit at position is set to 1.
            return (value & (1 << position)) != 0;
        }

        public static bool IsBitSetTo0(this byte value, byte position)
        {
            // If not 1, bit is 0.
            return !IsBitSetTo1(value, position);
        }
        
        
        public static int UpdateBit(this int value, int position, bool condition)
        {
            // Update a bit at position to bitValue.
            value = condition ? value.SetBitTo1(position) : value.SetBitTo0(position);

            return value;
        }
        
        public static int SetBitTo1(this int value, int position)
        {
            // Set a bit at position to 1.
            
            return value | (1 << position);
        }

        public static int SetBitTo0(this int value, int position)
        {
            // Set a bit at position to 0.
            return value & ~(1 << position);
        }
        
        public static bool IsBitSetTo1(this int value, int position)
        {
            // Return whether bit at position is set to 1.
            return (value & (1 << position)) != 0;
        }

        public static bool IsBitSetTo0(this int value, int position)
        {
            // If not 1, bit is 0.
            return !IsBitSetTo1(value, position);
        }
    }
}