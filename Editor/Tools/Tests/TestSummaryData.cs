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
    public class TestSummaryData
    {
        public int total;
        public int passed;
        public int failed;
        public int skipped;
    }
}
