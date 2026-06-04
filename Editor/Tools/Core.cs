using System.Collections;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
    public static class JsonOut
    {
        static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        };

        public static string Ok(object result)
        {
            return JsonConvert.SerializeObject(new { ok = true, result = result }, Settings);
        }

        public static string Error(Exception ex)
        {
            return JsonConvert.SerializeObject(new
            {
                ok = false,
                error = new
                {
                    type = ex.GetType().FullName,
                    message = ex.Message,
                    stackTrace = ex.StackTrace
                }
            }, Settings);
        }

        public static string ToJson(object value)
        {
            return JsonConvert.SerializeObject(value, Settings);
        }

        public static T Parse<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
                return default(T);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static string Try(Func<object> action)
        {
            try
            {
                return Ok(action());
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
    }

    public class ObjectRef
    {
        public int instanceId;
        public string name;
        public string path;
        public string assetPath;
        public string guid;
    }

    public class AssetRef
    {
        public string name;
        public string path;
        public string guid;
        public string type;
        public int instanceId;

        public static AssetRef FromPath(string path)
        {
            var type = AssetDatabase.GetMainAssetTypeAtPath(path);
            var obj = type == null ? null : AssetDatabase.LoadAssetAtPath(path, type);
            return new AssetRef
            {
                name = obj == null ? Path.GetFileNameWithoutExtension(path) : obj.name,
                path = path,
                guid = AssetDatabase.AssetPathToGUID(path),
                type = type == null ? null : type.FullName,
                instanceId = obj == null ? 0 : obj.GetInstanceID()
            };
        }

        public static AssetRef FromObject(UnityEngine.Object obj)
        {
            if (obj == null)
                return null;
            var path = AssetDatabase.GetAssetPath(obj);
            return new AssetRef
            {
                name = obj.name,
                path = path,
                guid = string.IsNullOrEmpty(path) ? null : AssetDatabase.AssetPathToGUID(path),
                type = obj.GetType().FullName,
                instanceId = obj.GetInstanceID()
            };
        }
    }

    public class GameObjectRefData
    {
        public string name;
        public string path;
        public int instanceId;
        public bool activeSelf;
        public string tag;
        public int layer;
        public int childCount;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
    }

    public class ComponentRefData
    {
        public string name;
        public string type;
        public int instanceId;
        public bool enabled;
    }

    public class SceneRefData
    {
        public string name;
        public string path;
        public int buildIndex;
        public bool isLoaded;
        public bool isDirty;
        public bool isActive;
        public int rootCount;
    }

    public class SelectionData
    {
        public int[] instanceIds;
        public string[] assetGuids;
        public string activeObjectName;
        public int activeObjectInstanceId;
        public string activeGameObjectPath;
    }

    public static class ToolUtil
    {
        public static string RequireAssetPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Asset path is empty.");
            path = path.Replace("\\", "/");
            if (!path.StartsWith("Assets/"))
                throw new ArgumentException("Asset path must start with 'Assets/'. Path: " + path);
            return path;
        }

        public static string ResolveAssetPath(string assetPath, string guid)
        {
            if (!string.IsNullOrEmpty(assetPath))
                return assetPath.Replace("\\", "/");
            if (!string.IsNullOrEmpty(guid))
                return AssetDatabase.GUIDToAssetPath(guid);
            throw new ArgumentException("Provide assetPath or guid.");
        }

        public static UnityEngine.Object ResolveObject(ObjectRef objRef)
        {
            if (objRef == null)
                return null;
            if (objRef.instanceId != 0)
                return EditorUtility.InstanceIDToObject(objRef.instanceId);
            if (!string.IsNullOrEmpty(objRef.assetPath) || !string.IsNullOrEmpty(objRef.guid))
            {
                var path = ResolveAssetPath(objRef.assetPath, objRef.guid);
                return AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            }
            var go = ResolveGameObject(objRef);
            if (go != null)
                return go;
            if (!string.IsNullOrEmpty(objRef.name))
                return Resources.FindObjectsOfTypeAll<UnityEngine.Object>().FirstOrDefault(o => o != null && o.name == objRef.name);
            return null;
        }

        public static GameObject ResolveGameObject(ObjectRef objRef)
        {
            if (objRef == null)
                return null;
            if (objRef.instanceId != 0)
                return EditorUtility.InstanceIDToObject(objRef.instanceId) as GameObject;
            if (!string.IsNullOrEmpty(objRef.path))
                return FindGameObjectByPath(objRef.path);
            if (!string.IsNullOrEmpty(objRef.name))
                return GameObject.Find(objRef.name) ?? Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(g => g.name == objRef.name);
            return null;
        }

        public static Component ResolveComponent(ObjectRef gameObjectRef, ObjectRef componentRef, string componentType)
        {
            if (componentRef != null && componentRef.instanceId != 0)
                return EditorUtility.InstanceIDToObject(componentRef.instanceId) as Component;
            var go = ResolveGameObject(gameObjectRef);
            if (go == null)
                return null;
            var components = go.GetComponents<Component>();
            if (componentRef != null && !string.IsNullOrEmpty(componentRef.name))
            {
                if (componentRef.name.StartsWith("[") && componentRef.name.EndsWith("]"))
                {
                    int index;
                    if (int.TryParse(componentRef.name.Trim('[', ']'), out index) && index >= 0 && index < components.Length)
                        return components[index];
                }
                return components.FirstOrDefault(c => c != null && c.GetType().Name == componentRef.name);
            }
            if (!string.IsNullOrEmpty(componentType))
                return components.FirstOrDefault(c => c != null && (c.GetType().FullName == componentType || c.GetType().Name == componentType));
            return components.FirstOrDefault();
        }

        public static Type FindType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
                return null;
            var type = Type.GetType(typeName);
            if (type != null)
                return type;
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = asm.GetType(typeName);
                if (type != null)
                    return type;
            }
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); }
                    catch { return new Type[0]; }
                })
                .FirstOrDefault(t => t.FullName == typeName || t.Name == typeName);
        }

        public static IEnumerable<Type> GetAllTypes()
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types;
                try { types = asm.GetTypes(); }
                catch (ReflectionTypeLoadException ex) { types = ex.Types.Where(t => t != null).ToArray(); }
                catch { continue; }
                foreach (var type in types)
                    if (type != null)
                        yield return type;
            }
        }

        public static GameObject FindGameObjectByPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;
            path = path.Trim('/');
            var parts = path.Split('/');
            if (parts.Length == 0)
                return null;
            var roots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            Transform current = null;
            foreach (var root in roots)
            {
                if (root.name == parts[0])
                {
                    current = root.transform;
                    break;
                }
            }
            if (current == null)
                return GameObject.Find(path);
            for (var i = 1; i < parts.Length; i++)
            {
                current = current.Find(parts[i]);
                if (current == null)
                    return null;
            }
            return current.gameObject;
        }

        public static string GetGameObjectPath(GameObject go)
        {
            if (go == null)
                return null;
            var names = new List<string>();
            var t = go.transform;
            while (t != null)
            {
                names.Add(t.name);
                t = t.parent;
            }
            names.Reverse();
            return string.Join("/", names.ToArray());
        }

        public static GameObjectRefData ToGameObjectData(GameObject go)
        {
            if (go == null)
                return null;
            return new GameObjectRefData
            {
                name = go.name,
                path = GetGameObjectPath(go),
                instanceId = go.GetInstanceID(),
                activeSelf = go.activeSelf,
                tag = go.tag,
                layer = go.layer,
                childCount = go.transform.childCount,
                position = go.transform.position,
                rotation = go.transform.eulerAngles,
                scale = go.transform.localScale
            };
        }

        public static ComponentRefData ToComponentData(Component c)
        {
            if (c == null)
                return null;
            var enabledProp = c.GetType().GetProperty("enabled", BindingFlags.Instance | BindingFlags.Public);
            var enabled = enabledProp == null || !(enabledProp.PropertyType == typeof(bool)) ? true : (bool)enabledProp.GetValue(c, null);
            return new ComponentRefData
            {
                name = c.GetType().Name,
                type = c.GetType().FullName,
                instanceId = c.GetInstanceID(),
                enabled = enabled
            };
        }

        public static object ToObjectSummary(UnityEngine.Object obj, bool includeSerialized, bool includeHidden)
        {
            if (obj == null)
                return null;
            var go = obj as GameObject;
            if (go != null)
                return ToGameObjectSummary(go, includeSerialized, includeHidden, true, false, 0);
            var component = obj as Component;
            if (component != null)
            {
                return new
                {
                    component = ToComponentData(component),
                    gameObject = ToGameObjectData(component.gameObject),
                    data = includeSerialized ? ReadSerializedObject(component, includeHidden) : null
                };
            }
            return new
            {
                asset = AssetRef.FromObject(obj),
                data = includeSerialized ? ReadSerializedObject(obj, includeHidden) : null
            };
        }

        public static object ToGameObjectSummary(GameObject go, bool includeSerialized, bool includeHidden, bool includeComponents, bool includeHierarchy, int hierarchyDepth)
        {
            if (go == null)
                return null;
            return new
            {
                gameObject = ToGameObjectData(go),
                data = includeSerialized ? ReadSerializedObject(go, includeHidden) : null,
                components = includeComponents ? go.GetComponents<Component>().Where(c => c != null).Select(ToComponentData).ToArray() : null,
                bounds = GetBoundsData(go),
                hierarchy = includeHierarchy ? ReadHierarchy(go.transform, Math.Max(0, hierarchyDepth)) : null
            };
        }

        public static object GetBoundsData(GameObject go)
        {
            if (go == null)
                return null;
            var renderers = go.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0)
                return null;
            var bounds = renderers[0].bounds;
            for (var i = 1; i < renderers.Length; i++)
                bounds.Encapsulate(renderers[i].bounds);
            return new { center = bounds.center, size = bounds.size, min = bounds.min, max = bounds.max };
        }

        public static object ReadHierarchy(Transform transform, int depth)
        {
            if (transform == null)
                return null;
            return new
            {
                name = transform.name,
                path = GetGameObjectPath(transform.gameObject),
                instanceId = transform.gameObject.GetInstanceID(),
                childCount = transform.childCount,
                children = depth <= 0
                    ? new object[0]
                    : Enumerable.Range(0, transform.childCount)
                        .Select(i => ReadHierarchy(transform.GetChild(i), depth - 1))
                        .ToArray()
            };
        }

        public static SceneRefData ToSceneData(UnityEngine.SceneManagement.Scene scene)
        {
            return new SceneRefData
            {
                name = scene.name,
                path = scene.path,
                buildIndex = scene.buildIndex,
                isLoaded = scene.isLoaded,
                isDirty = scene.isDirty,
                isActive = scene == UnityEngine.SceneManagement.SceneManager.GetActiveScene(),
                rootCount = scene.isLoaded ? scene.rootCount : 0
            };
        }

        public static Dictionary<string, object> ReadSerializedObject(UnityEngine.Object obj, bool includeHidden)
        {
            var result = new Dictionary<string, object>();
            if (obj == null)
                return result;
            var so = new SerializedObject(obj);
            var prop = so.GetIterator();
            var enterChildren = true;
            while (prop.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (!includeHidden && prop.propertyPath.StartsWith("m_"))
                    continue;
                result[prop.propertyPath] = ReadSerializedProperty(prop);
            }
            return result;
        }

        public static object ReadSerializedProperty(SerializedProperty prop)
        {
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Integer: return prop.intValue;
                case SerializedPropertyType.Boolean: return prop.boolValue;
                case SerializedPropertyType.Float: return prop.floatValue;
                case SerializedPropertyType.String: return prop.stringValue;
                case SerializedPropertyType.Color: return prop.colorValue;
                case SerializedPropertyType.ObjectReference:
                    return prop.objectReferenceValue == null ? null : new
                    {
                        name = prop.objectReferenceValue.name,
                        type = prop.objectReferenceValue.GetType().FullName,
                        instanceId = prop.objectReferenceValue.GetInstanceID(),
                        assetPath = AssetDatabase.GetAssetPath(prop.objectReferenceValue)
                    };
                case SerializedPropertyType.LayerMask: return prop.intValue;
                case SerializedPropertyType.Enum: return prop.enumDisplayNames != null && prop.enumValueIndex >= 0 && prop.enumValueIndex < prop.enumDisplayNames.Length ? prop.enumDisplayNames[prop.enumValueIndex] : prop.enumValueIndex.ToString();
                case SerializedPropertyType.Vector2: return prop.vector2Value;
                case SerializedPropertyType.Vector3: return prop.vector3Value;
                case SerializedPropertyType.Vector4: return prop.vector4Value;
                case SerializedPropertyType.Rect: return prop.rectValue;
                case SerializedPropertyType.Bounds: return prop.boundsValue;
                case SerializedPropertyType.Quaternion: return prop.quaternionValue.eulerAngles;
                case SerializedPropertyType.Character: return prop.intValue;
                case SerializedPropertyType.AnimationCurve: return prop.animationCurveValue == null ? null : prop.animationCurveValue.ToString();
                case SerializedPropertyType.Vector2Int: return prop.vector2IntValue;
                case SerializedPropertyType.Vector3Int: return prop.vector3IntValue;
                case SerializedPropertyType.RectInt: return prop.rectIntValue;
                case SerializedPropertyType.BoundsInt: return prop.boundsIntValue;
                default: return prop.propertyType.ToString();
            }
        }

        public static object ReadPath(UnityEngine.Object obj, string propertyPath)
        {
            if (obj == null)
                throw new ArgumentException("Object is null.");
            if (string.IsNullOrEmpty(propertyPath))
                return ToObjectSummary(obj, true, true);
            var so = new SerializedObject(obj);
            var prop = so.FindProperty(propertyPath.TrimStart('#', '/'));
            if (prop == null)
                throw new ArgumentException("Serialized property not found: " + propertyPath);
            return ReadSerializedProperty(prop);
        }

        public static void ApplySerializedPatch(UnityEngine.Object obj, string patchJson)
        {
            if (obj == null)
                throw new ArgumentException("Object is null.");
            if (string.IsNullOrEmpty(patchJson))
                return;
            var patch = JObject.Parse(patchJson);
            var so = new SerializedObject(obj);
            foreach (var item in patch)
            {
                var prop = so.FindProperty(item.Key);
                if (prop == null)
                    throw new ArgumentException("Serialized property not found: " + item.Key);
                SetSerializedProperty(prop, item.Value);
            }
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(obj);
        }

        public static void SetSerializedProperty(SerializedProperty prop, JToken value)
        {
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Integer:
                case SerializedPropertyType.LayerMask:
                    prop.intValue = value.Value<int>();
                    break;
                case SerializedPropertyType.Boolean:
                    prop.boolValue = value.Value<bool>();
                    break;
                case SerializedPropertyType.Float:
                    prop.floatValue = value.Value<float>();
                    break;
                case SerializedPropertyType.String:
                    prop.stringValue = value.Type == JTokenType.Null ? null : value.Value<string>();
                    break;
                case SerializedPropertyType.Enum:
                    if (value.Type == JTokenType.String)
                    {
                        var text = value.Value<string>();
                        var idx = Array.IndexOf(prop.enumDisplayNames, text);
                        if (idx < 0) idx = Array.IndexOf(prop.enumNames, text);
                        if (idx < 0) throw new ArgumentException("Enum value not found: " + text);
                        prop.enumValueIndex = idx;
                    }
                    else prop.enumValueIndex = value.Value<int>();
                    break;
                case SerializedPropertyType.Vector2:
                    prop.vector2Value = value.ToObject<Vector2>();
                    break;
                case SerializedPropertyType.Vector3:
                    prop.vector3Value = value.ToObject<Vector3>();
                    break;
                case SerializedPropertyType.Vector4:
                    prop.vector4Value = value.ToObject<Vector4>();
                    break;
                case SerializedPropertyType.Color:
                    prop.colorValue = value.ToObject<Color>();
                    break;
                case SerializedPropertyType.Vector2Int:
                    prop.vector2IntValue = value.ToObject<Vector2Int>();
                    break;
                case SerializedPropertyType.Vector3Int:
                    prop.vector3IntValue = value.ToObject<Vector3Int>();
                    break;
                case SerializedPropertyType.Rect:
                    prop.rectValue = value.ToObject<Rect>();
                    break;
                case SerializedPropertyType.RectInt:
                    prop.rectIntValue = value.ToObject<RectInt>();
                    break;
                case SerializedPropertyType.Bounds:
                    prop.boundsValue = value.ToObject<Bounds>();
                    break;
                case SerializedPropertyType.BoundsInt:
                    prop.boundsIntValue = value.ToObject<BoundsInt>();
                    break;
                case SerializedPropertyType.ObjectReference:
                    prop.objectReferenceValue = ResolveObject(value.ToObject<ObjectRef>());
                    break;
                default:
                    throw new NotSupportedException("Unsupported serialized property type: " + prop.propertyType);
            }
        }

        public static void SetTransform(GameObject go, Vector3? position, Vector3? rotation, Vector3? scale, bool worldSpace)
        {
            if (go == null)
                throw new ArgumentException("GameObject is null.");
            if (position.HasValue)
            {
                if (worldSpace) go.transform.position = position.Value;
                else go.transform.localPosition = position.Value;
            }
            if (rotation.HasValue)
            {
                if (worldSpace) go.transform.eulerAngles = rotation.Value;
                else go.transform.localEulerAngles = rotation.Value;
            }
            if (scale.HasValue)
                go.transform.localScale = scale.Value;
        }

        public static string EnsureParentFolder(string assetPath)
        {
            var folder = Path.GetDirectoryName(assetPath);
            if (string.IsNullOrEmpty(folder))
                return folder;
            folder = folder.Replace("\\", "/");
            if (AssetDatabase.IsValidFolder(folder))
                return folder;
            var parts = folder.Split('/');
            var current = parts[0];
            for (var i = 1; i < parts.Length; i++)
            {
                var next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
            return folder;
        }

        public static UnityEngine.SceneManagement.Scene FindScene(string scenePathOrName)
        {
            if (string.IsNullOrEmpty(scenePathOrName))
                return UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            for (var i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                if (scene.path == scenePathOrName || scene.name == scenePathOrName)
                    return scene;
            }
            return default(UnityEngine.SceneManagement.Scene);
        }

        public static string ProjectRelativePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Path is empty.");
            path = path.Replace("\\", "/");
            if (path.StartsWith("Assets/") || path.StartsWith("Packages/"))
                return path;
            var projectRoot = Directory.GetCurrentDirectory().Replace("\\", "/");
            var full = Path.GetFullPath(path).Replace("\\", "/");
            if (full.StartsWith(projectRoot + "/"))
                return full.Substring(projectRoot.Length + 1);
            return path;
        }

        public static object CoerceJToken(JToken token, Type targetType)
        {
            if (targetType == typeof(JToken) || targetType == typeof(object))
                return token;
            if (token == null || token.Type == JTokenType.Null)
                return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
            if (targetType == typeof(string))
                return token.Value<string>();
            if (targetType.IsEnum)
            {
                if (token.Type == JTokenType.String)
                    return Enum.Parse(targetType, token.Value<string>(), true);
                return Enum.ToObject(targetType, token.Value<int>());
            }
            if (targetType == typeof(UnityEngine.Object) || targetType.IsSubclassOf(typeof(UnityEngine.Object)))
            {
                var resolved = ResolveObject(token.ToObject<ObjectRef>());
                if (resolved == null)
                    return null;
                if (!targetType.IsAssignableFrom(resolved.GetType()))
                    throw new InvalidCastException("Object '" + resolved.name + "' is " + resolved.GetType().FullName + ", not " + targetType.FullName + ".");
                return resolved;
            }
            return token.ToObject(targetType);
        }

        public static object[] CoerceArguments(ParameterInfo[] parameters, string argsJson)
        {
            if (parameters.Length == 0)
                return new object[0];
            var token = string.IsNullOrEmpty(argsJson) ? new JArray() : JToken.Parse(argsJson);
            var args = new object[parameters.Length];
            if (token.Type == JTokenType.Array)
            {
                var arr = (JArray)token;
                for (var i = 0; i < parameters.Length; i++)
                    args[i] = i < arr.Count ? CoerceJToken(arr[i], parameters[i].ParameterType) : (parameters[i].HasDefaultValue ? parameters[i].DefaultValue : null);
                return args;
            }
            var obj = (JObject)token;
            for (var i = 0; i < parameters.Length; i++)
            {
                JToken value;
                args[i] = obj.TryGetValue(parameters[i].Name, StringComparison.OrdinalIgnoreCase, out value)
                    ? CoerceJToken(value, parameters[i].ParameterType)
                    : (parameters[i].HasDefaultValue ? parameters[i].DefaultValue : null);
            }
            return args;
        }
    }
}
