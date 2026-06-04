
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
    public class TestLogEntry
    {
        public string type;
        public string message;
        public string stackTrace;

        public static int ToLogLevel(LogType type)
        {
            switch (type)
            {
                case LogType.Exception: return 5;
                case LogType.Error: return 4;
                case LogType.Assert: return 3;
                case LogType.Warning: return 2;
                default: return 1;
            }
        }
    }
}
