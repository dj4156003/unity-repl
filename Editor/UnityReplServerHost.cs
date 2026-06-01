using LambdaLabs.UnityRepl.Editor.Transport;
using UnityEditor;
using UnityEngine;

namespace LambdaLabs.UnityRepl.Editor
{
    /// <summary>
    /// Entry point for UnityREPL. Starts the file-based IPC transport
    /// automatically when Unity Editor loads.
    /// </summary>
    [InitializeOnLoad]
    internal static class UnityReplServerHost
    {
        private const int StartupRetryFrames = 120;

        private static FileIpcTransport _ipcTransport;
        private static int _startupRetryFramesRemaining;

        public static bool IsRunning => _ipcTransport?.IsRunning ?? false;

        static UnityReplServerHost()
        {
            EditorApplication.quitting += OnEditorQuitting;
            QueueStart();
        }

        [InitializeOnLoadMethod]
        private static void OnEditorLoad()
        {
            QueueStart();
        }

        private static void QueueStart()
        {
            EditorApplication.delayCall -= StartServer;
            EditorApplication.delayCall += StartServer;

            _startupRetryFramesRemaining = StartupRetryFrames;
            EditorApplication.update -= RetryStartOnUpdate;
            EditorApplication.update += RetryStartOnUpdate;
        }

        private static void RetryStartOnUpdate()
        {
            if (IsRunning || _startupRetryFramesRemaining-- <= 0)
            {
                EditorApplication.update -= RetryStartOnUpdate;
                return;
            }

            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
                return;

            StartServer();
        }

        public static void StartServer()
        {
            if (IsRunning) return;

            try
            {
                _ipcTransport = new FileIpcTransport();
                _ipcTransport.Start();
                Debug.Log("[UnityREPL] Server started.");
            }
            catch (System.Exception ex)
            {
                _ipcTransport = null;
                Debug.LogError($"[UnityREPL] Failed to start server: {ex}");
            }
        }

        public static void StopServer(bool deletePortFile = true, string reason = null)
        {
            _ipcTransport?.Stop();
            _ipcTransport = null;
        }

        private static void OnEditorQuitting()
        {
            StopServer();
        }
    }
}
