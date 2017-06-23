using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace Skate
{
	public class Platform
	{
		public enum Type
		{
			Rectangle,
			JumpThrough
		}
		public Type type = Type.Rectangle;
		public Rectangle rectangle;
		public int ID;
		public Vector2 origin;

		public Platform(int ID, Rectangle rectangle, Type type)
		{
			this.ID = ID;
			this.rectangle = rectangle;
			this.type = type;
			this.origin = new Vector2(rectangle.X + rectangle.Width / 2, rectangle.Y + rectangle.Height / 2);
		}

		public void Initialize()
		{

		}

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			Game1.DrawRectangle(spriteBatch, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height), Color.Black);
		}
	}
}
