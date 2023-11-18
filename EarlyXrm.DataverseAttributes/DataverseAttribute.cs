namespace EarlyXrm.DataverseAttributes;

using System;

public abstract class DataverseAttribute : Attribute
{
    public string? Name { get; set; }
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public string? SchemaName { get; set; }
    public string? LogicalName { get { return SchemaName?.ToLower(); } }

    public string? Prefix { get; internal set; }
    public string? Solution { get; internal set; }
}