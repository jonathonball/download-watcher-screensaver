namespace FileMonitoring
{
    /// <summary>
    /// Provides methods to retrieve up-to-the-moment details about a file.
    /// </summary>
    public static class FileDetailsRetriever
    {
        /// <summary>
        /// Gets the actual size of the file on disk at the moment of the call.
        /// This method attempts to open a read-only stream to the file to determine
        /// its current length, which reflects the data written so far, rather than
        /// the final allocated size.
        /// </summary>
        /// <param name="filePath">The full path to the file.</param>
        /// <returns>The current size of the file in bytes. Returns -1 if the file is inaccessible.</returns>
        public static long GetActualFileSize(string filePath)
        {
            try
            {
                // By opening a stream with Read access and ReadWrite share, we can inspect the file
                // even while another process is writing to it. The stream's length will represent
                // the amount of data actually written and readable at this exact moment.
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    return fileStream.Length;
                }
            }
            catch (IOException)
            {
                // The file may be locked in a way that prevents even shared reading,
                // or it might be deleted between the directory listing and this call.
                return -1;
            }
            catch (Exception)
            {
                // Catch other potential exceptions, e.g., unauthorized access.
                return -1;
            }
        }

        /// <summary>
        /// Gets the last write time (UTC) of a file.
        /// </summary>
        /// <param name="filePath">The full path to the file.</param>
        /// <returns>The DateTime object representing the last write time in UTC.</returns>
        public static DateTime GetLastWriteTimeUtc(string filePath)
        {
            try
            {
                return File.GetLastWriteTimeUtc(filePath);
            }
            catch (Exception)
            {
                // In case of an access error, return a minimal date value.
                return DateTime.MinValue;
            }
        }
    }
}
