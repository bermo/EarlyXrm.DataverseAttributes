﻿namespace DataverseAttributes;

using System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum, AllowMultiple = false)]
public class DataverseTableAttribute : DataverseAttribute
{
    public string DisplayCollectionName { get; set; }
    public string CollectionName { get; set; }
    public bool OrganisationOwned { get; set; }
    public bool HasActivities { get; set; }
    public bool HasNotes { get; set; }
    public int BuildOrder { get; set; } = int.MinValue;
    public bool Hide { get; set; } = false;
    public bool IsCustom { get { return LogicalName.StartsWith(Prefix); } }

    public string[] Headers { get; set; }
}