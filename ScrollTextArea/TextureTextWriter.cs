using SkiaSharp;
using StereoKit;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Topten.RichTextKit;

namespace ScrollTextArea
{
    /// <summary>
    /// Writes a RichSTring to an SkBitmap or a Sprite
    /// </summary>
    public class TextureTextWriter
    {
        public TextureTextWriter() 
        {

        }

        public SKBitmap CreateImageFromText(RichString text, int maxWidth, SKColor? backgroundColor=null, int atX =0, int atY=0)
        {
            text.MaxWidth = maxWidth;

            if (backgroundColor == null)
            {
                backgroundColor = SKColors.White;
            }
            
            //create image
            var info = new SKImageInfo((int)Math.Ceiling(text.MaxWidth.Value), (int)Math.Ceiling(text.MeasuredHeight), SKColorType.Rgba8888, SKAlphaType.Premul);
            var bmp = new SKBitmap(info);
            var canvas = new SKCanvas(bmp);
            
            canvas.Clear(backgroundColor.Value);
            text.Paint(canvas, new SKPoint(atX, atY));

            return bmp;
        }


        public static Sprite CreateSpriteFromText(RichString text, int maxWidth,  SKColor? backgroundColor = null, int atX = 0, int atY = 0)
        {
            text.MaxWidth = maxWidth;

            if (backgroundColor == null)
            {
                backgroundColor = SKColors.White;
            }

            //create image
            var info = new SKImageInfo((int)Math.Ceiling(text.MaxWidth.Value), (int)Math.Ceiling(text.MeasuredHeight), SKColorType.Rgba8888, SKAlphaType.Premul);
            var bmp = new SKBitmap(info);
            var canvas = new SKCanvas(bmp);

            canvas.Clear(backgroundColor.Value);

            text.Paint(canvas, new SKPoint(atX, atY));

            //convert to sprite
            var imageBytes = bmp.GetPixelSpan().ToArray();
            var tx = new Tex();
            tx.SetColors(bmp.Width, bmp.Height, imageBytes);
            var sp = Sprite.FromTex(tx);
            
            return sp;
        }
        

    }
}
