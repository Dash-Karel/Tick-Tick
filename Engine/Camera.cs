using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine
{
    public class Camera
    {
        /// <summary>
        /// Everything within this rectangle will be shown/rendered
        /// </summary>
        Rectangle cameraView;

        /// <summary>
        /// CenterPosition is the location of the center of the cameraview
        /// </summary>
        public Vector2 CenterPosition
        {
            get
            {
                return new Vector2(cameraView.X - Origin.X, cameraView.Y - Origin.Y);
            }
        }
        public Vector2 Position
        {
            get
            {
                return new Vector2(cameraView.X, cameraView.Y);
            }
            set
            {
                cameraView.X = (int)value.X;
                cameraView.Y = (int)value.Y;
            }
        }
        public Point Size
        {
            get
            {
                return cameraView.Size;
            }
        }
        private Point Origin
        {
            get
            {
                return new Point(cameraView.Width / 2, cameraView.Height / 2);
            }
        }
        
        public Camera(Point cameraSize)
        {
            cameraView = new Rectangle(Point.Zero, cameraSize);
        }

        public Viewport CalculateViewPort(Point windowSize)
        {
            // create a Viewport object
            Viewport viewport = new Viewport();

            // calculate the two aspect ratios
            float cameraAspectRatio = (float)Size.X / Size.Y;
            float windowAspectRatio = (float)windowSize.X / windowSize.Y;

            // if the window is relatively wide, use the full window height
            if (windowAspectRatio > cameraAspectRatio)
            {
                viewport.Width = (int)(windowSize.Y * cameraAspectRatio);
                viewport.Height = windowSize.Y;
            }
            // if the window is relatively high, use the full window width
            else
            {
                viewport.Width = windowSize.X;
                viewport.Height = (int)(windowSize.X / cameraAspectRatio);
            }

            // calculate and store the top-left corner of the viewport
            viewport.X = (windowSize.X - viewport.Width) / 2;
            viewport.Y = (windowSize.Y - viewport.Height) / 2;

            return viewport;
        }
    }
}
