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
    public class AddComponentResponse
    {
        public bool success;
        public string gameObjectPath;
        public string componentType;
        public int componentInstanceId;
    }
}
