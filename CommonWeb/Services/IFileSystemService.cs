using System;
using System.Collections.Generic;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace HanumanInstitute.CommonWeb
{
    /// <summary>
    /// Extends IFileSystem with a few extra IO functions. IFileSystem provides wrappers around all IO methods.
    /// </summary>
    public interface IFileSystemService
    {
        /// <summary>
        /// Opens an existing file for reading text.
        /// </summary>
        /// <param name="path">The path to be opened for reading.</param>
        /// <returns>The opened file TextReader.</returns>
        TextReader OpenFileText(string path);
        /// <summary>
        /// Opens an existing file for reading.
        /// </summary>
        /// <param name="path">The path to be opened for reading.</param>
        /// <returns>The opened file stream.</returns>
        Stream OpenFileStream(string path);
        /// <summary>
        /// Creates or overwrites a file in the specified path.
        /// </summary>
        /// <param name="path">The path of the file to create.</param>
        /// <returns>The created file stream.</returns>
        Stream CreateFileStream(string path);
        /// <summary>
        /// Ensures the directory of specified path exists. If it doesn't exist, creates the directory.
        /// </summary>
        /// <param name="path">The absolute path to validate.</param>
        void EnsureDirectoryExists(string path);
        /// <summary>
        /// Deletes a file if it exists.
        /// </summary>
        /// <param name="path">The path of the file to delete.</param>
        void DeleteFileSilent(string path);
        /// <summary>
        /// Returns all files of specified extensions.
        /// </summary>
        /// <param name="path">The path in which to search.</param>
        /// <param name="extensions">A list of file extensions to return, each extension must include the dot.</param>
        /// <param name="searchOption">Specifies additional search options.</param>
        /// <returns>A list of files paths matching search conditions.</returns>
        //IEnumerable<string> GetFilesByExtensions(string path, string[] extensions, SearchOption searchOption = SearchOption.TopDirectoryOnly);
        /// <summary>
        /// Saves an image to specified file path with the image format defined by the file extension.
        /// </summary>
        /// <typeparam name="TPixel">The pixel format.</typeparam>
        /// <param name="image">The image to save to disk.</param>
        /// <param name="filePath">The file path to save the image to.</param>
        void SaveImage<TPixel>(Image<TPixel> image, string filePath)
            where TPixel : struct, IPixel<TPixel>;
    }
}
