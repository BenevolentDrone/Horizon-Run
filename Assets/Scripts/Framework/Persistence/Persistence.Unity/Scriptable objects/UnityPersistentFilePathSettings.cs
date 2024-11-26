using UnityEngine;

using System.IO;

namespace HereticalSolutions.Persistence.IO
{
    /// <summary>
    /// File system settings specifying where to locate save files and providing shorthand methods to users
    /// </summary>
    [CreateAssetMenu(fileName = "PersistentFilePathSettings", menuName = "Settings/Serialization/Persistent file path settings", order = 2)]
    public class UnityPersistentFilePathSettings : ScriptableObject
    {
        /// <summary>
        /// Save file's relative path to save folder location
        /// </summary>
        public string RelativePath;

        /// <summary>
        /// Should the file be looked for in the resources folder
        /// </summary>
        public bool LoadFromResourcesFolder;

        /// <summary>
        /// Full file path
        /// </summary>
        public string FullPath
        {
            get
            {
                return LoadFromResourcesFolder
                    ? $"{Application.dataPath}/Resources/{RelativePath}".SanitizePath()
                    : Path.Combine(Application.persistentDataPath, RelativePath).SanitizePath();
            }
        }

        /// <summary>
        /// File path for Resources.Load routine
        /// </summary>
        public string ResourcePath
        {
            get
            {
                int lastIndexOfSeparator = RelativePath.LastIndexOf('.');

                if (lastIndexOfSeparator == -1)
                    return RelativePath;

                return RelativePath.Remove(lastIndexOfSeparator);
            }
        }
    }
}