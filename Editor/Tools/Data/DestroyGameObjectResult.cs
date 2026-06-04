using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace UnityReplGs.Tools.Data
{
    public class DestroyGameObjectResult
    {
        public bool destroyed;
        public string name;
        public string path;
        public int instanceId;
    }
}
