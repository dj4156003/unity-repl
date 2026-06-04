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
    public class ModifyObjectResponse
    {
        public bool success;
        public string name;
        public string type;
        public int instanceId;
    }
}
