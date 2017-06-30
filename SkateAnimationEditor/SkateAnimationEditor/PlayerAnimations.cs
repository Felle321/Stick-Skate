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
			public Animation animation;
			public Vector2 textureOffset, textureOrigin;
		}

		public ReturnValue GetAnimation(string key)
		{
			ReturnValue returnValue = new ReturnValue();

			switch (key)
			{
				case ("Idle"):
					returnValue.animation = new Animation("Player_Idle");
					returnValue.textureOffset = new Vector2(20, 43);
					returnValue.textureOrigin = new Vector2(32, 128);
					break;
				case ("JumpCharge"):
					returnValue.animation = new Animation("Player_JumpCharge");
					returnValue.textureOffset = new Vector2(21, 42);
					returnValue.textureOrigin = new Vector2(32, 128);
					break;
				case ("Jump"):
					returnValue.animation = new Animation("Player_Jump");
					returnValue.textureOffset = new Vector2(21, 42);
					returnValue.textureOrigin = new Vector2(32, 128);
					break;
				case ("KickFlip"):
					returnValue.animation = new Animation("Player_Jump");
					returnValue.animation.speed = .5f;
					returnValue.textureOffset = new Vector2(-1, 0);
					returnValue.textureOrigin = new Vector2(32, 128);
					break;
				default:
					returnValue.animation = new Animation("Player_Idle");
					returnValue.textureOffset = new Vector2(20, 43);
					returnValue.textureOrigin = new Vector2(32, 128);
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
			SpriteHandler.AddSprite("Player_Idle", new Sprite(Content.Load<Texture2D>("idle"), 4, 64, 128));
			SpriteHandler.SetInfo("Player_Idle", 0.1f, 1);
			SpriteHandler.AddSprite("Player_Jump", new Sprite(Content.Load<Texture2D>("jump"), 15, 64, 128));
			SpriteHandler.SetInfo("Player_Jump", 0.1f, 1);
			SpriteHandler.AddSprite("Player_JumpCharge", new Sprite(Content.Load<Texture2D>("jumpCharge"), 4, 64, 128));
			SpriteHandler.SetInfo("Player_JumpCharge", 0.1f, 1);
		}
	}
}
