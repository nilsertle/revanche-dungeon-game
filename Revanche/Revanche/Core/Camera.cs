using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace Revanche.Core
{
    public sealed class Camera
    {
        private const int MapOffset = 3;
        private const int Speed12 = 12;
        private const int Speed15 = 15;
        private const int Speed20 = 20;
        private const int Speed25 = 25;
        private const int EdgeOffset = 100;

        private const float F2 = 2f;
        private const float ZoomThreshold1 = 0.4f;
        private const float ZoomThreshold2 = 2.4f;
        private const float ZoomClampLower = 0.3f;
        private const float ZoomClampUpper = 2.5f;

        [JsonProperty] public Rectangle Bounds { get; private set; }
        [JsonProperty] public Matrix Transform { get; private set; }
        [JsonProperty] public Vector2 Position { get; private set; }
        [JsonProperty] public float Zoom { get; private set; }
        [JsonProperty] public float CameraSpeed { get; private set; }
        [JsonProperty] public bool IsLocked { get; set; }

        internal Rectangle mVisibleArea;
        internal Rectangle mVisibleMap;
        private Matrix mInverse;

        public Camera(Vector2 initPos)
        {
            UpdateBounds();
            this.mVisibleMap = new Rectangle();
            this.mVisibleArea = new Rectangle();
            CameraSpeed = Speed12;
            Zoom = 1f;
            Position = initPos;
            Transform = new Matrix();
            UpdateMatrices();
            UpdateVisible();
        }
        private void UpdateMatrices()
        {
            Transform = Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
                        Matrix.CreateScale(Zoom, Zoom, 1) *
                        Matrix.CreateTranslation(new Vector3(Bounds.Width/F2, Bounds.Height/F2, 0));
            mInverse = Matrix.Invert(Transform);
        }

        private void UpdateBounds()
        {
            Bounds = new Rectangle(new Point(0, 0), new Point(Game1.mScreenWidth, Game1.mScreenHeight));
        }

        /// <summary>
        /// Updates which rectangles are currently visible. Once for the Map Grid and once for the Objects.
        /// </summary>
        private void UpdateVisible()
        {
            var tl = Vector2.Transform(Vector2.Zero, mInverse);
            var br = Vector2.Transform(new Vector2(Bounds.Width, Bounds.Height), mInverse);
            this.mVisibleArea = new Rectangle(new Point((int)tl.X, (int)tl.Y), new Point((int)(br.X - tl.X), (int)(br.Y - tl.Y)));
            this.mVisibleMap = new Rectangle(new Point((int)(tl.X / (Game1.sScaledPixelSize) - 1), (int)(tl.Y / (Game1.sScaledPixelSize)) - 1),
                new Point((int)((br.X - tl.X) / (Game1.sScaledPixelSize) + MapOffset), (int)((br.Y - tl.Y) / (Game1.sScaledPixelSize) + MapOffset)));
        }

        private void MoveCamera(Vector2 pos)
        {
            Position += pos;
        }

        public void AdjustZoom(float zoomDelta)
        {
            Zoom = Math.Clamp(Zoom + zoomDelta, ZoomClampLower, ZoomClampUpper);
        }

        internal void SetCameraPosition(Vector2 pos)
        {
            Position = pos;
        }

        private void Normalize(ref float nX, ref float nY)
        {

            nX = Math.Clamp(nX, -CameraSpeed, CameraSpeed);
            nY = Math.Clamp(nY, -CameraSpeed, CameraSpeed);

        }
        private void AdjustMouse(Point mousePosition, ref Vector2 cameraMovement, float nX,
            float nY)
        {
            if (mousePosition.X <= EdgeOffset)
            {
                cameraMovement.X = -CameraSpeed;
                cameraMovement.Y = nY;
            }
            else if (mousePosition.X >= Bounds.Width - EdgeOffset)
            {
                cameraMovement.X = CameraSpeed;
                cameraMovement.Y = nY;
            }
            if (mousePosition.Y <= EdgeOffset)
            {
                cameraMovement.Y = -CameraSpeed;
                cameraMovement.X = nX;
            }
            else if (mousePosition.Y >= Bounds.Height - EdgeOffset)
            {
                cameraMovement.Y = CameraSpeed;
                cameraMovement.X = nX;
            }
        }

        private void SetCameraSpeed(float zoom)
        {
            CameraSpeed = zoom switch
            {
                <= ZoomThreshold1 => Speed25,
                > ZoomThreshold1 and < ZoomThreshold2 => Speed20,
                >= ZoomThreshold2 => Speed15,
                _ => Speed12
            };
        }

        internal void UpdateCamera()
        {
            UpdateBounds();
            UpdateMatrices();
            UpdateVisible();

            if (!IsLocked)
            {
                var cameraMovement = Vector2.Zero;
                var mousePosition = Mouse.GetState().Position;

                var normalizedX = (float)((-1.0 + 2.0 * mousePosition.X / Game1.mScreenWidth) * CameraSpeed);
                var normalizedY = (float)(-(1.0 - 2.0 * mousePosition.Y / Game1.mScreenHeight) * CameraSpeed);

                Normalize(ref normalizedX, ref normalizedY);
                SetCameraSpeed(Zoom);
                AdjustMouse(mousePosition, ref cameraMovement, normalizedX, normalizedY);
                MoveCamera(cameraMovement);
            }
        }

        /// <summary>
        /// Translates Camera relative coordinates to world coordinates
        /// </summary>
        /// <param name="screenPosition"></param>
        /// <returns></returns>
        public Vector2 CameraToWorld(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition, Matrix.Invert(Transform));
        }

        /// <summary>
        /// Translates the center of a single grid square vector into world coordinates
        /// </summary>
        /// <param name="gridVector"></param>
        public static Vector2 TileCenterToWorld(Vector2 gridVector)
        {
            gridVector.X = (gridVector.X * Game1.sScaledPixelSize) + Game1.sScaledPixelSize / F2;
            gridVector.Y = (gridVector.Y * Game1.sScaledPixelSize) + Game1.sScaledPixelSize / F2;
            return gridVector;
        }

    }
}
