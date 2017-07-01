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
	public class PlayerAnimation
	{
		public Animation animation;
		public Vector2 textureOffset, textureOrigin;

		public static PlayerAnimation GetAnimation(string key)
		{
			PlayerAnimation returnValue = new PlayerAnimation();

			switch (key)
			{
				case ("Idle"):
					returnValue.animation = new Animation("Player_Idle");
					returnValue.animation.speed = .3f;
					returnValue.textureOffset = new Vector2(0, 0);
					returnValue.textureOrigin = new Vector2(32, 128);
					break;
				case ("JumpCharge"):
					returnValue.animation = new Animation("Player_JumpCharge");
					returnValue.animation.speed = 0;
					returnValue.textureOffset = new Vector2(0, 0);
					returnValue.textureOrigin = new Vector2(32, 128);
					break;
				case ("Jump"):
					returnValue.animation = new Animation("Player_Jump");
					returnValue.animation.speed = .7f;
					returnValue.textureOffset = new Vector2(0, 0);
					returnValue.textureOrigin = new Vector2(32, 128);
					break;
				case ("KickFlip"):
					returnValue.animation = new Animation("Player_Jump");
					returnValue.animation.speed = .7f;
					returnValue.textureOffset = new Vector2(0, 0);
					returnValue.textureOrigin = new Vector2(32, 128);
					break;
				case ("Accelerate"):
					returnValue.animation = new Animation("Player_Accelerate");
					returnValue.animation.speed = .7f;
					returnValue.textureOffset = new Vector2(0, 10);
					returnValue.textureOrigin = new Vector2(32, 128);
					break;
				case ("50-50"):
				returnValue.animation = new Animation("Grind_FiftyFifty");
					returnValue.animation.speed = .3f;
					returnValue.textureOffset = new Vector2(0, 0);
					returnValue.textureOrigin = new Vector2(32, 128);
					break;
				default:
					returnValue.animation = new Animation("Player_Idle");
					returnValue.animation.speed = .1f;
					returnValue.textureOffset = new Vector2(0, 0);
					returnValue.textureOrigin = new Vector2(32, 128);
					break;
			}

			return returnValue;
		}

		/// <summary>
		/// Loads every animation for the player
		/// </summary>
		/// <param name="Content"></param>
		public static void LoadContent(ContentManager Content)
		{
			SpriteHandler.AddSprite("Player_Idle", new Sprite(Content.Load<Texture2D>("idle"), 4, 64, 128));
			SpriteHandler.AddSprite("Player_Jump", new Sprite(Content.Load<Texture2D>("jump"), 15, 64, 128));
			SpriteHandler.AddSprite("Player_JumpCharge", new Sprite(Content.Load<Texture2D>("jumpCharge"), 8, 64, 128));
			SpriteHandler.AddSprite("Player_Accelerate", new Sprite(Content.Load<Texture2D>("accelerate"), 15, 64, 128));
			SpriteHandler.AddSprite("Grind_FiftyFifty", new Sprite(Content.Load<Texture2D>("Grind_FiftyFifty"), 4, 64, 128));
		}
	}
}
