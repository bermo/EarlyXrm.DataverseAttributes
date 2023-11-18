namespace EarlyXrm.DataverseAttributes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
public class DataverseColumnAttribute : DataverseAttribute
{
    public bool Hide { get; set; } = false;

    public string? RelationshipSchemaName { get; set; }
    public string? LookupSchemaName { get; set; }
    public string? LookupDisplayName { get; set; }

    public string Tab { get; set; } = "";
    public string Section { get; set; } = "";
    public bool ShowSectionLabel { get; set; } = false;
    public int SectionColumn { get; set; } = 1;
    public int SectionWidth { get; set; } = 0;

    public bool IsCalculated { get; set; }
    public string? Formula { get; set; }
    public bool IsPrimary { get; set; }
    public bool ReadOnly { get; set; }
    public int Row { get; set; } = int.MaxValue;

    public int RowSpan { get; set; }
    public int Column { get; set; } = 1;
    public int ColumnSpan { get; set; } = 1;
    public bool ShowGridLabel { get; set; } = false;
    public int MaxLength { get; set; } = 200;
    public int? MinValue { get; set; }
    public int? MaxValue { get; set; }
    public int? Precision { get; set; }

    public RequiredLevel RequiredLevel { get; set; } = RequiredLevel.None;

    public bool Multiline { get; set; } = false;
    public bool IsFile { get; set; } = false;
    public bool IsImage { get; set;} = false;
    public bool IncludeTime { get; set; } = false;
    public bool IsGlobal { get; set; } = true;
    public bool BoolDefault { get; set; } = false;
    public bool IsEmail { get; set; } = false;
    public string? Autonumber { get; set; }

    public bool Collapsed { get; set; } = false;

    public bool IsStateOptionSet
    {
        get => LogicalName == "statecode";
    }

    public bool IsStatusOptionSet
    {
        get => LogicalName == "statuscode";
    }

    public bool IsEnum
    {
        get => UnderlyingType?.IsEnum == true;
    }

    public bool IsMultiEnum
    {
        get => PropertyInfo?.PropertyType.IsGenericType == true && PropertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(List<>) &&
                PropertyInfo.PropertyType.GetGenericArguments().First().IsEnum;
    }

    public bool IsGlobalOptionset
    {
        get
        {
            if (IsStateOptionSet || IsStatusOptionSet)
                return false;

            return IsGlobal;
        }
    }

    public bool IsCustom { get { return LogicalName?.StartsWith(Prefix) ?? false; } }
    public bool IsCustomRelationship
    {
        get
        {
            if (RelationshipSchemaName?.Contains("Annotation") != false)
                return false;

            return RelationshipSchemaName.StartsWith(Prefix);
        }
    }

    public bool IsOneToMany { get; internal set; }
    public bool IsManyToOne { get; internal set; }

    public bool IsManyToMany { get; set; }
    public Type[]? M2MTypes { get; internal set; }
    public string? IntersectEntity { get; set; }

    public bool IsSimple { get; internal set; }

    public Type? UnderlyingType { get; internal set; }
    public PropertyInfo? PropertyInfo { get; internal set; }

    public string? ViewName { get; set; }
    public string[]? View { get; set; }
    public int[]? ViewWidth { get; set; }
    public string? ViewOrder { get; set; }
    public bool ViewAscending { get; set; } = false;

    public string[]? PrimarySegments { get; set; }
    public string? PrimaryDescription { get; set; }

    public bool IsParental { get; set; }

    public string? FilterParent { get; set; }
    public string? FilterField { get; set; }

    public string[]? Ancestors { get; set; }
}