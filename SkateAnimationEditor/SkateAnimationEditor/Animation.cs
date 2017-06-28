﻿using System;
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
	public class Animation
	{
		public float currentTexture = 0;
		public float currentFrame = 0;
		public float framesTotal = 0;
		public float speed = 1;
		public string sprite = "";
		public float scale = 1;
		public int Width { get { return SpriteHandler.sprites[sprite].width; } set { } }
		public int Height { get { return SpriteHandler.sprites[sprite].height; } set { } }

		public bool animationEnd = false;
		public Animation(string key)
		{
			if (key == null)
				return;
			sprite = key;
			framesTotal = SpriteHandler.sprites[key].framesTotal;
			speed = SpriteHandler.sprites[key].speed;
		}

		public Animation(string key, int startingFrame)
		{
			sprite = key;
			currentFrame = startingFrame;
			framesTotal = SpriteHandler.sprites[key].framesTotal;
			speed = SpriteHandler.sprites[key].speed;
		}

		/// <summary>
		/// Draws the sprite with the given key
		/// </summary>
		/// <param name="key">The key which decides what sprite to be used</param>
		/// <param name="rand"></param>
		/// <param name="spriteBatch"></param>
		/// <param name="camera"></param>
		/// <param name="position">The position of the sprite</param>
		public void Draw(SpriteBatch spriteBatch, Camera camera, Vector2 position, SpriteEffects spriteEffect, float depth)
		{
			if (currentTexture == SpriteHandler.sprites[sprite].textures.Count - 1 && ((int)Math.Floor(currentFrame) == SpriteHandler.sprites[sprite].frames[(int)Math.Floor(currentTexture)] || !SpriteHandler.sprites[sprite].spriteSheet[(int)Math.Floor(currentTexture)]))
				animationEnd = true;
			else
				animationEnd = false;
			SpriteHandler.sprites[sprite].Draw(spriteBatch, camera, position, this, spriteEffect, depth);
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
		public void Draw(SpriteBatch spriteBatch, Camera camera, Vector2 position, Vector2 vector2Scale, SpriteEffects spriteEffect, float depth)
		{
			if (currentTexture == SpriteHandler.sprites[sprite].textures.Count - 1 && ((int)Math.Floor(currentFrame) == SpriteHandler.sprites[sprite].frames[(int)Math.Floor(currentTexture)] || !SpriteHandler.sprites[sprite].spriteSheet[(int)Math.Floor(currentTexture)]))
				animationEnd = true;
			else
				animationEnd = false;
			SpriteHandler.sprites[sprite].Draw(spriteBatch, camera, position, vector2Scale, this, spriteEffect, depth);
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
		public void Draw(SpriteBatch spriteBatch, Camera camera, Vector2 position, float scale, float angle, Vector2 origin, Color color, float opacity, SpriteEffects spriteEffect, float depth)
		{
			if (currentTexture == SpriteHandler.sprites[sprite].textures.Count - 1 && ((int)Math.Floor(currentFrame) == SpriteHandler.sprites[sprite].frames[(int)Math.Floor(currentTexture)] || !SpriteHandler.sprites[sprite].spriteSheet[(int)Math.Floor(currentTexture)]))
				animationEnd = true;
			else
				animationEnd = false;
			SpriteHandler.sprites[sprite].Draw(spriteBatch, camera, position, scale, angle, origin, color, opacity, this, spriteEffect, depth);
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
		public void Draw(SpriteBatch spriteBatch, Camera camera, Vector2 position, Vector2 vector2Scale, float angle, Vector2 origin, Color color, float opacity, SpriteEffects spriteEffect, float depth)
		{
			if (currentTexture == SpriteHandler.sprites[sprite].textures.Count - 1 && ((int)Math.Floor(currentFrame) == SpriteHandler.sprites[sprite].frames[(int)Math.Floor(currentTexture)] || !SpriteHandler.sprites[sprite].spriteSheet[(int)Math.Floor(currentTexture)]))
				animationEnd = true;
			else
				animationEnd = false;
			SpriteHandler.sprites[sprite].Draw(spriteBatch, camera, position, vector2Scale, angle, origin, color, opacity, this, spriteEffect, depth);
		}
	}
}
