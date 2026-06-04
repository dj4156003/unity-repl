using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace UnityReplGs.Tools
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ToolCommentAttribute : Attribute
    {
        public string Summary { get; private set; }
        public string[] Examples { get; private set; }

        public ToolCommentAttribute(string summary, params string[] examples)
        {
            Summary = summary;
            Examples = examples ?? new string[0];
        }
    }

    [ToolComment(
        "Inspect the compiled UnityREPL tool catalog and generate JSON or markdown API documentation.",
        "UnityReplGs.Tools.ToolListTool.List(false)",
        "UnityReplGs.Tools.ToolListTool.Get(\"AssetsFindTool\")",
        "UnityReplGs.Tools.ToolListTool.Markdown(null, \"Scene\")")]
    public static class ToolListTool
    {
        static readonly string[] KnownAreas =
        {
            "Assets",
            "Console",
            "Editor",
            "GameObject",
            "Object",
            "Package",
            "Profiler",
            "Reflection",
            "Scene",
            "Screenshot",
            "Script",
            "Tests",
            "Tool",
            "Type"
        };

        public static string List(bool includeMethods = true, string area = null)
        {
            return JsonOut.Try(() => FindTools(area)
                .Select(type => ToolDoc(type, includeMethods))
                .ToArray());
        }

        public static string Get(string toolName)
        {
            return JsonOut.Try(() =>
            {
                var type = FindTool(toolName);
                if (type == null)
                    throw new ArgumentException("Tool not found: " + toolName);
                return ToolDoc(type, true);
            });
        }

        public static string Markdown(string toolName = null, string area = null)
        {
            return JsonOut.Try(() =>
            {
                var types = string.IsNullOrEmpty(toolName)
                    ? FindTools(area).ToArray()
                    : new[] { FindTool(toolName) };
                if (types.Any(type => type == null))
                    throw new ArgumentException("Tool not found: " + toolName);
                return BuildMarkdown(types);
            });
        }

        static IEnumerable<Type> FindTools(string area)
        {
            return typeof(ToolListTool).Assembly.GetTypes()
                .Where(IsToolClass)
                .Where(type => string.IsNullOrEmpty(area) || string.Equals(GetArea(type), area, StringComparison.OrdinalIgnoreCase))
                .OrderBy(GetArea)
                .ThenBy(type => type.Name);
        }

        static Type FindTool(string toolName)
        {
            if (string.IsNullOrEmpty(toolName))
                return null;
            return FindTools(null).FirstOrDefault(type =>
                string.Equals(type.Name, toolName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(type.FullName, toolName, StringComparison.OrdinalIgnoreCase));
        }

        static bool IsToolClass(Type type)
        {
            return type != null
                && type.IsClass
                && type.IsAbstract
                && type.IsSealed
                && type.IsPublic
                && type.Namespace == "UnityReplGs.Tools"
                && type.Name.EndsWith("Tool", StringComparison.Ordinal);
        }

        static object ToolDoc(Type type, bool includeMethods)
        {
            var comment = type.GetCustomAttribute<ToolCommentAttribute>();
            return new
            {
                name = type.Name,
                fullName = type.FullName,
                area = GetArea(type),
                summary = comment == null || string.IsNullOrEmpty(comment.Summary) ? BuildSummary(type.Name) : comment.Summary,
                examples = comment == null ? new string[0] : comment.Examples,
                methods = includeMethods ? GetMethodDocs(type).ToArray() : null
            };
        }

        static IEnumerable<object> GetMethodDocs(Type type)
        {
            return type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Where(method => !method.IsSpecialName)
                .OrderBy(method => method.Name)
                .ThenBy(method => method.GetParameters().Length)
                .Select(method => new
                {
                    name = method.Name,
                    signature = BuildSignature(type, method),
                    returnType = FormatType(method.ReturnType),
                    isCoroutine = typeof(IEnumerator).IsAssignableFrom(method.ReturnType),
                    parameters = method.GetParameters().Select(ParameterDoc).ToArray()
                });
        }

        static object ParameterDoc(ParameterInfo parameter)
        {
            return new
            {
                name = parameter.Name,
                type = FormatType(parameter.ParameterType),
                optional = parameter.IsOptional,
                defaultValue = parameter.IsOptional ? FormatDefault(parameter.DefaultValue) : null
            };
        }

        static string BuildSignature(Type declaringType, MethodInfo method)
        {
            var parameters = method.GetParameters()
                .Select(parameter =>
                {
                    var text = FormatType(parameter.ParameterType) + " " + parameter.Name;
                    if (parameter.IsOptional)
                        text += " = " + FormatDefault(parameter.DefaultValue);
                    return text;
                });
            return FormatType(method.ReturnType) + " " + declaringType.Name + "." + method.Name + "(" + string.Join(", ", parameters.ToArray()) + ")";
        }

        static string BuildMarkdown(IEnumerable<Type> types)
        {
            var builder = new StringBuilder();
            builder.AppendLine("# UnityREPL Tool API");
            foreach (var type in types)
            {
                builder.AppendLine();
                builder.Append("## ").Append(type.Name).AppendLine();
                builder.AppendLine();
                var comment = type.GetCustomAttribute<ToolCommentAttribute>();
                var summary = comment == null || string.IsNullOrEmpty(comment.Summary) ? BuildSummary(type.Name) : comment.Summary;
                builder.Append("- Area: `").Append(GetArea(type)).AppendLine("`");
                builder.Append("- Full name: `").Append(type.FullName).AppendLine("`");
                builder.Append("- Summary: ").AppendLine(summary);
                if (comment != null && comment.Examples.Length > 0)
                {
                    builder.AppendLine();
                    builder.AppendLine("Examples:");
                    builder.AppendLine();
                    builder.AppendLine("```csharp");
                    foreach (var example in comment.Examples)
                        builder.AppendLine(example);
                    builder.AppendLine("```");
                }
                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                    .Where(method => !method.IsSpecialName)
                    .OrderBy(method => method.Name)
                    .ThenBy(method => method.GetParameters().Length))
                {
                    builder.AppendLine();
                    builder.Append("### ").Append(method.Name).AppendLine();
                    builder.AppendLine();
                    builder.Append("`").Append(BuildSignature(type, method)).AppendLine("`");
                }
            }
            return builder.ToString();
        }

        static string GetArea(Type type)
        {
            return KnownAreas.FirstOrDefault(area => type.Name.StartsWith(area, StringComparison.Ordinal)) ?? "Other";
        }

        static string BuildSummary(string typeName)
        {
            if (typeName.EndsWith("Tool", StringComparison.Ordinal))
                typeName = typeName.Substring(0, typeName.Length - "Tool".Length);
            var builder = new StringBuilder();
            for (var i = 0; i < typeName.Length; i++)
            {
                if (i > 0 && char.IsUpper(typeName[i]) && !char.IsUpper(typeName[i - 1]))
                    builder.Append(' ');
                builder.Append(typeName[i]);
            }
            return builder.ToString();
        }

        static string FormatType(Type type)
        {
            if (type == null)
                return "void";
            if (type.IsByRef)
                return FormatType(type.GetElementType()) + "&";
            if (type.IsArray)
                return FormatType(type.GetElementType()) + "[]";

            var nullable = Nullable.GetUnderlyingType(type);
            if (nullable != null)
                return FormatType(nullable) + "?";

            if (type == typeof(void))
                return "void";
            if (type == typeof(bool))
                return "bool";
            if (type == typeof(byte))
                return "byte";
            if (type == typeof(int))
                return "int";
            if (type == typeof(long))
                return "long";
            if (type == typeof(float))
                return "float";
            if (type == typeof(double))
                return "double";
            if (type == typeof(string))
                return "string";
            if (type == typeof(object))
                return "object";

            if (type.IsGenericType)
            {
                var name = type.Name;
                var tick = name.IndexOf('`');
                if (tick >= 0)
                    name = name.Substring(0, tick);
                return name + "<" + string.Join(", ", type.GetGenericArguments().Select(FormatType).ToArray()) + ">";
            }

            return string.IsNullOrEmpty(type.Namespace) || type.Namespace == "UnityReplGs.Tools"
                ? type.Name
                : type.FullName;
        }

        static string FormatDefault(object value)
        {
            if (value == null || value == DBNull.Value || value == Type.Missing)
                return "null";
            if (value is string)
                return "\"" + value.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
            if (value is bool)
                return (bool)value ? "true" : "false";
            if (value is Enum)
                return value.GetType().Name + "." + value;
            if (value is float)
                return ((float)value).ToString(CultureInfo.InvariantCulture) + "f";
            if (value is double)
                return ((double)value).ToString(CultureInfo.InvariantCulture);
            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }
    }
}
