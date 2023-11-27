namespace EarlyXrm.DataverseAttributes;

using System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
public class DataverseTableAttribute : DataverseAttribute
{
    public string? DisplayCollectionName { get; set; }
    public string? CollectionName { get; set; }
    public bool OrganisationOwned { get; set; }
    public bool HasActivities { get; set; }
    public bool HasNotes { get; set; }
    public int BuildOrder { get; set; } = int.MinValue;
    public bool Hide { get; set; } = false;
    public bool IsCustom { get { return LogicalName?.StartsWith(Prefix) ?? false; } }

    public string[]? Headers { get; set; }
}