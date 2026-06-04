using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace UnityReplGs.Tools.Tests
{
    public class TestResultData
    {
        public string name;
        public string fullName;
        public string status;
        public double duration;
        public string message;
        public string stackTrace;
    }
}
