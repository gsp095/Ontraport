using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace HanumanInstitute.CommonWeb
{
    /// <summary>
    /// Provides extension methods to manage images.
    /// </summary>
    public static class ImageExtensions
    {
        /// <summary>
        /// Resizes the image within specified maximum dimensions.
        /// </summary>
        /// <param name="maxWidth">The maximum width.</param>
        /// <param name="maxHeight">The maximum height.</param>
        public static void ResizeToArea<TPixel>(this Image<TPixel> image, int maxWidth, int maxHeight, bool upscale = false) where TPixel : struct, IPixel<TPixel>
        {
            image.CheckNotNull(nameof(image));
            maxWidth.CheckRange(nameof(maxWidth), 1);
            maxHeight.CheckRange(nameof(maxHeight), 1);

            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);
            // Note: ratio can be > 1 for small images, then we skip resize.
            if (ratio < 1 || upscale)
            {
                var newWidth = (int)(image.Width * ratio);
                var newHeight = (int)(image.Height * ratio);

                image.Mutate(p => p.Resize(newWidth, newHeight));
            }
        }

        /// <summary>
        /// Returns the size of an image resized to fix within an area while preserving proportions.
        /// </summary>
        /// <param name="width">The image width.</param>
        /// <param name="height">The image height.</param>
        /// <param name="maxWidth">The area width.</param>
        /// <param name="maxHeight">The area height.</param>
        /// <returns></returns>
        public static Size CalculateResizeToArea(int width, int height, int maxWidth, int maxHeight)
        {
            width.CheckRange(nameof(width), 1);
            height.CheckRange(nameof(height), 1);
            maxWidth.CheckRange(nameof(maxWidth), 1);
            maxHeight.CheckRange(nameof(maxHeight), 1);

            var ratioX = (double)maxWidth / width;
            var ratioY = (double)maxHeight / height;
            var ratio = Math.Min(ratioX, ratioY);
            return new Size((int)(width * ratio), (int)(height * ratio));
        }
    }
}
