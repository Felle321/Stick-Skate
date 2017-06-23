using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Skate
{
	public class Player
	{
		public Vector2 position;
		public int width, height;
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

		public Texture2D texture;

		public Player( Texture2D texture)
		{
			this.texture = texture;
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(texture, new Vector2(Centre.X - texture.Width / 2, Rectangle.Bottom - texture.Height - offset), Color.White);
		}
	}
}