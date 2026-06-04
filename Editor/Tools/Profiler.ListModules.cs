using UnityEngine.Profiling;

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
    [ToolComment("List Unity ProfilerArea values and their enabled state.", "UnityReplGs.Tools.ProfilerListModulesTool.ListModules()")]
    public static class ProfilerListModulesTool
    {
        public static string ListModules()
        {
            return JsonOut.Try(() => System.Enum.GetValues(typeof(ProfilerArea)).Cast<ProfilerArea>()
                .Select(a => new { name = a.ToString(), enabled = Profiler.GetAreaEnabled(a) })
                .ToArray());
        }
    }
}
