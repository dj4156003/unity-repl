using System.Collections;
using UnityEditor.TestTools.TestRunner.Api;

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
    [ToolComment("Run Unity Test Framework tests from EditMode or PlayMode and return structured results.", "UnityReplGs.Tools.TestsRunTool.Run(\"EditMode\", null, null, null, null, false, true, false, false, \"Warning\", false, 300)")]
    public static class TestsRunTool
    {
        public static IEnumerator Run(string testMode = "EditMode", string testAssembly = null, string testNamespace = null, string testClass = null, string testMethod = null, bool includePassingTests = false, bool includeMessages = true, bool includeStacktrace = false, bool includeLogs = false, string minLogType = "Warning", bool includeLogStacktrace = false, int timeoutSeconds = 300)
        {
            var dirtyScenes = Enumerable.Range(0, UnityEngine.SceneManagement.SceneManager.sceneCount)
                .Select(i => UnityEngine.SceneManagement.SceneManager.GetSceneAt(i))
                .Where(s => s.isDirty)
                .Select(s => string.IsNullOrEmpty(s.path) ? s.name : s.path)
                .ToArray();
            if (dirtyScenes.Length > 0)
            {
                yield return JsonOut.Error(new InvalidOperationException("Open scenes have unsaved changes: " + string.Join(", ", dirtyScenes)));
                yield break;
            }

            AssetDatabase.Refresh();
            while (EditorApplication.isCompiling)
                yield return null;
            if (EditorUtility.scriptCompilationFailed)
            {
                yield return JsonOut.Error(new InvalidOperationException("Cannot run tests: Unity project has script compilation errors."));
                yield break;
            }

            TestMode parsedMode;
            if (!Enum.TryParse(testMode, true, out parsedMode))
            {
                yield return JsonOut.Error(new ArgumentException("Invalid testMode: " + testMode));
                yield break;
            }

            var filter = new Filter { testMode = parsedMode };
            if (!string.IsNullOrEmpty(testAssembly))
                filter.assemblyNames = new[] { testAssembly };
            if (!string.IsNullOrEmpty(testMethod))
                filter.testNames = new[] { testMethod };
            var groups = new List<string>();
            if (!string.IsNullOrEmpty(testNamespace))
                groups.Add(testNamespace);
            if (!string.IsNullOrEmpty(testClass))
                groups.Add(testClass);
            if (groups.Count > 0)
                filter.groupNames = groups.ToArray();

            var collector = new ReplTestCallbacks(includePassingTests, includeMessages, includeStacktrace, includeLogs, minLogType, includeLogStacktrace);
            var api = ScriptableObject.CreateInstance<TestRunnerApi>();
            api.RegisterCallbacks(collector);
            collector.BeginLogCapture();
            try
            {
                api.Execute(new ExecutionSettings(filter));
                var deadline = EditorApplication.timeSinceStartup + Math.Max(1, timeoutSeconds);
                while (!collector.Done && EditorApplication.timeSinceStartup < deadline)
                    yield return null;
                if (!collector.Done)
                    yield return JsonOut.Error(new TimeoutException("Test run timed out after " + timeoutSeconds + " seconds."));
                else
                    yield return JsonOut.Ok(collector.Response);
            }
            finally
            {
                collector.EndLogCapture();
                api.UnregisterCallbacks(collector);
                UnityEngine.Object.DestroyImmediate(api);
            }
        }

        class ReplTestCallbacks : ICallbacks
        {
            readonly bool includePassingTests;
            readonly bool includeMessages;
            readonly bool includeStacktrace;
            readonly bool includeLogs;
            readonly LogType minLogType;
            readonly bool includeLogStacktrace;
            readonly List<object> results = new List<object>();
            readonly List<object> logs = new List<object>();
            int total;
            int passed;
            int failed;
            int skipped;

            public bool Done { get; private set; }
            public object Response
            {
                get
                {
                    return new
                    {
                        summary = new { total = total, passed = passed, failed = failed, skipped = skipped },
                        results = results,
                        logs = includeLogs ? logs : null
                    };
                }
            }

            public ReplTestCallbacks(bool includePassingTests, bool includeMessages, bool includeStacktrace, bool includeLogs, string minLogType, bool includeLogStacktrace)
            {
                this.includePassingTests = includePassingTests;
                this.includeMessages = includeMessages;
                this.includeStacktrace = includeStacktrace;
                this.includeLogs = includeLogs;
                this.includeLogStacktrace = includeLogStacktrace;
                LogType parsed;
                this.minLogType = Enum.TryParse(minLogType, true, out parsed) ? parsed : LogType.Warning;
            }

            public void BeginLogCapture()
            {
                if (includeLogs)
                    Application.logMessageReceived += OnLog;
            }

            public void EndLogCapture()
            {
                if (includeLogs)
                    Application.logMessageReceived -= OnLog;
            }

            public void RunStarted(ITestAdaptor testsToRun) { }

            public void RunFinished(ITestResultAdaptor result)
            {
                total = result.TestStatus == TestStatus.Skipped ? 0 : result.PassCount + result.FailCount + result.SkipCount + result.InconclusiveCount;
                passed = result.PassCount;
                failed = result.FailCount;
                skipped = result.SkipCount + result.InconclusiveCount;
                Done = true;
            }

            public void TestStarted(ITestAdaptor test) { }

            public void TestFinished(ITestResultAdaptor result)
            {
                var status = result.TestStatus.ToString();
                var passedResult = result.TestStatus == TestStatus.Passed;
                if (passedResult && !includePassingTests)
                    return;
                results.Add(new
                {
                    name = result.Name,
                    fullName = result.FullName,
                    status = status,
                    duration = result.Duration,
                    message = includeMessages ? result.Message : null,
                    stackTrace = includeStacktrace ? result.StackTrace : null
                });
            }

            void OnLog(string condition, string stackTrace, LogType type)
            {
                if (!ShouldInclude(type))
                    return;
                logs.Add(new { type = type.ToString(), message = condition, stackTrace = includeLogStacktrace ? stackTrace : null });
            }

            bool ShouldInclude(LogType type)
            {
                return Severity(type) >= Severity(minLogType);
            }

            static int Severity(LogType type)
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
}
