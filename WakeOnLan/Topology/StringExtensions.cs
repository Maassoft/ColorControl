
namespace System.Net.Topology
{
    internal static class StringExtensions
    {
        // TODO: The following method uses unsafe stuff. We don't want to pass the /unsafe flag to the compiler, so we use a simple, safe method instead.
        // As soon as .NET Standard with Span<T> and the new String.Create is available, we can make this faster and safe.
        /*
        internal static unsafe string Reverse(this string input)
        {
            int len = input.Length;

            // Why allocate a char[] array on the heap when you won't use it
            // outside of this method? Use the stack.
            char* reversed = stackalloc char[len];

            // Avoid bounds-checking performance penalties.
            fixed (char* str = input)
            {
                int i = 0;
                int j = i + len - 1;
                while (i < len)
                    reversed[i++] = str[j--];
            }

            // Need to use this overload for the System.String constructor
            // as providing just the char* pointer could result in garbage
            // at the end of the string (no guarantee of null terminator).
            return new string(reversed, 0, len);
        }
         */
        internal static string Reverse(this string input)
        {
            char[] chars = input.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }
    }
}
