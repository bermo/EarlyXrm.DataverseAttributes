namespace DataverseAttributes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

internal static class Extensions
{
    public static string GetDisplayName(this string input)
    {
        if (input == null) return null;

        return string.Concat(input.Select(x => char.IsUpper(x) || char.IsDigit(x) ? " " + x : x.ToString())).TrimStart(' ');
    }
}