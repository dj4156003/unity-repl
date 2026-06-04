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
    public enum TestResultStatus
    {
        Passed,
        Failed,
        Skipped,
        Inconclusive
    }
}
