namespace FileMonitoring
{
    /// <summary>
    /// Monitors a directory and retrieves metadata for the files within it.
    /// </summary>
    public class DirectoryMonitor
    {
        /// <summary>
        /// Retrieves a list of file metadata from the specified directory,
        /// sorted by the last modified date in descending order.
        /// </summary>
        /// <param name="path">The absolute path to the directory to monitor.</param>
        /// <param name="debugMode">If true, prints errors to the console.</param>
        /// <returns>A sorted list of FileMetadata objects.</returns>
        public List<FileMetadata> GetDirectoryListing(string path, bool debugMode = false)
        {
            var fileMetadataList = new List<FileMetadata>();

            if (!Directory.Exists(path))
            {
                if (debugMode)
                {
                    Console.WriteLine($"Error: Directory not found - {path}");
                }
                return fileMetadataList;
            }

            try
            {
                var files = Directory.GetFiles(path);
                foreach (var file in files)
                {
                    long size = FileDetailsRetriever.GetActualFileSize(file);
                    DateTime lastModified = FileDetailsRetriever.GetLastWriteTimeUtc(file);
                    string lastModifiedStrFormatted = lastModified == DateTime.MinValue ? "??" : lastModified.ToString("HH:mm:ss");

                    fileMetadataList.Add(new FileMetadata
                    {
                        Name = Path.GetFileName(file),
                        FullPath = file,
                        SizeInBytes = size == -1 ? "INACCESSIBLE" : size.ToString(),
                        LastModifiedUtc = lastModifiedStrFormatted
                    });

                    if (size == -1 && debugMode)
                    {
                        Console.WriteLine($"Note: '{Path.GetFileName(file)}' is currently locked or inaccessible. It will be listed, but size/date may be unavailable until the lock is released.");
                    }
                }
            }
            catch (Exception ex)
            {
                if (debugMode)
                {
                    Console.WriteLine($"An error occurred while accessing directory '{path}': {ex.Message}");
                }
                // Return an empty list or whatever has been collected so far
                return fileMetadataList;
            }

            // Sort the list by newest file first
            return fileMetadataList.OrderByDescending(f => f.LastModifiedUtc).ToList();
        }
    }
}
