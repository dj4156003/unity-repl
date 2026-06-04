namespace UnityReplGs.Tools.Data
{
    public class PackageData
    {
        public string name;
        public string displayName;
        public string version;
        public string source;
        public string resolvedPath;
        public string assetPath;
        public string packageId;
        public bool isDirectDependency;

        public static PackageData From(UnityEditor.PackageManager.PackageInfo info)
        {
            return new PackageData
            {
                name = info.name,
                displayName = info.displayName,
                version = info.version,
                source = info.source.ToString(),
                resolvedPath = info.resolvedPath,
                assetPath = info.assetPath,
                packageId = info.packageId,
                isDirectDependency = info.isDirectDependency
            };
        }
    }
}
