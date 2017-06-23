using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Skate
{
	public class Solid
	{
		public Point a, b;
		public float k;

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

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(Game1.pixel)
		}
	}
}