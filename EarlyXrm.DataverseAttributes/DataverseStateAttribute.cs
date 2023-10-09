namespace DataverseAttributes;

using System;

public class DataverseStateAttribute : Attribute
{
    public int State { get; set; } = 0;
}