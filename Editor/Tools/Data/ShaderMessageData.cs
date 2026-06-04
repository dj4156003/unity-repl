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
    public class ShaderMessageData
    {
        public string message;
        public string file;
        public int line;
        public int platform;
        public int severity;
    }
}
