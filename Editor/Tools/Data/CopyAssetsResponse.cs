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
    public class CopyAssetsResponse
    {
        public string sourcePath;
        public string destinationPath;
        public bool success;
        public string error;
    }
}
