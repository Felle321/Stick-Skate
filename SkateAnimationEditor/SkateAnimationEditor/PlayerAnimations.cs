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

namespace SkateAnimationEditor
{
	public class PlayerAnimations
	{
		public struct ReturnValue
		{
			public Animation animation, animationWheels;
			public Vector2 textureOffset, wheelsOffset;
		}

		public ReturnValue GetAnimation(string key)
		{
			ReturnValue returnValue = new ReturnValue();

			switch (key)
			{
				case ("Idle"):
					returnValue.animation = new Animation("Player_Idle", 0);
					returnValue.animationWheels = null;
					returnValue.textureOffset = new Vector2(-40, -40);
					returnValue.wheelsOffset = Vector2.Zero;
					break;
				case ("JumpCharge"):
					returnValue.animation = new Animation("Player_JumpCharge", 0);
					returnValue.animationWheels = null;
					returnValue.textureOffset = new Vector2(-40, -40);
					returnValue.wheelsOffset = Vector2.Zero;
					break;
				case ("Jump"):
					returnValue.animation = new Animation("Player_Jump", 0);
					returnValue.animationWheels = null;
					returnValue.textureOffset = new Vector2(-40, -40);
					returnValue.wheelsOffset = Vector2.Zero;
					break;
				default:
					break;
			}
			
			return returnValue;
		}

		/// <summary>
		/// Loads every animation for the player
		/// </summary>
		/// <param name="Content"></param>
		public void LoadContent(ContentManager Content)
		{
			SpriteHandler.AddSprite("Player_Idle", new Sprite(Content.Load<Texture2D>("idle"), 5, 64, 128));
			SpriteHandler.AddSprite("Player_Jump", new Sprite(Content.Load<Texture2D>("jump"), 10, 64, 128));
			SpriteHandler.AddSprite("Player_JumpCharge", new Sprite(Content.Load<Texture2D>("jumpCharge"), 5, 64, 128));
		}
	}
}
