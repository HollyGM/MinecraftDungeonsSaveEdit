using System;
using System.IO;
using System.Threading.Tasks;
#nullable enable

namespace MCDSaveEdit
{
    public static class SafeFileWriter
    {
        /// <summary>
        /// Writes <paramref name="stream"/> to <paramref name="filePath"/> without ever leaving a partially written file behind.
        /// The data goes to a temporary file first, which then replaces the destination.
        /// When <paramref name="backupFilePath"/> is given and the destination already exists, the previous contents are kept there.
        /// </summary>
        public static async Task writeAsync(Stream stream, string filePath, string? backupFilePath = null)
        {
            var fullPath = Path.GetFullPath(filePath);
            var tempPath = fullPath + ".tmp";
            try
            {
                stream.Seek(0, SeekOrigin.Begin);
                using (var tempStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await stream.CopyToAsync(tempStream);
                    await tempStream.FlushAsync();
                }

                if (!File.Exists(fullPath))
                {
                    File.Move(tempPath, fullPath);
                    return;
                }

                try
                {
                    File.Replace(tempPath, fullPath, backupFilePath, ignoreMetadataErrors: true);
                }
                catch (Exception e) when (e is IOException || e is PlatformNotSupportedException)
                {
                    //File.Replace is not supported by every file system, so fall back to copying
                    if (File.Exists(fullPath))
                    {
                        if (backupFilePath != null)
                        {
                            File.Copy(fullPath, backupFilePath, overwrite: true);
                        }
                        File.Copy(tempPath, fullPath, overwrite: true);
                    }
                    else
                    {
                        //File.Replace already moved the original out of the way
                        File.Move(tempPath, fullPath);
                    }
                }
            }
            finally
            {
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
            }
        }
    }
}
