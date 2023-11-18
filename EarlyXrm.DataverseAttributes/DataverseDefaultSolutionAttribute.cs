namespace EarlyXrm.DataverseAttributes;

using System;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
public class DataverseDefaultSolutionAttribute : Attribute
{
    public string? Solution { get; set; }
    public string? Prefix { get; set; }
}