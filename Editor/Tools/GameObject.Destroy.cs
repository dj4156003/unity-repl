
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
    [ToolComment("Destroy a scene GameObject.", "UnityReplGs.Tools.GameObjectDestroyTool.Destroy(new UnityReplGs.Tools.ObjectRef { name = \"Probe Cube\" })")]
    public static class GameObjectDestroyTool
    {
        public static string Destroy(ObjectRef gameObjectRef)
        {
            return JsonOut.Try(() =>
            {
                var go = ToolUtil.ResolveGameObject(gameObjectRef);
                if (go == null)
                    throw new System.ArgumentException("GameObject not found.");
                var data = ToolUtil.ToGameObjectData(go);
                Undo.DestroyObjectImmediate(go);
                return new { destroyed = true, gameObject = data };
            });
        }
    }
}
