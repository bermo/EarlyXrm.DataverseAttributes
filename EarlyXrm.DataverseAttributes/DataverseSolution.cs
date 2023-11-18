namespace EarlyXrm.DataverseAttributes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class DataverseSolution
{
    public static string? Prefix = null;
    public static string? Solution = null;

    public static DataverseTableAttribute DataverseTable(this Type type)
    {
        var entity = type.GetCustomAttribute<DataverseTableAttribute>() ?? new DataverseTableAttribute();

        if (entity.Prefix == null && Prefix != null) entity.Prefix = Prefix;
        if (entity.Solution == null && Solution != null) entity.Solution = Solution;

        if (entity.Name == null) entity.Name = type.Name;
        if (entity.DisplayName == null) entity.DisplayName = entity.Name.GetDisplayName();
        if (entity.DisplayCollectionName == null)
            entity.DisplayCollectionName = entity.DisplayName + "s";
        if (entity.CollectionName == null) entity.CollectionName = entity.DisplayCollectionName?.Replace(" ", "");
        if (entity.SchemaName == null)
        {
            entity.SchemaName = entity.Prefix + type.Name.Replace(" ", "");
        }

        return entity;
    }

    public static Dictionary<string, string> statusFieldMap = new()
    {
        { "Status", "StateCode" },
        { "StatusReason", "StatusCode" }
    };

    public static DataverseColumnAttribute DataverseColumn(this PropertyInfo propertyInfo)
    {
        if (propertyInfo == null)
        {
            return new DataverseColumnAttribute
            {
                Name = "Name",
                DisplayName = "Name",
                SchemaName = Prefix + "Name"
            };
        }

        var field = propertyInfo.GetCustomAttribute<DataverseColumnAttribute>() ?? new DataverseColumnAttribute();

        field.PropertyInfo = propertyInfo;

        if (field.Prefix == null && Prefix != null) field.Prefix = Prefix;
        if (field.Solution == null && Solution != null) field.Solution = Solution;

        if (field.Name == null) field.Name = propertyInfo.Name;
        if (field.DisplayName == null)
        {
            field.DisplayName = field.Name.GetDisplayName();
        }

        if (propertyInfo.PropertyType.IsGenericType)
            field.UnderlyingType = propertyInfo.PropertyType.GetGenericArguments().First();
        else if (propertyInfo.PropertyType.IsArray && propertyInfo.PropertyType != typeof(byte[]))
            field.UnderlyingType = propertyInfo.PropertyType.GetElementType();
        else
            field.UnderlyingType = propertyInfo.PropertyType;

        field.IsSimple = field.UnderlyingType.IsSimple();
        field.IsOneToMany = !field.IsManyToMany && !field.IsSimple && propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(List<>);
        field.IsManyToOne = !field.IsManyToMany && !field.IsSimple && !field.IsOneToMany && !propertyInfo.PropertyType.IsArray;

        if (field.RowSpan == default(int))
        {
            if (field.IsOneToMany || field.IsManyToMany)
                field.RowSpan = 6;
            else if (field.Multiline)
                field.RowSpan = 4;
            else if (field.IsSimple || field.IsManyToOne)
                field.RowSpan = 1;
            else // webresource (ancestor grid)?
                field.RowSpan = 10;
        }

        if (field.SchemaName == null)
        {
            if (field.IsSimple)
            {
                var builtin = new[] { "CreatedOn", "CreatedBy", "ModifiedOn", "ModifiedBy" }.Union(statusFieldMap.Select(x => x.Key));

                var match = builtin.FirstOrDefault(x => x == field.Name);
                if (match != null)
                {
                    if (statusFieldMap.ContainsKey(match))
                        field.SchemaName = statusFieldMap[match];
                    else
                        field.SchemaName = match;
                }
                else
                    field.SchemaName = field.Prefix + propertyInfo.Name;
            }
            else
            {
                var parent = propertyInfo.DeclaringType.DataverseTable();
                var underlying = field.UnderlyingType.DataverseTable();

                field.SchemaName = field.Prefix + $"{field.Name}Id";
            }

            if (field.SchemaName != null && field.SchemaName.Length > 45)
            {
                field.SchemaName = field.SchemaName.Substring(0, 45);
            }
        }

        if (field.RelationshipSchemaName == null)
        {
            var parent = propertyInfo.DeclaringType.DataverseTable();
            var underlying = field.UnderlyingType.DataverseTable();

            if (field.IsManyToMany)
            {
                field.M2MTypes = new[] { propertyInfo.DeclaringType, field.UnderlyingType }.OrderBy(x => x.Name).ToArray();
                field.RelationshipSchemaName = Prefix + string.Join("_", field.M2MTypes.Select(x => x.DataverseTable().CollectionName));
                field.SchemaName = Prefix + string.Join("", field.M2MTypes.Select(x => x.DataverseTable().CollectionName));
            }
            else if (field.IsOneToMany)
            {
                var match = field.Name.Replace(underlying.CollectionName, "");

                var underlyingProps = field.UnderlyingType.GetProperties().Where(x => x.PropertyType == propertyInfo.DeclaringType);
                if (underlyingProps.Any())
                {
                    var matchProp = underlyingProps.FirstOrDefault(x => x.Name.StartsWith(match));
                    if (matchProp != null)
                    {
                        field.RelationshipSchemaName = Prefix + matchProp.Name + "_" + field.Name;
                        field.LookupDisplayName = matchProp.Name.GetDisplayName();
                        field.LookupSchemaName = Prefix + matchProp.Name + "Id";
                    }
                    else
                    {
                        var firstProp = underlyingProps.FirstOrDefault();
                        field.RelationshipSchemaName = Prefix + firstProp.Name + "_" + field.Name;
                        field.LookupDisplayName = firstProp.Name.GetDisplayName();
                        field.LookupSchemaName = Prefix + firstProp.Name + "Id";
                    }
                }
                else
                {
                    field.RelationshipSchemaName = Prefix + parent.Name + "_" + field.Name;
                    field.LookupDisplayName = parent.Name.GetDisplayName();
                    field.LookupSchemaName = Prefix + parent.Name + "Id";
                }
            }
            else if (field.IsManyToOne)
            {
                var match = field.Name.Replace(underlying.Name, "");

                var underlyingProps = field.UnderlyingType.GetProperties().Where(x => x.PropertyType.IsGenericType && x.PropertyType.GetGenericArguments().First() == propertyInfo.DeclaringType);

                if (underlyingProps.Any())
                {
                    var matchProp = underlyingProps.FirstOrDefault(x => x.Name.StartsWith(match));
                    if (matchProp != null)
                    {
                        field.RelationshipSchemaName = Prefix + field.Name + "_" + matchProp.Name;
                    }
                    else
                    {
                        var firstProp = underlyingProps.FirstOrDefault();
                        field.RelationshipSchemaName = Prefix + field.Name + "_" + firstProp.Name;
                    }
                }
                else
                {
                    var collectionName = propertyInfo.DeclaringType.GetCustomAttribute<DataverseTableAttribute>()?.CollectionName ?? propertyInfo.DeclaringType.Name + "s";
                    field.RelationshipSchemaName = Prefix + field.Name + "_" + collectionName;
                }

                field.LookupDisplayName = field.Name.GetDisplayName();
                field.LookupSchemaName = Prefix + field.Name + "Id";
            }
        }

        return field;
    }

    public static bool IsSimple(this Type type)
    {
        return type.IsValueType ||
            type.IsPrimitive ||
            new[] {
                    typeof(string),
                    typeof(decimal),
                    typeof(DateTime),
                    typeof(DateTimeOffset),
                    typeof(TimeSpan),
                    typeof(Guid),
                    typeof(byte[])
            }.Contains(type) ||
            Convert.GetTypeCode(type) != TypeCode.Object;
    }
}