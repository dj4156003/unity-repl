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
    public class DestroyComponentsResponse
    {
        public bool destroyed;
        public string componentType;
        public int componentInstanceId;
    }
}
