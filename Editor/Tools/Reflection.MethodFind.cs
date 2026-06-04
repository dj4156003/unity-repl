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
    [ToolComment("Search methods on a type by name, visibility, and static-only filters.", "UnityReplGs.Tools.ReflectionMethodFindTool.Find(\"UnityEditor.EditorApplication\", \"Queue\", false, true, 20)")]
    public static class ReflectionMethodFindTool
    {
        public static string Find(string typeName = null, string methodName = null, bool includeNonPublic = false, bool staticOnly = false, int maxResults = 50)
        {
            return JsonOut.Try(() =>
            {
                var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
                if (includeNonPublic)
                    flags |= BindingFlags.NonPublic;
                var types = string.IsNullOrEmpty(typeName)
                    ? ToolUtil.GetAllTypes()
                    : new[] { ToolUtil.FindType(typeName) }.Where(t => t != null);
                return types.SelectMany(t => t.GetMethods(flags)
                        .Where(m => !staticOnly || m.IsStatic)
                        .Where(m => string.IsNullOrEmpty(methodName) || m.Name.IndexOf(methodName, StringComparison.OrdinalIgnoreCase) >= 0)
                        .Select(m => new
                        {
                            type = t.FullName,
                            name = m.Name,
                            isStatic = m.IsStatic,
                            returnType = m.ReturnType.FullName,
                            parameters = m.GetParameters().Select(p => new { name = p.Name, type = p.ParameterType.FullName, optional = p.IsOptional }).ToArray()
                        }))
                    .Take(maxResults)
                    .ToArray();
            });
        }
    }
}
