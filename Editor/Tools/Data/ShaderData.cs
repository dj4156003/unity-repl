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
    public class ShaderData
    {
        public string name;
        public string path;
        public bool isSupported;
        public int maximumLOD;
        public int renderQueue;
        public object[] properties;
        public object[] messages;
    }
}
