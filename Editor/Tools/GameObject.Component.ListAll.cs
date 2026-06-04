
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
    [ToolComment("List loaded component types and optionally search by name.", "UnityReplGs.Tools.GameObjectComponentListAllTool.ListAll(\"Collider\", 50)")]
    public static class GameObjectComponentListAllTool
    {
        public static string ListAll(string search = null, int maxResults = 200)
        {
            return JsonOut.Try(() => ToolUtil.GetAllTypes()
                .Where(t => t != null && typeof(Component).IsAssignableFrom(t) && !t.IsAbstract)
                .Where(t => string.IsNullOrEmpty(search) || t.FullName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 || t.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                .OrderBy(t => t.FullName)
                .Take(maxResults)
                .Select(t => new { name = t.Name, type = t.FullName, assembly = t.Assembly.GetName().Name })
                .ToArray());
        }
    }
}
