using StereoKit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Topten.RichTextKit;

namespace ScrollTextArea
{

    /// <summary>
    /// A scollable text window. Can be scrolled by eye-gaze, or finger.
    /// </summary>
    public class ScrollingTextArea
    {
        private readonly ScrollingTexture _scroller;

        /// <summary>
        /// The amount of space behind the image which will accept move events from the finger. 
        /// </summary>
        public float TouchAreaThickness { get; private set; } = 0.05f;

        public bool ShowDebug { get; set; }

        /// <summary>
        /// The last point that the user touched.
        /// </summary>
        private Point? _lastPoint = null;
        private DateTime? _timeSinceLastEyeScroll;
        private DateTime? _timeSinceHandScroll;


        public ScrollingTextArea(RichString rs, int width, int windowVerticalSize)
        {
            var skimage = new TextureTextWriter().CreateImageFromText(rs, width);
            var cropper = new TextureCropper(skimage);
            
            _scroller = new ScrollingTexture(cropper, 
                totalImageWidth: skimage.Width, 
                totalImageHeight: skimage.Height, 
                windowVerticalSize: windowVerticalSize);
        }


        public ScrollingTextArea(ScrollingTexture scroller)
        {
            _scroller = scroller;
        }

        /// <summary>
        /// Add the scolling image to the UI
        /// </summary>
        /// <param name="windowPose">the pose of the window this image will be added to</param>
        /// <param name="widthInWorld">width in real world dimentions (metres)</param>
        /// <param name="heightInWorld">height in real world dimentions (metres)</param>
        /// <returns></returns>
        public Point? AddScroller(Pose windowPose, float widthInWorld, float heightInWorld)
        {
            var startPosImage = UI.LayoutAt;
            var sprite = _scroller.GetSprite();
            UI.Image(sprite, new Vec2(widthInWorld, heightInWorld));

            //find the x location at the rightmost of the image
            UI.SameLine();
            var endXImage = UI.LayoutAt.x;

            //find the y location under the image
            UI.NextLine();
            var endYImage = UI.LayoutAt.y;
            var endZImage = UI.LayoutAt.z;

            startPosImage = startPosImage + new Vec3(0, 0, TouchAreaThickness);
            var end = new Vec3(endXImage, endYImage, endZImage + -0.01f);

            //find out if there is a finger near the surface of the image

            UI.ShowVolumes = ShowDebug;
            var bounds = Bounds.FromCorners(end, startPosImage);
            var inside = UI.VolumeAt("fingerTouchArea", bounds, UIConfirm.Push, out var handed);
            UI.ShowVolumes = false;


            //is a hand inside the volume in front of the image?
            if (inside.IsActive())
            {
                
                HandJoint fingerTip = Input.Hand(handed).Get(FingerId.Index, JointId.Tip);

                var fingerPosition = fingerTip.Pose.position;

                var (pos, _) = PositionInImage(windowPose, widthInWorld, heightInWorld, startPosImage, fingerPosition);

                //if the user is dragging their finger through the volume, then scroll the text by the same amounnt of y pixel changes
                if (_lastPoint != null)
                {
                    _timeSinceHandScroll = DateTime.Now;

                    var yDiff = _lastPoint.Value.Y - pos.Y;
                    _scroller.MoveWindowRelative(yDiff);

                    //useful for debugging
                    _scroller.DrawPixel(pos.X, pos.Y);
                    _lastPoint = pos;
                    _timeSinceLastEyeScroll = null;
                    return pos;
                } 
                else
                {
                    _lastPoint = pos;
                }

            }
            else
            {
                _lastPoint = null;

                //dont immediately scroll if the hand is removed
                if (_timeSinceHandScroll != null)
                {
                    var diff = DateTime.Now - _timeSinceHandScroll.Value;
                    if (diff.TotalMilliseconds < 1500)
                    {
                        return null;
                    }
                }

                _timeSinceHandScroll = null;

                //if not finger scrolling, then check for eye scrolling
                DoEyeScrolling(windowPose, widthInWorld, heightInWorld, startPosImage);
            }

            return null;
        }


        /// <summary>
        /// If eye tracking is available then find out where in the image the user is looking, and scroll the text accordingly.
        /// </summary>
        /// <returns></returns>
        private Point? DoEyeScrolling(Pose windowPose, float widthInWorld, float heightInWorld, Vec3 startPosImage)
        {
            if (Input.EyesTracked.IsActive())
            {
                var eyeray = Input.Eyes.Ray;
                Plane p = new Plane(windowPose.position, windowPose.Forward);

                if (eyeray.Intersect(p, out var pointEyesAreIntersecting))
                {
                    //pointEyesAreIntersecting.
                    var (posEyeIsLookingAt, outside) = PositionInImage(windowPose, widthInWorld, heightInWorld, startPosImage, pointEyesAreIntersecting);

                    if (!outside)
                    {
                        //Note: needs a bit of cleaning up.
                        var buffer = _scroller.WindowVerticalSize / 3f;
                        var timeDiff = DateTime.Now - _timeSinceLastEyeScroll;
                        var moveAmount = 2;
                        if (timeDiff != null)
                        {
                            var m = (timeDiff.Value.TotalSeconds * 300);
                            moveAmount = (int)m;
                        }

                        if ( posEyeIsLookingAt.Y < buffer) 
                        {
                            _scroller.MoveWindowRelative(-moveAmount);

                            _timeSinceLastEyeScroll = DateTime.Now;
                        } 
                        else if ( posEyeIsLookingAt.Y > (_scroller.WindowVerticalSize - buffer))
                        {
                            _scroller.MoveWindowRelative(moveAmount);

                            _timeSinceLastEyeScroll = DateTime.Now;
                        }
                    }

                    return posEyeIsLookingAt;
                }
            }

            _timeSinceLastEyeScroll = null;
            return null;
        }

        /// <summary>
        /// From a position in world space, find the position in the image in pixels
        /// </summary>
        /// <returns></returns>
        private (Point, bool) PositionInImage(Pose windowPose, float widthInWorld, float heightInWorld, Vec3 startPosImage, Vec3 pointInWorld)
        {
            var rotateFingerTopBackToOriginMatrix = windowPose.ToMatrix().Inverse;

            //the position of the finger relative to the window, where 0,0 is actually at the middle of the topmost of the window
            //and the negative direction is to the right, and negative is down
            var positionOfFingerRelativeToWindowCentered = rotateFingerTopBackToOriginMatrix.Transform(pointInWorld);

            ///map these coordinates to the image coordinate system. (0,0) is at the top left of the image, and positive y direction downwards.
            //flip the signs on the xaxis since the are the wrong way around
            var x = -positionOfFingerRelativeToWindowCentered.x;
            var y = -positionOfFingerRelativeToWindowCentered.y;

            //adjust so that the image is at 0,0 (the top left of the window was at 0,0)
            var adjustedx = x + startPosImage.x;
            var adjustedy = y + startPosImage.y;

            //calculate percentage into the image in both axis (from 0 to 1)
            var xPerc = (float)adjustedx / (float)widthInWorld;
            var yPerc = (float)adjustedy / (float)heightInWorld;

            //calculate exact pixel the user's finger is over
            var actualX = _scroller.Width * xPerc;
            var actualY = _scroller.WindowVerticalSize * yPerc;

            var farOutside = false;

            //adjust incase we have gone over the edge of the image
            if (actualX > _scroller.Width)
            {
                if (actualX > _scroller.Width + 0.01f)
                {
                    farOutside = true;
                }
                actualX = _scroller.Width;
            }
            if (actualY > _scroller.WindowVerticalSize)
            {
                if (actualY > _scroller.WindowVerticalSize + 0.01f)
                {
                    farOutside = true;
                }
                actualY = _scroller.WindowVerticalSize;
            }
            if (actualY < 0)
            {
                if (actualY < -0.01f)
                {
                    farOutside = true;
                }
                actualY = 0;
            }
            if (actualX < 0)
            {
               
                if (actualX < -0.01f)
                {
                    farOutside = true;
                }
                actualX = 0;
            }

            var pos = new Point { X = (int)actualX, Y = (int)actualY };
            return (pos, farOutside);
        }
    }
}

