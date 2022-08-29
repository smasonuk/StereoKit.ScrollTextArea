using StereoKit;

namespace ScrollTextArea
{
    /// <summary>
    /// Vertical scrolling window that displays a texture.
    /// </summary>
    public class ScrollingTexture
    {
        private TextureCropper _cropper;
        private int _windowVerticalSize;

        /// <summary>
        /// The width of the image in pixels.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// The total height of the image in pixels (ignoring the window size).
        /// </summary>
        public int TotalHeight { get; private set; }

        /// <summary>
        /// The height of the image in pixels that is visible in the window.
        /// </summary>
        public int WindowVerticalSize
        {
            get
            {
                return _windowVerticalSize;
            }
        }

        private int _currentRow = 0;
        private Tex _texture = new Tex();
        private byte[] _imageBytes;
        private Sprite _oldSprite;
        private bool _changedWindow = true;

        public ScrollingTexture(TextureCropper cropper, int totalImageWidth, int totalImageHeight, int windowVerticalSize)
        {
            Create(cropper, totalImageWidth, totalImageHeight, windowVerticalSize);
        }

        private void Create(TextureCropper cropper, int width, int height, int windowVerticalSize)
        {
            Width = width;
            TotalHeight = height;

            _cropper = cropper;

            if (windowVerticalSize > TotalHeight)
            {
                _windowVerticalSize = TotalHeight;
            }
            else
            {
                _windowVerticalSize = windowVerticalSize;
            }
        }

        private byte[] GetPixelWindow()
        {
            var cropped = _cropper.GetCroppedHorizontalSlice(_currentRow, _windowVerticalSize);
            return cropped;
        }

        public Sprite GetSprite()
        {
            if (_changedWindow)
            {
                _imageBytes = GetPixelWindow();
                _texture.SetColors(Width, _windowVerticalSize, _imageBytes);
                _oldSprite = Sprite.FromTex(_texture, SpriteType.Single);
                _changedWindow = false;
            }

            return _oldSprite;
        }


        /// <summary>
        /// Useful for debugging
        /// </summary>
        internal void DrawPixel(int x, int y)
        {
            if (x >= 0 && x < Width && y >= 0 && y < WindowVerticalSize)
            {
                var start = (((y * Width) ) + x) * 4;

                _imageBytes[start] = 0;
                _imageBytes[start + 1] = 0;
                _imageBytes[start + 2] = 0;
                _imageBytes[start + 3] = 255;

                _texture.SetColors(Width, _windowVerticalSize, _imageBytes);
                _oldSprite = Sprite.FromTex(_texture, SpriteType.Single);
            }

        }

        public bool MoveWindowRelative(int relativeRows)
        {
            _changedWindow = true;
            _currentRow += relativeRows;
            if (_currentRow < 0)
            {
                _currentRow = 0;
                return false;
            }
            else if (_currentRow > TotalHeight - _windowVerticalSize)
            {
                _currentRow = TotalHeight - _windowVerticalSize;
                return false;
            }

            return true;
        }

        public bool MoveWindowTo(int row)
        {
            _changedWindow = true;
            if (row < 0)
            {
                _currentRow = 0;
                return false;
            }

            if (row > TotalHeight - _windowVerticalSize)
            {
                _currentRow = TotalHeight - _windowVerticalSize;
                return false;
            }

            _currentRow = row;
            return true;
        }

    }
}

