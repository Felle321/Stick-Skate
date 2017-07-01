using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Skate
{
	public class Camera
	{
		public float zoom, zoomTarget; // Camera Zoom
		public Matrix transform; // Matrix Transform
		public Vector2 pos; // Camera Position
		public Vector2 posPrev; // Camera Position
		public Vector2 movement = new Vector2(0, 0);
		public Rectangle rectangle = new Rectangle(0, 0, 0, 0);
		float rotation = 0;

		public Vector2 target;

		private Vector2 shakeOffset = Vector2.Zero;
		public List<Vector2> shakes = new List<Vector2>();
		public List<int> shakeDurations = new List<int>();

		public void AddShake(Vector2 magnitude, int duration)
		{
			shakes.Add(magnitude);
			shakeDurations.Add(duration);
		}

		public void StopShake()
		{
			shakes = new List<Vector2>();
			shakeDurations = new List<int>();
		}

		public void Move(Vector2 newMove)
		{
			pos += newMove;
			target += newMove;
		}

		public Camera(Vector2 newPos, float zoom)
		{
			this.zoom = zoom;
			pos = newPos;
			posPrev = newPos;
		}

		public void Update(Random rand)
		{
			if (Vector2.Distance(pos, target) < 1)
			{
				pos = target;
				movement = Vector2.Zero;
			}
			else
			{
				movement.X = (target.X - pos.X) * .12f;
				movement.Y = (target.Y - pos.Y) * .12f;

				pos += movement;
			}

			if (Math.Abs(zoomTarget - zoom) < .001)
				zoom = zoomTarget;
			else
			{
				if (zoomTarget - zoom < 0)
					zoom -= .0005f;
				else
					zoom += .0005f;
			}

			shakeOffset = Vector2.Zero;

			for (int i = 0; i < shakes.Count; i++)
			{
				shakeOffset += new Vector2(shakes[i].X * ((float)rand.NextDouble() * 2 - 1), shakes[i].Y * ((float)rand.NextDouble() * 2 - 1));
				shakeDurations[i]--;
				if (shakeDurations[i] <= 0)
				{
					shakeDurations.RemoveAt(i);
					shakes.RemoveAt(i);
				}
			}

			pos += shakeOffset;

			if (pos.X == float.NaN)
				pos.X = posPrev.X;
			if (pos.Y == float.NaN)
				pos.Y = posPrev.Y;

			rectangle = new Rectangle((int)(pos.X - Game1.screenWidth * .5f / zoom) - 32, (int)(pos.Y - Game1.screenHeight * .5f / zoom) - 32, (int)(Game1.screenWidth / zoom) + 64, (int)(Game1.screenHeight / zoom) + 64);

			posPrev = pos;
		}

		public Matrix get_transformation(GraphicsDevice graphicsDevice)
		{
			transform =       // Thanks to o KB o for this solution
				Matrix.CreateTranslation(new Vector3(-pos.X, -pos.Y, 0)) *
				Matrix.CreateRotationZ(rotation) *
				Matrix.CreateScale(new Vector3(zoom, zoom, 1)) *
				Matrix.CreateTranslation(new Vector3(Game1.screenWidth * 0.5f, Game1.screenHeight * 0.5f, 0));
			return transform;
		}
	}
}
