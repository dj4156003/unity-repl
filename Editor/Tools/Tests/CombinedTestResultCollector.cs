
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
    public class CombinedTestResultCollector
    {
        public readonly List<object> collectors = new List<object>();

        public object Combine()
        {
            return new { collectors = collectors };
        }
    }
}
