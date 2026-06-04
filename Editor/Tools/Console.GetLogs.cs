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
    [ToolComment("Read recent Unity Console entries, optionally filtered by log type.", "UnityReplGs.Tools.ConsoleGetLogsTool.GetLogs(50, \"Warning\")")]
    public static class ConsoleGetLogsTool
    {
        public static string GetLogs(int maxEntries = 100, string logType = null)
        {
            return JsonOut.Try(() =>
            {
                if (maxEntries <= 0)
                    throw new ArgumentException("maxEntries must be greater than zero.");
                var entriesType = Type.GetType("UnityEditor.LogEntries,UnityEditor");
                var entryType = Type.GetType("UnityEditor.LogEntry,UnityEditor");
                if (entriesType == null || entryType == null)
                    throw new InvalidOperationException("Unity editor log entry types were not found.");

                var getCount = entriesType.GetMethod("GetCount", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                var start = entriesType.GetMethod("StartGettingEntries", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                var getEntry = entriesType.GetMethod("GetEntryInternal", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                var end = entriesType.GetMethod("EndGettingEntries", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                if (getCount == null || getEntry == null)
                    throw new MissingMethodException("UnityEditor.LogEntries.GetEntryInternal");

                var count = (int)getCount.Invoke(null, null);
                var result = new List<object>();
                start?.Invoke(null, null);
                try
                {
                    var min = Math.Max(0, count - maxEntries);
                    for (var i = count - 1; i >= min; i--)
                    {
                        var entry = Activator.CreateInstance(entryType);
                        var ok = (bool)getEntry.Invoke(null, new[] { (object)i, entry });
                        if (!ok)
                            continue;
                        var condition = ReadField<string>(entryType, entry, "condition");
                        var file = ReadField<string>(entryType, entry, "file");
                        var line = ReadField<int>(entryType, entry, "line");
                        var mode = ReadField<int>(entryType, entry, "mode");
                        var instanceId = ReadField<int>(entryType, entry, "instanceID");
                        var typeName = GuessLogType(mode, condition);
                        if (!string.IsNullOrEmpty(logType) && !typeName.Equals(logType, StringComparison.OrdinalIgnoreCase))
                            continue;
                        result.Add(new { condition = condition, file = file, line = line, mode = mode, type = typeName, instanceId = instanceId });
                    }
                }
                finally
                {
                    end?.Invoke(null, null);
                }
                return new { count = count, returned = result.Count, entries = result };
            });
        }

        static T ReadField<T>(Type type, object instance, string name)
        {
            var field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field == null)
                return default(T);
            var value = field.GetValue(instance);
            return value == null ? default(T) : (T)value;
        }

        static string GuessLogType(int mode, string condition)
        {
            if ((mode & (1 << 11)) != 0 || (condition != null && condition.IndexOf("Exception", StringComparison.OrdinalIgnoreCase) >= 0))
                return LogType.Exception.ToString();
            if ((mode & (1 << 0)) != 0 || (mode & (1 << 2)) != 0)
                return LogType.Error.ToString();
            if ((mode & (1 << 1)) != 0)
                return LogType.Assert.ToString();
            if ((mode & (1 << 3)) != 0)
                return LogType.Warning.ToString();
            return LogType.Log.ToString();
        }
    }
}
