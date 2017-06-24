using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Skate
{
	public class Slope
	{
		public Rectangle rectangle;
		public bool faceRight;
		public float angle;
		public float k = 0;
		public int platformID = -1;

		public Slope(Rectangle newRect, bool faceRight)
		{
			rectangle = newRect;
			this.faceRight = faceRight;
			if (faceRight)
			{
				angle = (float)Math.Tanh(rectangle.Height / rectangle.Width);
			}
			else
			{
				angle = MathHelper.ToRadians(180) - (float)Math.Tanh(rectangle.Height / rectangle.Width);
			}

			k = rectangle.Height / ((float)rectangle.Width);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			Game1.DrawLine(spriteBatch, new Vector2(rectangle.X, rectangle.Y + rectangle.Height), new Vector2(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height), Color.Red, 8);
			if (faceRight)
			{
				Game1.DrawLine(spriteBatch, new Vector2(rectangle.X, rectangle.Y + rectangle.Height), new Vector2(rectangle.X + rectangle.Width, rectangle.Y), Color.Red, 8);
				Game1.DrawLine(spriteBatch, new Vector2(rectangle.X + rectangle.Width, rectangle.Y), new Vector2(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height), Color.Red, 8);
			}
			else
			{
				Game1.DrawLine(spriteBatch, new Vector2(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height), new Vector2(rectangle.X, rectangle.Y), Color.Red, 8);
				Game1.DrawLine(spriteBatch, new Vector2(rectangle.X, rectangle.Y), new Vector2(rectangle.X, rectangle.Y + rectangle.Height), Color.Red, 8);
			}
		}

		public float Function(float x)
		{
			if (x < 0)
			{
				return 0;
			}
			else if (x > rectangle.Width)
			{
				return -rectangle.Height;
			}
			else
			{
				if (faceRight)
				{
					return k * x;
				}
				else
				{
					return -k * x;
				}
			}
		}
	}
}