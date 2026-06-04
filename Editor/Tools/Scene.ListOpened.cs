
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
    [ToolComment("List all currently opened Unity scenes.", "UnityReplGs.Tools.SceneListOpenedTool.ListOpened()")]
    public static class SceneListOpenedTool
    {
        public static string ListOpened()
        {
            return JsonOut.Try(() => Enumerable.Range(0, UnityEngine.SceneManagement.SceneManager.sceneCount)
                .Select(i => ToolUtil.ToSceneData(UnityEngine.SceneManagement.SceneManager.GetSceneAt(i)))
                .ToArray());
        }
    }
}
