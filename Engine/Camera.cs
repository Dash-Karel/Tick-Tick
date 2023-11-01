using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Engine
{
    public class Camera : GameObject
    {
        /// <summary>
        /// The WorldSize, this needs to be updated when changes to the ExtendedGame.WorldSize are made
        /// </summary>
        public Point WorldSize { private get; set; }

        /// <summary>
        /// The speed at which the camera moves towards the subject
        /// </summary>
        public float Speed { get; set; } = 2.5f;
        
        /// <summary>
        /// CenterPosition is the location of the center of the cameraview
        /// </summary>
        public Vector2 CenterPosition
        {
            get
            {
                return new Vector2(GlobalPosition.X + Origin.X, GlobalPosition.Y + Origin.Y);
            }
        }

        /// <summary>
        /// the size of the area that will be shown
        /// </summary>
        public Point Size { get; private set; }

        /// <summary>
        /// The center of the size window
        /// </summary>
        private Point Origin
        {
            get
            {
                return new Point(Size.X / 2, Size.Y / 2);
            }
        }

        public SpriteGameObject Subject { private get; set; }
        
        public Camera(Point cameraSize, Point worldSize)
        {
            Visible = false;
            Size = cameraSize;
            WorldSize = worldSize;
        }

        public override void Update(GameTime gameTime)
        {
            if (Subject != null)
            {
                velocity = (Subject.GlobalPosition - CenterPosition) * Speed;
                float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                localPosition = Vector2.Clamp(localPosition + velocity * deltaTime, Vector2.Zero , (WorldSize - Size).ToVector2());
            }
            else
                localPosition = Vector2.Zero;
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
