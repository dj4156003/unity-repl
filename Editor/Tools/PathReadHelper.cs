
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
    public static class PathReadHelper
    {
        public const string PathReadAggregateTypeName = "UnityReplGs.PathReadAggregate";
        public const string EmptyPathTypeName = "<empty-path>";
        public const string UnresolvedTypeName = "<unresolved>";

        public static Dictionary<string, object> Read(UnityEngine.Object target, IEnumerable<string> paths)
        {
            if (target == null)
                throw new System.ArgumentException("target is null.");
            if (paths == null)
                return new Dictionary<string, object>();
            var result = new Dictionary<string, object>();
            foreach (var path in paths)
                result[path] = ReadPath(target, path);
            return result;
        }

        static object ReadPath(UnityEngine.Object target, string path)
        {
            if (string.IsNullOrEmpty(path))
                return new { name = target.name, type = target.GetType().FullName, instanceId = target.GetInstanceID() };
            var so = new UnityEditor.SerializedObject(target);
            var prop = so.FindProperty(path.TrimStart('#', '/'));
            if (prop == null)
                return new { type = UnresolvedTypeName, path = path };
            switch (prop.propertyType)
            {
                case UnityEditor.SerializedPropertyType.Integer: return prop.intValue;
                case UnityEditor.SerializedPropertyType.Boolean: return prop.boolValue;
                case UnityEditor.SerializedPropertyType.Float: return prop.floatValue;
                case UnityEditor.SerializedPropertyType.String: return prop.stringValue;
                case UnityEditor.SerializedPropertyType.Color: return prop.colorValue;
                case UnityEditor.SerializedPropertyType.ObjectReference: return prop.objectReferenceValue == null ? null : new { name = prop.objectReferenceValue.name, type = prop.objectReferenceValue.GetType().FullName, instanceId = prop.objectReferenceValue.GetInstanceID() };
                case UnityEditor.SerializedPropertyType.Enum: return prop.enumDisplayNames != null && prop.enumValueIndex >= 0 && prop.enumValueIndex < prop.enumDisplayNames.Length ? prop.enumDisplayNames[prop.enumValueIndex] : prop.enumValueIndex.ToString();
                case UnityEditor.SerializedPropertyType.Vector2: return prop.vector2Value;
                case UnityEditor.SerializedPropertyType.Vector3: return prop.vector3Value;
                case UnityEditor.SerializedPropertyType.Vector4: return prop.vector4Value;
                case UnityEditor.SerializedPropertyType.Rect: return prop.rectValue;
                case UnityEditor.SerializedPropertyType.Bounds: return prop.boundsValue;
                default: return prop.propertyType.ToString();
            }
        }
    }
}
