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
    [ToolComment("Invoke a reflected script method by type and method name.", "UnityReplGs.Tools.ScriptExecuteTool.Execute(\"MyEditorScript\", \"Main\", null, null, false)")]
    public static class ScriptExecuteTool
    {
        public static string Execute(string typeName, string methodName = "Main", string argsJson = null, ObjectRef targetRef = null, bool includeNonPublic = false)
        {
            return JsonOut.Try(() =>
            {
                var type = ToolUtil.FindType(typeName);
                if (type == null)
                    throw new ArgumentException("Type not found: " + typeName);
                var flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;
                if (includeNonPublic)
                    flags |= BindingFlags.NonPublic;
                var method = type.GetMethods(flags)
                    .Where(m => m.Name == methodName)
                    .OrderBy(m => m.GetParameters().Length)
                    .FirstOrDefault();
                if (method == null)
                    throw new MissingMethodException(type.FullName, methodName);
                object target = null;
                if (!method.IsStatic)
                {
                    target = ToolUtil.ResolveObject(targetRef);
                    if (target == null)
                        throw new ArgumentException("targetRef is required for instance methods.");
                }
                var args = ToolUtil.CoerceArguments(method.GetParameters(), argsJson);
                var value = method.Invoke(target, args);
                return new { value = value, returnType = method.ReturnType.FullName };
            });
        }
    }
}
