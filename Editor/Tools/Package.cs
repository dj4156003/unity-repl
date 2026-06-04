using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace UnityReplGs.Tools
{
    public static class PackageTools
    {
        public const string PackageAddToolId = "package-add";
        public const string PackageListToolId = "package-list";
        public const string PackageRemoveToolId = "package-remove";
        public const string PackageSearchToolId = "package-search";
    }
}
