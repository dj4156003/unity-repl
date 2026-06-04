using System.Reflection;
using Newtonsoft.Json.Linq;

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
    [ToolComment("Invoke a reflected static or instance method with JSON arguments.", "UnityReplGs.Tools.ReflectionMethodCallTool.Call(\"UnityEditor.EditorApplication\", \"QueuePlayerLoopUpdate\", null, null, false)")]
    public static class ReflectionMethodCallTool
    {
        public static string Call(string typeName, string methodName, string argsJson = null, ObjectRef targetRef = null, bool includeNonPublic = false)
        {
            return JsonOut.Try(() =>
            {
                var type = ToolUtil.FindType(typeName);
                if (type == null)
                    throw new ArgumentException("Type not found: " + typeName);
                var flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;
                if (includeNonPublic)
                    flags |= BindingFlags.NonPublic;
                var suppliedCount = CountArgs(argsJson);
                var candidates = type.GetMethods(flags).Where(m => m.Name == methodName).ToArray();
                var method = candidates
                    .Where(m => m.GetParameters().Length >= suppliedCount)
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
                    if (!type.IsAssignableFrom(target.GetType()))
                        throw new ArgumentException("targetRef type is " + target.GetType().FullName + ", not " + type.FullName + ".");
                }
                var args = ToolUtil.CoerceArguments(method.GetParameters(), argsJson);
                var value = method.Invoke(target, args);
                return new { value = value, returnType = method.ReturnType.FullName };
            });
        }

        static int CountArgs(string argsJson)
        {
            if (string.IsNullOrEmpty(argsJson))
                return 0;
            var token = JToken.Parse(argsJson);
            if (token.Type == JTokenType.Array)
                return ((JArray)token).Count;
            if (token.Type == JTokenType.Object)
                return ((JObject)token).Count;
            return 1;
        }
    }
}
