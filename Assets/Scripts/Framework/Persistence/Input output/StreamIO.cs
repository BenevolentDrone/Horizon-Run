using System;
using System.IO;
using System.Text;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
    public static class StreamIO
    {
        public static bool OpenReadStream(
            FilePathSettings settings,
            out FileStream dataStream,
            ILogger logger = null)
        {
            string savePath = settings.FullPath;

            dataStream = default(FileStream);

            if (!FileExists(
                settings.FullPath,
                logger))
                return false;

            dataStream = new FileStream(savePath, FileMode.Open);

            return true;
        }
        
        public static bool OpenReadStream(
            FilePathSettings settings,
            out StreamReader streamReader,
            ILogger logger = null)
        {
            string savePath = settings.FullPath;

            streamReader = default(StreamReader);

            if (!FileExists(
                settings.FullPath,
                logger))
                return false;

            streamReader = new StreamReader(savePath, Encoding.UTF8);

            return true;
        }
        
        public static bool OpenWriteStream(
            FilePathSettings settings,
            out FileStream dataStream,
            ILogger logger = null)
        {
            string savePath = settings.FullPath;

            EnsureDirectoryExists(
                savePath,
                logger);

            //Courtesy of https://stackoverflow.com/questions/7306214/append-lines-to-a-file-using-a-streamwriter
            //Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filemode?view=net-8.0
            dataStream = new FileStream(
                savePath,
                FileMode.Append,
                FileAccess.Write);
                //FileMode.Create);

            return true;
        }
        
        public static bool OpenWriteStream(
            FilePathSettings settings,
            out StreamWriter streamWriter,
            ILogger logger = null)
        {
            string savePath = settings.FullPath;

            EnsureDirectoryExists(
                savePath,
                logger);
            
            //Courtesy of https://stackoverflow.com/questions/7306214/append-lines-to-a-file-using-a-streamwriter
            streamWriter = new StreamWriter(
                savePath,
                //false,
                append: true,
                Encoding.UTF8);

            return true;
        }

        public static void CloseStream(FileStream dataStream)
        {
            dataStream.Close();
        }
        
        public static void CloseStream(StreamReader streamReader)
        {
            streamReader.Close();
        }
        
        public static void CloseStream(StreamWriter streamWriter)
        {
            streamWriter.Close();
        }

        public static void Erase(FilePathSettings settings)
        {
            string savePath = settings.FullPath;

            if (File.Exists(savePath))
                File.Delete(savePath);
        }
        
        private static bool FileExists(
            string path,
            ILogger logger = null)
        {
            if (string.IsNullOrEmpty(path))
                throw new Exception(
                    logger.TryFormatException("INVALID PATH"));
			
            string directoryPath = Path.GetDirectoryName(path);

            if (string.IsNullOrEmpty(directoryPath))
                throw new Exception(
                    logger.TryFormatException("INVALID DIRECTORY PATH"));
			
            if (!Directory.Exists(directoryPath))
            {
                return false;
            }

            return File.Exists(path);
        }
        
        private static void EnsureDirectoryExists(
            string path,
            ILogger logger = null)
        {
            if (string.IsNullOrEmpty(path))
                throw new Exception(
                    logger.TryFormatException("INVALID PATH"));
			
            string directoryPath = Path.GetDirectoryName(path);

            if (string.IsNullOrEmpty(directoryPath))
                throw new Exception(
                    logger.TryFormatException("INVALID DIRECTORY PATH"));
			
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }
    }
}