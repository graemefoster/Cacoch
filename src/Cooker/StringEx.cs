using System;

namespace Cooker
{
    public static class StringEx
    {
        public static string AtMost(this string str, int length)
        {
            return str.Substring(0, Math.Min(length, str.Length));
        }
    }
}