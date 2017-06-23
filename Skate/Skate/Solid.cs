using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Skate
{
	public class Solid
	{
		public Point a, b;
		public float k;
		public bool isWall;

		/// <summary>
		/// A line with two positions acting as a Solid
		/// </summary>
		/// <param name="a"> a.X less than or equal too b.X </param>
		/// <param name="b"></param>
		public Solid(Point a, Point b)
		{
			this.a = a;
			this.b = b;

			if (b.X - a.X == 0)
				k = float.NaN;
			else if (b.Y - a.Y == 0)
				k = 0;
			else
				k = (b.Y - a.Y) / (b.X - a.X);
		}

		/// <summary>
		/// Returns the Y-pos for a given X-value
		/// </summary>
		/// <param name="x">Not relative to Solid</param>
		/// <returns></returns>
		public float GetY(float x)
		{
			if (k == 0)
				return a.Y;
			else
			{
				if (x < a.X)
					return a.Y;
				else if (x < b.X)
					return b.X;
				else
				{
					return k * (x - a.X);
				}
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(Game1.pixel, a.ToVector2(), null, Color.Red, (float)Math.Atan(k), Vector2.Zero, new Vector2(new Vector2(b.X - a.X, b.Y - a.Y).Length(), 5), SpriteEffects.None, 0f);
		}
	}
}