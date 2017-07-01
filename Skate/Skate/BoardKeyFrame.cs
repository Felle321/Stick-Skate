using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Skate
{
	public class BoardKeyFrame
	{
		public int frame;
		public float scaleX, scaleY, rotation;
		public Vector2 position;
		public bool animationFreeze, drawSimpleBoard;

		public BoardKeyFrame(int frame, float scaleX, float scaleY, float rotation, Vector2 position, bool animationFreeze, bool drawSimpleBoard)
		{
			this.frame = frame;
			this.scaleX = scaleX;
			this.scaleY = scaleY;
			this.rotation = rotation;
			this.position = position;
			this.animationFreeze = animationFreeze;
			this.drawSimpleBoard = drawSimpleBoard;
		}

		public string GetCommand()
		{
			return "keyFrames.Add(new BoardKeyFrame(" + frame + ", " + Game1.FloatToString(scaleX) + ", " + Game1.FloatToString(scaleY) + ", " + Game1.FloatToString(rotation) + ", new Vector2(" + Game1.FloatToString(position.X) + ", " + Game1.FloatToString(position.Y) + "), " + animationFreeze.ToString().ToLower() + ", " + drawSimpleBoard.ToString().ToLower() + "));";
		}
	}
}
