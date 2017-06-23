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
		float speed = 5f;
		Vector2 movement;
		public bool onGround = false;
		public int solidRef;
		public float angle = 0f;

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
				speed -= .9f;

				if (speed < 3)
					speed = 10f;
			}
			else
				movement.Y += Game1.gravity;

			position += movement;
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(texture, new Vector2(Centre.X - texture.Width / 2, Rectangle.Bottom - texture.Height - offset), Color.White);
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