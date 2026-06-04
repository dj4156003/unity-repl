
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
    public class TestRunResponse
    {
        public object summary;
        public List<object> results;
        public List<object> logs;
    }
}
