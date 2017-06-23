using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Skate
{
	public class Player
	{
		public Vector2 position;
		public int width = 40;
		public int height = 40;
		public Rectangle Rectangle
		{
			get
			{
				return new Rectangle((int)Math.Round(position.X), (int)Math.Round(position.Y), width, height);
			}
		}
		public Vector2 Centre
		{
			get
			{
				return position + new Vector2(width / 2, height / 2);
			}
		}
		int offset = 0;
		float speed = 25f;
		public Vector2 movement;
		public bool onGround, onGroundPrev, onSlope, fallThrough = false;
		public Slope slope;
		public int solidRef, platform, platformPrev;
		public float angle = 0f;
		public float bounceFactor = 0f;
		int minSpeed = 12;
		Color color = Color.Black;

		public Texture2D texture;

		public Player( Texture2D texture)
		{
			this.texture = texture;
		}

		public void Update()
		{
			if (onGround)
			{
				movement.X = speed;

				if (speed < minSpeed)
					speed = minSpeed;

				if (onSlope)
				{
					angle = -slope.angle;
					speed -= slope.k;
				}
				else
					angle = 0;
			}
			else
			{
				movement.Y += Game1.gravity;

				if (Math.Abs(angle) < .02f)
					angle = 0;
				else if (angle < 0)
					angle += 0.001f;
				else
					angle -= 0.001f;
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(texture, new Vector2(Centre.X, Rectangle.Bottom - offset), null, Color.White, angle, new Vector2(texture.Width / 2, texture.Height), 1f, SpriteEffects.None, 0f);
		}

		internal void SetContactYPos(float y)
		{
			position.Y = y - texture.Height - offset;
		}

		internal void SetGround(bool ground)
		{
			this.onGround = ground;

			if (ground)
				offset = 0;
		}
	}
}