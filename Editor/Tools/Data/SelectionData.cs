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
    public class SelectionData
    {
        public int[] instanceIds;
        public string[] assetGuids;
        public string activeObjectName;
        public int activeObjectInstanceId;
        public string activeGameObjectPath;
    }
}
