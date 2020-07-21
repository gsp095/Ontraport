using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace HanumanInstitute.CommonWeb
{
    /// <summary>
    /// Extends FileSystem with a few extra IO functions. FileSystem provides wrappers around all IO methods.
    /// </summary>
    public class FileSystemService : IFileSystemService
    {
        public FileSystemService() { }

        /// <summary>
        /// Opens an existing file for reading text.
        /// </summary>
        /// <param name="path">The path to be opened for reading.</param>
        /// <returns>The opened file TextReader.</returns>
        public TextReader OpenFileText(string path) => File.OpenText(path);

        /// <summary>
        /// Opens an existing file for reading.
        /// </summary>
        /// <param name="path">The path to be opened for reading.</param>
        /// <returns>The opened file stream.</returns>
        public Stream OpenFileStream(string path) => File.OpenRead(path);

        /// <summary>
        /// Creates or overwrites a file in the specified path.
        /// </summary>
        /// <param name="path">The path of the file to create.</param>
        /// <returns>The created file stream.</returns>
        public Stream CreateFileStream(string path) => File.Create(path);

        /// <summary>
        /// Ensures the directory of specified path exists. If it doesn't exist, creates the directory.
        /// </summary>
        /// <param name="path">The absolute path to validate.</param>
        public void EnsureDirectoryExists(string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        }

        /// <summary>
        /// Deletes a file if it exists.
        /// </summary>
        /// <param name="path">The path of the file to delete.</param>
        public void DeleteFileSilent(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// Returns all files of specified extensions.
        /// </summary>
        /// <param name="path">The path in which to search.</param>
        /// <param name="extensions">A list of file extensions to return, each extension must include the dot.</param>
        /// <param name="searchOption">Specifies additional search options.</param>
        /// <returns>A list of files paths matching search conditions.</returns>
        //public IEnumerable<string> GetFilesByExtensions(string path, string[] extensions, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        //{
        //    try
        //    {
        //        return Directory.EnumerateFiles(path, "*", searchOption).Where(f => extensions.Any(s => f.EndsWith(s, System.StringComparison.InvariantCultureIgnoreCase)));
        //    }
        //    catch 
        //    {
        //        return new string[] { };
        //    }
        //}

        /// <summary>
        /// Saves an image to specified file path with the image format defined by the file extension.
        /// </summary>
        /// <typeparam name="TPixel">The pixel format.</typeparam>
        /// <param name="image">The image to save to disk.</param>
        /// <param name="filePath">The file path to save the image to.</param>
        public void SaveImage<TPixel>(Image<TPixel> image, string filePath)
            where TPixel : struct, IPixel<TPixel>
        {
            image.Save(filePath);
        }
    }
}
