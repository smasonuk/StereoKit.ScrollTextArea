using SkiaSharp;
using StereoKit;
using System;

namespace ScrollTextArea
{
    /// <summary>
    /// Crops an image, it only crops in horizontal slices which is only what we need for vertical scrolling.
    /// </summary>
    public class TextureCropper
    {
        private byte[] _imageBytes;
        private int _width;

        public TextureCropper(SKBitmap bmp)
        {
            _imageBytes = bmp.GetPixelSpan().ToArray();
            _width = bmp.Width;
        }

        public byte[] GetAll()
        {
            return _imageBytes;
        }

        public byte[] GetCroppedHorizontalSlice(int rowNum, int numberOfRows)
        {
            var bytesPerPixel = 4;
            var rowLength = _width;
            var startPixel = rowLength * rowNum;
            var numberOfPixels = numberOfRows * rowLength;
            var numberOfBytes = numberOfPixels * bytesPerPixel;

            var croppedImageBytes = new byte[numberOfBytes];

            Array.Copy(_imageBytes, startPixel * bytesPerPixel, croppedImageBytes, 0, numberOfBytes);
            return croppedImageBytes;
        }





    }
}
