using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace SkateAnimationEditor
{
	public static class SpriteHandler
	{
		public static Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();

		/// <summary>
		/// Loads and adds a sprite to the SpriteHandler
		/// </summary>
		/// <param name="key">The key which will be used as a reference</param>
		/// <param name="sprite">The sprite to add</param>
		public static void AddSprite(string key, Sprite sprite)
		{
			sprites.Add(key, sprite);
		}

		/// <summary>
		/// Sets some basic info about the sprite
		/// </summary>
		/// <param name="key"></param>
		/// <param name="speed"></param>
		/// <param name="scale"></param>
		public static void SetInfo(string key, float speed, float scale)
		{
			sprites[key].speed = speed;
			sprites[key].scale = scale;
		}

		/// <summary>
		/// Draws the sprite with the given key
		/// </summary>
		/// <param name="key">The key which decides what sprite to be used</param>
		/// <param name="rand"></param>
		/// <param name="spriteBatch"></param>
		/// <param name="camera"></param>
		/// <param name="position">The position of the sprite</param>
		public static void Draw(string key, SpriteBatch spriteBatch, Camera camera, Vector2 position, SpriteEffects spriteEffect, float depth)
		{
			sprites[key].Draw(spriteBatch, camera, position, spriteEffect, depth);
		}

		/// <summary>
		/// Draws the sprite with the given key
		/// </summary>
		/// <param name="key">The key which decides what sprite to be used</param>
		/// <param name="rand"></param>
		/// <param name="spriteBatch"></param>
		/// <param name="camera"></param>
		/// <param name="position">The position of the sprite</param>
		/// <param name="vector2Scale">The Vector2 scale of the sprite</param>
		public static void Draw(string key, SpriteBatch spriteBatch, Camera camera, Vector2 position, Vector2 vector2Scale, SpriteEffects spriteEffect, float depth)
		{
			sprites[key].Draw(spriteBatch, camera, position, vector2Scale, spriteEffect, depth);
		}

		internal static void SetSpeed()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Draws the sprite with the given key
		/// </summary>
		/// <param name="key">The key which decides what sprite to be used</param>
		/// <param name="rand"></param>
		/// <param name="spriteBatch"></param>
		/// <param name="camera"></param>
		/// <param name="position">The position of the sprite</param>
		/// <param name="scale"></param>
		/// <param name="angle"></param>
		/// <param name="color"></param>
		/// <param name="origin"></param>
		public static void Draw(string key, SpriteBatch spriteBatch, Camera camera, Vector2 position, float scale, float angle, Vector2 origin, Color color, float opacity, SpriteEffects spriteEffect, float depth)
		{
			sprites[key].Draw(spriteBatch, camera, position, scale, angle, origin, color, opacity, spriteEffect, depth);
		}

		/// <summary>
		/// Draws the sprite with the given key
		/// </summary>
		/// <param name="key">The key which decides what sprite to be used</param>
		/// <param name="rand"></param>
		/// <param name="spriteBatch"></param>
		/// <param name="camera"></param>
		/// <param name="position">The position of the sprite</param>
		/// <param name="vector2Scale"></param>
		/// <param name="angle"></param>
		/// <param name="color"></param>
		/// <param name="origin"></param>
		public static void Draw(string key, SpriteBatch spriteBatch, Camera camera, Vector2 position, Vector2 vector2Scale, float angle, Vector2 origin, Color color, float opacity, SpriteEffects spriteEffect, float depth)
		{
			sprites[key].Draw(spriteBatch, camera, position, vector2Scale, angle, origin, color, opacity, spriteEffect, depth);
		}


		public static Sprite InstantiateSprite(string key)
		{
			Sprite oldSpr = sprites[key];
			Sprite spr = new Sprite(oldSpr.textures, oldSpr.frames, oldSpr.width, oldSpr.height);
			spr.angle = oldSpr.angle;
			spr.color = oldSpr.color;
			spr.scale = oldSpr.scale;
			spr.speed = oldSpr.speed;
			spr.origin = oldSpr.origin;
			spr.HUD = oldSpr.HUD;
			spr.framesTotal = oldSpr.framesTotal;
			spr.opacity = oldSpr.opacity;

			return spr;
		}
	}
}
