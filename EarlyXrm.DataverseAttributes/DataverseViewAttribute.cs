namespace EarlyXrm.DataverseAttributes;

using System;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
public class DataverseViewAttribute : Attribute
{
    public string[] Values { get; set; }

    public DataverseViewAttribute(params string[] values)
    {
        Values = values;
    }
}