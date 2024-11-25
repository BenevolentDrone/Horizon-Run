using System;
using System.IO;
using System.Threading.Tasks;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
    [SerializationStrategy]
    public class FileStreamStrategy
        : ISerializationStrategy,
          IAsyncSerializationStrategy,
          IStrategyWithIODestination,
          IStrategyWithStream,
          IStrategyWithState,
          IBlockSerializationStrategy,
          IAsyncBlockSerializationStrategy
    {
        private static readonly Type[] allowedValueTypes = new Type[]
        {
            typeof(byte[])
        };

        private readonly bool flushAutomatically;

        private readonly ILogger logger;


        public string FullPath { get; private set; }


        private FileStream fileStream;

        public FileStream FileStream { get { return fileStream; } }


        public FileStreamStrategy(
            string fullPath,
            bool flushAutomatically = false,
            ILogger logger = null)
        {
            FullPath = fullPath;

            this.flushAutomatically = flushAutomatically;

            this.logger = logger;


            CurrentMode = ESreamMode.NONE;

            StreamOpen = false;

            fileStream = default(FileStream);
        }


        #region ISerializationStrategy

        public Type[] AllowedValueTypes { get => allowedValueTypes; }

        #region Read

        public bool Read<TValue>(
            out TValue value)
        {
            AssertStrategyIsValid(
                typeof(TValue),
                ESreamMode.READ);

            //Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

            // Read the source file into a byte array.
            byte[] result = new byte[fileStream.Length];

            int numBytesToRead = (int)fileStream.Length;

            int numBytesRead = 0;

            while (numBytesToRead > 0)
            {
                // Read may return anything from 0 to numBytesToRead.
                int n = fileStream.Read(
                    result,
                    numBytesRead,
                    numBytesToRead);

                // Break when the end of the file is reached.
                if (n == 0)
                    break;

                numBytesRead += n;

                numBytesToRead -= n;
            }

            numBytesToRead = result.Length;

            value = result.CastFromTo<byte[], TValue>();

            return true;
        }

        public bool Read(
            Type valueType,
            out object value)
        {
            AssertStrategyIsValid(
                valueType,
                ESreamMode.READ);

            //Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

            // Read the source file into a byte array.
            byte[] result = new byte[fileStream.Length];

            int numBytesToRead = (int)fileStream.Length;

            int numBytesRead = 0;

            while (numBytesToRead > 0)
            {
                // Read may return anything from 0 to numBytesToRead.
                int n = fileStream.Read(
                    result,
                    numBytesRead,
                    numBytesToRead);

                // Break when the end of the file is reached.
                if (n == 0)
                    break;

                numBytesRead += n;

                numBytesToRead -= n;
            }

            numBytesToRead = result.Length;

            value = result;

            return true;
        }

        #endregion

        #region Write

        public bool Write<TValue>(
            TValue value)
        {
            AssertStrategyIsValid(
                typeof(TValue),
                ESreamMode.WRITE);

            byte[] contents = value.CastFromTo<TValue, byte[]>();

            //Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

            int numBytesToWrite = contents.Length;

            // Write the byte array to the other FileStream.
            fileStream.Write(
                contents,
                0,
                numBytesToWrite);

            if (flushAutomatically)
                Flush();

            return true;
        }

        public bool Write(
            Type valueType,
            object value)
        {
            AssertStrategyIsValid(
                valueType,
                ESreamMode.WRITE);

            byte[] contents = value.CastFromTo<object, byte[]>();

            //Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

            int numBytesToWrite = contents.Length;

            // Write the byte array to the other FileStream.
            fileStream.Write(
                contents,
                0,
                numBytesToWrite);

            if (flushAutomatically)
                Flush();

            return true;
        }

        #endregion

        #region Append

        public bool Append<TValue>(
            TValue value)
        {
            AssertStrategyIsValid(
                typeof(TValue),
                ESreamMode.APPEND);

            byte[] contents = value.CastFromTo<TValue, byte[]>();

            //Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

            int numBytesToWrite = contents.Length;

            // Write the byte array to the other FileStream.
            fileStream.Write(
                contents,
                0,
                numBytesToWrite);

            if (flushAutomatically)
                Flush();

            return true;
        }

        public bool Append(
            Type valueType,
            object value)
        {
            AssertStrategyIsValid(
                valueType,
                ESreamMode.READ);

            byte[] contents = value.CastFromTo<object, byte[]>();

            //Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

            int numBytesToWrite = contents.Length;

            // Write the byte array to the other FileStream.
            fileStream.Write(
                contents,
                0,
                numBytesToWrite);

            if (flushAutomatically)
                Flush();

            return true;
        }

        #endregion

        #endregion

        #region IAsyncSerializationStrategy

        #region Read

        public async Task<(bool, TValue)> ReadAsync<TValue>()
        {
            AssertStrategyIsValid(
                typeof(TValue),
                ESreamMode.READ);

            //Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

            // Read the source file into a byte array.
            byte[] result = new byte[fileStream.Length];

            int numBytesToRead = (int)fileStream.Length;

            int numBytesRead = 0;

            while (numBytesToRead > 0)
            {
                // Read may return anything from 0 to numBytesToRead.
                int n = await fileStream.ReadAsync(
                    result,
                    numBytesRead,
                    numBytesToRead);

                // Break when the end of the file is reached.
                if (n == 0)
                    break;

                numBytesRead += n;

                numBytesToRead -= n;
            }

            numBytesToRead = result.Length;

            TValue value = result.CastFromTo<byte[], TValue>();

            return (true, value);
        }

        public async Task<(bool, object)> ReadAsync(
            Type valueType)
        {
            AssertStrategyIsValid(
                valueType,
                ESreamMode.READ);

            //Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

            // Read the source file into a byte array.
            byte[] result = new byte[fileStream.Length];

            int numBytesToRead = (int)fileStream.Length;

            int numBytesRead = 0;

            while (numBytesToRead > 0)
            {
                // Read may return anything from 0 to numBytesToRead.
                int n = await fileStream.ReadAsync(
                    result,
                    numBytesRead,
                    numBytesToRead);

                // Break when the end of the file is reached.
                if (n == 0)
                    break;

                numBytesRead += n;

                numBytesToRead -= n;
            }

            numBytesToRead = result.Length;

            object value = result;

            return (true, value);
        }

        #endregion

        #region Write

        public async Task<bool> WriteAsync<TValue>(
            TValue value)
        {
            AssertStrategyIsValid(
                typeof(TValue),
                ESreamMode.WRITE);

            byte[] contents = value.CastFromTo<TValue, byte[]>();

            //Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

            int numBytesToWrite = contents.Length;

            // Write the byte array to the other FileStream.
            await fileStream.WriteAsync(
                contents,
                0,
                numBytesToWrite);

            if (flushAutomatically)
                await FlushAsync();

            return true;
        }

        public async Task<bool> WriteAsync(
            Type valueType,
            object value)
        {
            AssertStrategyIsValid(
                valueType,
                ESreamMode.WRITE);

            byte[] contents = value.CastFromTo<object, byte[]>();

            //Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

            int numBytesToWrite = contents.Length;

            // Write the byte array to the other FileStream.
            await fileStream.WriteAsync(
                contents,
                0,
                numBytesToWrite);

            if (flushAutomatically)
                await FlushAsync();

            return true;
        }

        #endregion

        #region Append

        public async Task<bool> AppendAsync<TValue>(
            TValue value)
        {
            AssertStrategyIsValid(
                typeof(TValue),
                ESreamMode.APPEND);

            byte[] contents = value.CastFromTo<TValue, byte[]>();

            //Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

            int numBytesToWrite = contents.Length;

            // Write the byte array to the other FileStream.
            await fileStream.WriteAsync(
                contents,
                0,
                numBytesToWrite);

            if (flushAutomatically)
                await FlushAsync();

            return true;
        }

        public async Task<bool> AppendAsync(
            Type valueType,
            object value)
        {
            AssertStrategyIsValid(
                valueType,
                ESreamMode.APPEND);

            byte[] contents = value.CastFromTo<object, byte[]>();

            //Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

            int numBytesToWrite = contents.Length;

            // Write the byte array to the other FileStream.
            await fileStream.WriteAsync(
                contents,
                0,
                numBytesToWrite);

            if (flushAutomatically)
                await FlushAsync();

            return true;
        }

        #endregion

        #endregion

        #region IStrategyWithIODestination

        public void EnsureIOTargetDestinationExists()
        {
            IOHelpers.EnsureDirectoryExists(
                FullPath,
                logger,
                GetType());
        }

        public bool IOTargetExists()
        {
            return IOHelpers.FileExists(
                FullPath,
                logger,
                GetType());
        }

        public void CreateIOTarget()
        {
            string savePath = FullPath;

            IOHelpers.EnsureDirectoryExists(
                FullPath,
                logger,
                GetType());

            if (!IOHelpers.FileExists(
                savePath,
                logger,
                GetType()))
            {
                File.Create(
                    FullPath);
            }
        }

        public void EraseIOTarget()
        {
            string savePath = FullPath;

            if (IOHelpers.FileExists(
                savePath,
                logger,
                GetType()))
            {
                File.Delete(savePath);
            }
        }

        #endregion

        #region IStrategyWithStream

        public ESreamMode CurrentMode { get; private set; }

        public Stream Stream
        {
            get
            {
                switch (CurrentMode)
                {
                    case ESreamMode.READ:
                    case ESreamMode.WRITE:
                    case ESreamMode.APPEND:
                        return fileStream;

                    default:
                        return null;
                }
            }
        }

        public bool StreamOpen { get; private set; }

        #region Flush

        public bool FlushAutomatically { get => flushAutomatically; }

        public void Flush()
        {
            if (!StreamOpen)
                return;

            if (fileStream != null)
            {
                fileStream.Flush(true);
            }
        }

        public async Task FlushAsync()
        {
            if (!StreamOpen)
                return;

            if (fileStream != null)
            {
                await fileStream.FlushAsync();
            }
        }

        #endregion


        #region Seek

        public long Position
        {
            get
            {
                if (!StreamOpen)
                    return -1;

                if ((CurrentMode == ESreamMode.READ || CurrentMode == ESreamMode.WRITE || CurrentMode == ESreamMode.APPEND)
                    && fileStream != null)
                {
                    return fileStream.Position;
                }

                return -1;
            }
        }

        public bool CanSeek
        {
            get
            {
                if (!StreamOpen)
                    return false;

                if ((CurrentMode == ESreamMode.READ ||CurrentMode == ESreamMode.WRITE || CurrentMode == ESreamMode.APPEND)
                    && fileStream != null)
                {
                    return fileStream.CanSeek;
                }

                return false;
            }
        }

        public bool Seek(
            long offset,
            out long position)
        {
            if (!StreamOpen)
            {
                position = -1;

                return false;
            }

            if ((CurrentMode == ESreamMode.READ || CurrentMode == ESreamMode.WRITE || CurrentMode == ESreamMode.APPEND)
                && fileStream != null)
            {
                position = fileStream.Seek(
                    offset,
                    SeekOrigin.Current);

                return true;
            }

            position = -1;

            return false;
        }

        public bool SeekFromStart(
            long offset,
            out long position)
        {
            if (!StreamOpen)
            {
                position = -1;

                return false;
            }

            if ((CurrentMode == ESreamMode.READ || CurrentMode == ESreamMode.WRITE || CurrentMode == ESreamMode.APPEND)
                && fileStream != null)
            {
                position = fileStream.Seek(
                    offset,
                    SeekOrigin.Begin);

                return true;
            }

            position = -1;

            return false;
        }

        public bool SeekFromFinish(
            long offset,
            out long position)
        {
            if (!StreamOpen)
            {
                position = -1;

                return false;
            }

            if ((CurrentMode == ESreamMode.READ || CurrentMode == ESreamMode.WRITE || CurrentMode == ESreamMode.APPEND)
                && fileStream != null)
            {
                position = fileStream.Seek(
                    offset,
                    SeekOrigin.End);

                return true;
            }

            position = -1;

            return false;
        }

        #endregion

        #endregion

        #region IStrategyWithState

        public void InitializeRead()
        {
            if (StreamOpen)
                return;

            StreamOpen = OpenReadStream(
                out fileStream);

            if (!StreamOpen)
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"FAILED TO OPEN STREAM: {FullPath}"));

            CurrentMode = ESreamMode.READ;
        }

        public void FinalizeRead()
        {
            if (!StreamOpen)
                return;

            if (CurrentMode != ESreamMode.READ)
                return;

            if (fileStream != null)
                CloseStream(
                    fileStream);

            fileStream = default(FileStream);

            StreamOpen = false;

            CurrentMode = ESreamMode.NONE;
        }

        public void InitializeWrite()
        {
            if (StreamOpen)
                return;

            StreamOpen = OpenWriteStream(
                out fileStream);

            if (!StreamOpen)
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"FAILED TO OPEN STREAM: {FullPath}"));

            CurrentMode = ESreamMode.WRITE;
        }

        public void FinalizeWrite()
        {
            if (!StreamOpen)
                return;

            if (CurrentMode != ESreamMode.WRITE)
                return;

            if (fileStream != null)
                CloseStream(
                    fileStream);

            fileStream = default(FileStream);

            StreamOpen = false;

            CurrentMode = ESreamMode.NONE;
        }

        public void InitializeAppend()
        {
            if (StreamOpen)
                return;

            StreamOpen = OpenAppendStream(
                out fileStream);

            if (!StreamOpen)
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"FAILED TO OPEN STREAM: {FullPath}"));

            CurrentMode = ESreamMode.APPEND;
        }

        public void FinalizeAppend()
        {
            if (!StreamOpen)
                return;

            if (CurrentMode != ESreamMode.APPEND)
                return;

            if (fileStream != null)
                CloseStream(
                    fileStream);

            fileStream = default(FileStream);

            StreamOpen = false;

            CurrentMode = ESreamMode.NONE;
        }

        #endregion

        #region IBlockSerializationStrategy

        #region Read

        public bool BlockRead<TValue>(
            int blockOffset,
            int blockSize,
            out TValue value)
        {
            AssertStrategyIsValid(
                typeof(TValue),
                ESreamMode.READ);

            // Read the source file into a byte array.
            byte[] result = new byte[blockSize];

            int resultLength = fileStream.Read(
                result,
                0,
                blockSize);

            if (resultLength != blockSize)
            {
                byte[] resultTrimmed = new byte[resultLength];

                Array.Copy(
                    result,
                    resultTrimmed,
                    resultLength);

                result = resultTrimmed;
            }

            value = result.CastFromTo<byte[], TValue>();

            return true;
        }

        public bool BlockRead(
            Type valueType,
            int blockOffset,
            int blockSize,
            out object value)
        {
            AssertStrategyIsValid(
                valueType,
                ESreamMode.READ);

            // Read the source file into a byte array.
            byte[] result = new byte[blockSize];

            int resultLength = fileStream.Read(
                result,
                0,
                blockSize);

            if (resultLength != blockSize)
            {
                byte[] resultTrimmed = new byte[resultLength];

                Array.Copy(
                    result,
                    resultTrimmed,
                    resultLength);

                result = resultTrimmed;
            }

            value = result;

            return true;
        }

        #endregion

        #region Write

        public bool BlockWrite<TValue>(
            TValue value,
            int blockOffset,
            int blockSize)
        {
            AssertStrategyIsValid(
                typeof(TValue),
                ESreamMode.WRITE);

            byte[] contents = value.CastFromTo<TValue, byte[]>();

            fileStream.Write(
                contents,
                blockOffset,
                blockSize);

            if (flushAutomatically)
                Flush();

            return true;
        }

        public bool BlockWrite(
            Type valueType,
            object value,
            int blockOffset,
            int blockSize)
        {
            AssertStrategyIsValid(
                valueType,
                ESreamMode.WRITE);

            byte[] contents = value.CastFromTo<object, byte[]>();

            fileStream.Write(
                contents,
                blockOffset,
                blockSize);

            if (flushAutomatically)
                Flush();

            return true;
        }

        #endregion

        #endregion

        #region IAsyncBlockSerializationStrategy

        #region Read

        public async Task<(bool, TValue)> BlockReadAsync<TValue>(
            int blockOffset,
            int blockSize)
        {
            AssertStrategyIsValid(
                typeof(TValue),
                ESreamMode.READ);

            // Read the source file into a byte array.
            byte[] result = new byte[blockSize];

            int resultLength = await fileStream.ReadAsync(
                result,
                0,
                blockSize);

            if (resultLength != blockSize)
            {
                byte[] resultTrimmed = new byte[resultLength];

                Array.Copy(
                    result,
                    resultTrimmed,
                    resultLength);

                result = resultTrimmed;
            }

            TValue value = result.CastFromTo<byte[], TValue>();

            return (true, value);
        }

        public async Task<(bool, object)> BlockReadAsync(
            Type valueType,
            int blockOffset,
            int blockSize)
        {
            AssertStrategyIsValid(
                valueType,
                ESreamMode.READ);

            // Read the source file into a byte array.
            byte[] result = new byte[blockSize];

            int resultLength = await fileStream.ReadAsync(
                result,
                0,
                blockSize);

            if (resultLength != blockSize)
            {
                byte[] resultTrimmed = new byte[resultLength];

                Array.Copy(
                    result,
                    resultTrimmed,
                    resultLength);

                result = resultTrimmed;
            }

            object value = result;

            return (true, value);
        }

        #endregion

        #region Write

        public async Task<bool> BlockWriteAsync<TValue>(
            TValue value,
            int blockOffset,
            int blockSize)
        {
            AssertStrategyIsValid(
                typeof(TValue),
                ESreamMode.WRITE);

            byte[] contents = value.CastFromTo<TValue, byte[]>();

            await fileStream.WriteAsync(
                contents,
                blockOffset,
                blockSize);

            if (flushAutomatically)
                await FlushAsync();

            return true;
        }

        public async Task<bool> BlockWriteAsync(
            Type valueType,
            object value,
            int blockOffset,
            int blockSize)
        {
            AssertStrategyIsValid(
                valueType,
                ESreamMode.WRITE);

            byte[] contents = value.CastFromTo<object, byte[]>();

            await fileStream.WriteAsync(
                contents,
                blockOffset,
                blockSize);

            if (flushAutomatically)
                await FlushAsync();

            return true;
        }

        #endregion

        #endregion

        private void AssertStrategyIsValid(
            Type valueType,
            ESreamMode preferredMode)
        {
            if (valueType != typeof(byte[]))
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"INVALID VALUE TYPE: {valueType.Name}"));

            if (!StreamOpen)
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"STREAM NOT OPEN: {FullPath}"));

            if (CurrentMode != preferredMode)
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"INVALID STREAM MODE: {CurrentMode}"));

            if (fileStream == null)
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"FILE STREAM IS NULL: {FullPath}"));
        }

        private bool OpenReadStream(
            out FileStream dataStream)
        {
            dataStream = default(FileStream);

            if (!IOHelpers.FileExists(
                FullPath,
                logger,
                GetType()))
            {
                return false;
            }

            dataStream = new FileStream(
                FullPath,
                FileMode.Open);

            return true;
        }

        private bool OpenWriteStream(
            out FileStream dataStream)
        {
            IOHelpers.EnsureDirectoryExists(
                FullPath,
                logger,
                GetType());

            //Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filemode?view=net-8.0
            dataStream = new FileStream(
                FullPath,
                FileMode.Create,
                FileAccess.Write);

            return true;
        }

        private bool OpenAppendStream(
            out FileStream dataStream)
        {
            IOHelpers.EnsureDirectoryExists(
                FullPath,
                logger,
                GetType());

            //Courtesy of https://stackoverflow.com/questions/7306214/append-lines-to-a-file-using-a-fileStream
            //Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filemode?view=net-8.0
            dataStream = new FileStream(
                FullPath,
                FileMode.Append,
                FileAccess.Write);

            return true;
        }

        private void CloseStream(FileStream dataStream)
        {
            dataStream.Close();
        }
    }
}