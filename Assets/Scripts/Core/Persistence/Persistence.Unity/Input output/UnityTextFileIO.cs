using System;
using System.IO;

using UnityEngine;

namespace HereticalSolutions.Persistence.IO
{
    public static class UnityTextFileIO
    {
        public static bool Write(
            UnityPersistentFilePathSettings settings,
            string contents)
        {
            string savePath = settings.FullPath;

            EnsureDirectoryExists(savePath);

            File.WriteAllText(savePath, contents);

            return true;
        }

        public static bool Write(
            UnityPersistentFilePathSettings settings,
            byte[] contents)
        {
            string savePath = settings.FullPath;

            EnsureDirectoryExists(savePath);

            File.WriteAllBytes(savePath, contents);

            return true;
        }
        
        public static bool Read(
            UnityPersistentFilePathSettings settings,
            out string contents)
        {
            string savePath = settings.FullPath;

            contents = string.Empty;
			
            if (!FileExists(savePath))
                return false;
			
            contents = settings.LoadFromResourcesFolder
                ? Resources.Load<TextAsset>(settings.ResourcePath).text
                : File.ReadAllText(savePath);

            return true;
        }
        
        public static bool Read(
            UnityPersistentFilePathSettings settings,
            out byte[] contents)
        {
            string savePath = settings.FullPath;

            contents = null;
			
            if (!FileExists(savePath))
                return false;

            if (settings.LoadFromResourcesFolder)
            {
                TextAsset textAsset = Resources.Load(settings.ResourcePath) as TextAsset;
                
                MemoryStream memoryStream = new MemoryStream(textAsset.bytes);

                contents = memoryStream.ToArray();
            }
            else
            {
                contents = File.ReadAllBytes(savePath);
            }

            return true;
        }
        
        public static void Erase(UnityPersistentFilePathSettings settings)
        {
            string savePath = settings.FullPath;

            if (File.Exists(savePath))
                File.Delete(savePath);
        }
        
        /// <summary>
        /// Checks whether the file at the specified path exists
        /// - Also makes sure the folder directory specified in the path actually exists anyway
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <returns>Does the file exist</returns>
        private static bool FileExists(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new Exception("[UnityTextFileIO] INVALID PATH");
			
            string directoryPath = Path.GetDirectoryName(path);

            if (string.IsNullOrEmpty(directoryPath))
                throw new Exception("[UnityTextFileIO] INVALID DIRECTORY PATH");
			
            if (!Directory.Exists(directoryPath))
            {
                return false;
            }

            return File.Exists(path);
        }

        private static void EnsureDirectoryExists(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new Exception("[UnityTextFileIO] INVALID PATH");
			
            string directoryPath = Path.GetDirectoryName(path);

            if (string.IsNullOrEmpty(directoryPath))
                throw new Exception("[UnityTextFileIO] INVALID DIRECTORY PATH");
			
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }
    }
}