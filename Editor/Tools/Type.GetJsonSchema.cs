using System.ComponentModel;
using System.Reflection;

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace UnityReplGs.Tools
{
    [ToolComment("Generate a JSON-schema-like description for a .NET or Unity type.", "UnityReplGs.Tools.TypeGetJsonSchemaTool.GetJsonSchema(\"UnityEngine.GameObject\", true, 3)")]
    public static class TypeGetJsonSchemaTool
    {
        public static string GetJsonSchema(string typeName, bool includeDescriptions = true, int maxDepth = 3)
        {
            return JsonOut.Try(() =>
            {
                var type = ToolUtil.FindType(typeName);
                if (type == null)
                    throw new ArgumentException("Type not found: " + typeName);
                return BuildSchema(type, includeDescriptions, maxDepth, new HashSet<Type>());
            });
        }

        static object BuildSchema(Type type, bool includeDescriptions, int depth, HashSet<Type> seen)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;
            if (type == typeof(string) || type == typeof(char))
                return new Dictionary<string, object> { { "type", "string" } };
            if (type == typeof(bool))
                return new Dictionary<string, object> { { "type", "boolean" } };
            if (type.IsPrimitive && type != typeof(bool) && type != typeof(char))
                return new Dictionary<string, object> { { "type", type == typeof(float) || type == typeof(double) || type == typeof(decimal) ? "number" : "integer" } };
            if (type.IsEnum)
                return new Dictionary<string, object> { { "type", "string" }, { "enum", Enum.GetNames(type) } };
            if (type.IsArray)
                return new Dictionary<string, object> { { "type", "array" }, { "items", BuildSchema(type.GetElementType(), includeDescriptions, depth - 1, seen) } };
            if (typeof(System.Collections.IEnumerable).IsAssignableFrom(type) && type.IsGenericType)
                return new Dictionary<string, object> { { "type", "array" }, { "items", BuildSchema(type.GetGenericArguments()[0], includeDescriptions, depth - 1, seen) } };
            if (depth <= 0 || seen.Contains(type))
                return new Dictionary<string, object> { { "type", "object" }, { "title", type.FullName } };

            seen.Add(type);
            var properties = new Dictionary<string, object>();
            foreach (var member in type.GetMembers(BindingFlags.Instance | BindingFlags.Public))
            {
                Type memberType = null;
                if (member is PropertyInfo)
                {
                    var prop = (PropertyInfo)member;
                    if (prop.GetIndexParameters().Length > 0 || !prop.CanRead)
                        continue;
                    memberType = prop.PropertyType;
                }
                else if (member is FieldInfo)
                {
                    memberType = ((FieldInfo)member).FieldType;
                }
                else continue;

                var schema = BuildSchema(memberType, includeDescriptions, depth - 1, seen) as Dictionary<string, object>;
                if (schema == null)
                    continue;
                if (includeDescriptions)
                {
                    var desc = member.GetCustomAttributes(typeof(DescriptionAttribute), true).Cast<DescriptionAttribute>().FirstOrDefault();
                    if (desc != null)
                        schema["description"] = desc.Description;
                }
                properties[member.Name] = schema;
            }
            seen.Remove(type);
            return new Dictionary<string, object>
            {
                { "type", "object" },
                { "title", type.FullName },
                { "properties", properties }
            };
        }
    }
}
