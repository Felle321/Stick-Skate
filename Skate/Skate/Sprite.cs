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
	public class Sprite
	{
		public bool animationEnd = false;
		public bool HUD = false;
		public float speed = 1;
		public float scale = 1;
		public float angle = 0;
		public Vector2 origin = Vector2.Zero;
		public Color color = Color.White;
		public int width, height;
		public bool animated = false;
		public bool draw = true;
		public bool drawOnce = false;
		public int framesTotal;
		float currentTexture = 0;
		public float currentFrame = 0;
		public float opacity = 1f;

		public List<Texture2D> textures = new List<Texture2D>();
		public List<bool> spriteSheet = new List<bool>();
		public List<int> frames = new List<int>();
		public List<Sprite> children = new List<Sprite>();
		public List<int> odds = new List<int>();

		/// <summary>
		/// This constructor is not compatible with spritesheets!
		/// </summary>
		/// <param name="texture"></param>
		public Sprite(Texture2D texture)
		{
			textures.Add(texture);
			speed = 0;
			width = textures[0].Width;
			height = textures[0].Height;
			frames.Add(1);
			spriteSheet.Add(false);

			Initialize();
		}

		/// <summary>
		/// Initializes a new Sprite
		/// </summary>
		/// <param name="textures">A spritesheet</param>
		/// <param name="frames">The amount of frames the spritesheet contains</param>
		public Sprite(Texture2D texture, int frames, int width, int height)
		{
			speed = 0;
			this.width = width;
			this.height = height;

			if (frames > 1)
				spriteSheet.Add(true);
			else
			{
				spriteSheet.Add(false);
				if (frames < 1)
					frames = 1;
			}

			this.frames.Add(frames);

			this.textures.Add(texture);

			Initialize();
		}

		/// <summary>
		/// Initializes a new Sprite
		/// </summary>
		/// <param name="textures">A list of textures or spritesheets to be played in a sequence</param>
		/// <param name="framesPerTexture">The amount of frames per each spritesheet/texture</param>
		public Sprite(List<Texture2D> textures, List<int> framesPerTexture, int width, int height)
		{
			speed = 0;
			this.width = width;
			this.height = height;

			for (int i = 0; i < framesPerTexture.Count; i++)
			{
				if (framesPerTexture[i] > 1)
					spriteSheet.Add(true);
				else
				{
					spriteSheet.Add(false);
					if (framesPerTexture[i] < 1)
						framesPerTexture[i] = 1;
				}

				frames.Add(framesPerTexture[i]);
			}

			this.textures.AddRange(textures);

			Initialize();
		}

		public void Initialize()
		{
			for (int i = 0; i < frames.Count; i++)
			{
				framesTotal += frames[i];
				if (frames[i] > 1)
					animated = true;
			}

			if (frames.Count > 1)
				animated = true;
		}

		#region WithoutAnimation
		/// <summary>
		/// Draws the sprite
		/// </summary>
		/// <param name="rand">An instance of the class Random</param>
		/// <param name="spriteBatch">The currently used SpriteBatch</param>
		/// <param name="camera">The active camera</param>
		/// <param name="position">The position of the Sprite</param>
		public void Draw(SpriteBatch spriteBatch, Camera camera, Vector2 position, SpriteEffects spriteEffect, float depth)
		{
			if (currentTexture == textures.Count - 1 && ((int)Math.Floor(currentFrame) == frames[(int)Math.Floor(currentTexture)] || !spriteSheet[(int)Math.Floor(currentTexture)]))
				animationEnd = true;
			else
				animationEnd = false;

			if (draw && (camera.rectangle.Intersects(new Rectangle(Game1.CeilAdv(position.X), Game1.CeilAdv(position.Y), width, height)) || HUD))
			{
				if (!animated)
				{
					spriteBatch.Draw(textures[0], position, null, GetColorOpaque(color, opacity), angle, origin, scale, spriteEffect, depth);
				}
				else
				{
					if (spriteSheet[(int)Math.Floor(currentTexture)])
					{
						int x = (int)(currentFrame % Math.Floor((float)(textures[(int)Math.Floor(currentTexture)].Width / width))) * width;
						int y = (int)Math.Floor(currentFrame / (textures[(int)Math.Floor(currentTexture)].Width / width)) * height;
						spriteBatch.Draw(textures[(int)Math.Floor(currentTexture)], position, new Rectangle(x, y, width, height), GetColorOpaque(color, opacity), angle, origin, scale, spriteEffect, depth);
					}
					else
					{
						spriteBatch.Draw(textures[(int)Math.Floor(currentTexture)], position, null, GetColorOpaque(color, opacity), angle, origin, scale, spriteEffect, depth);
					}
				}
			}

			if (animated)
			{
				if (spriteSheet[(int)Math.Floor(currentTexture)])
				{
					if (currentFrame >= frames[(int)Math.Floor(currentTexture)])
					{
						currentFrame = 0;
						currentTexture = (currentTexture + 1) % (textures.Count);
					}
					else
						currentFrame = (currentFrame + speed);
				}
				else
					currentTexture = (currentTexture + speed) % (textures.Count);
			}

			if (drawOnce && animationEnd)
				draw = false;
		}

		/// <summary>
		/// Draws the sprite
		/// </summary>
		/// <param name="rand">An instance of the class Random</param>
		/// <param name="spriteBatch">The currently used SpriteBatch</param>
		/// <param name="camera">The active camera</param>
		/// <param name="rectangle">The position and borders of the Sprite</param>
		public void Draw(SpriteBatch spriteBatch, Camera camera, Rectangle rectangle, SpriteEffects spriteEffect, float depth)
		{
			if (currentTexture == textures.Count - 1 && ((int)Math.Floor(currentFrame) == frames[(int)Math.Floor(currentTexture)] || !spriteSheet[(int)Math.Floor(currentTexture)]))
				animationEnd = true;
			else
				animationEnd = false;

			if (draw && (camera.rectangle.Intersects(rectangle) || HUD))
			{
				if (!animated)
				{
					spriteBatch.Draw(textures[0], rectangle, null, GetColorOpaque(color, opacity), angle, origin, spriteEffect, depth);
				}
				else
				{
					if (spriteSheet[(int)Math.Floor(currentTexture)])
					{
						int x = (int)(currentFrame % Math.Floor((float)(textures[(int)Math.Floor(currentTexture)].Width / width))) * width;
						int y = (int)Math.Floor(currentFrame / (textures[(int)Math.Floor(currentTexture)].Width / width)) * height;
						spriteBatch.Draw(textures[(int)Math.Floor(currentTexture)], rectangle, new Rectangle(x, y, width, height), GetColorOpaque(color, opacity), angle, origin, spriteEffect, depth);
					}
					else
					{
						spriteBatch.Draw(textures[(int)Math.Floor(currentTexture)], rectangle, null, GetColorOpaque(color, opacity), angle, origin, spriteEffect, depth);
					}
				}
			}

			if (animated)
			{
				if (spriteSheet[(int)Math.Floor(currentTexture)])
				{
					if (currentFrame >= frames[(int)Math.Floor(currentTexture)])
					{
						currentFrame = 0;
						currentTexture = (currentTexture + 1) % (textures.Count);
					}
					else
						currentFrame = (currentFrame + speed);
				}
				else
					currentTexture = (currentTexture + speed) % (textures.Count);
			}

			if (drawOnce && animationEnd)
				draw = false;
		}


		/// <summary>
		/// Draws the sprite
		/// </summary>
		/// <param name="rand">An instance of the class Random</param>
		/// <param name="spriteBatch">The currently used SpriteBatch</param>
		/// <param name="camera">The active camera</param>
		/// <param name="position">The position of the Sprite</param>
		public void Draw(SpriteBatch spriteBatch, Camera camera, Rectangle rectangle, Rectangle sourceRectangle, SpriteEffects spriteEffect, float depth)
		{
			if (currentTexture == textures.Count - 1 && ((int)Math.Floor(currentFrame) == frames[(int)Math.Floor(currentTexture)] || !spriteSheet[(int)Math.Floor(currentTexture)]))
				animationEnd = true;
			else
				animationEnd = false;

			if (draw && (camera.rectangle.Intersects(new Rectangle(Game1.CeilAdv(rectangle.X), Game1.CeilAdv(rectangle.Y), width, height)) || HUD))
			{
				if (!animated)
				{
					spriteBatch.Draw(textures[0], rectangle.Location.ToVector2(), sourceRectangle, GetColorOpaque(color, opacity), angle, origin, scale, spriteEffect, depth);
				}
				else
				{
					if (spriteSheet[(int)Math.Floor(currentTexture)])
					{
						int x = (int)(currentFrame % Math.Floor((float)(textures[(int)Math.Floor(currentTexture)].Width / width))) * width;
						int y = (int)Math.Floor(currentFrame / (textures[(int)Math.Floor(currentTexture)].Width / width)) * height;
						spriteBatch.Draw(textures[(int)Math.Floor(currentTexture)], rectangle.Location.ToVector2(), new Rectangle(x + sourceRectangle.X, y + sourceRectangle.Y, sourceRectangle.Width, sourceRectangle.Height), GetColorOpaque(color, opacity), angle, origin, scale, spriteEffect, depth);
					}
					else
					{
						spriteBatch.Draw(textures[(int)Math.Floor(currentTexture)], rectangle.Location.ToVector2(), null, GetColorOpaque(color, opacity), angle, origin, scale, spriteEffect, depth);
					}
				}
			}

			if (animated)
			{
				if (spriteSheet[(int)Math.Floor(currentTexture)])
				{
					if (currentFrame >= frames[(int)Math.Floor(currentTexture)])
					{
						currentFrame = 0;
						currentTexture = (currentTexture + 1) % (textures.Count);
					}
					else
						currentFrame = (currentFrame + speed);
				}
				else
					currentTexture = (currentTexture + speed) % (textures.Count);
			}

			if (drawOnce && animationEnd)
				draw = false;
		}
		/// <summary>
		/// Draws the sprite
		/// </summary>
		/// <param name="rand">An instance of the class Random</param>
		/// <param name="spriteBatch">The currently used SpriteBatch</param>
		/// <param name="camera">The active camera</param>
		/// <param name="position">The position of the Pprite</param>
		/// <param name="vector2Scale">The Vector2 value to rescale the sprite with</param>
		public void Draw(SpriteBatch spriteBatch, Camera camera, Vector2 position, Vector2 vector2Scale, SpriteEffects spriteEffect, float depth)
		{
			if (currentTexture == textures.Count - 1 && ((int)Math.Floor(currentFrame) == frames[(int)Math.Floor(currentTexture)] || !spriteSheet[(int)Math.Floor(currentTexture)]))
				animationEnd = true;
			else
				animationEnd = false;

			if (draw && (camera.rectangle.Intersects(new Rectangle(Game1.CeilAdv(position.X), Game1.CeilAdv(position.Y), width, height)) || HUD))
			{
				if (!animated)
				{
					spriteBatch.Draw(textures[0], position, null, GetColorOpaque(color, opacity), angle, Vector2.Zero, scale, spriteEffect, depth);
				}
				else
				{
					if (spriteSheet[(int)Math.Floor(currentTexture)])
					{
						int x = (int)(currentFrame % Math.Floor((float)(textures[(int)Math.Floor(currentTexture)].Width / width))) * width;
						int y = (int)Math.Floor(currentFrame / (textures[(int)Math.Floor(currentTexture)].Width / width)) * height;
						spriteBatch.Draw(textures[(int)Math.Floor(currentTexture)], position, new Rectangle(x, y, width, height), GetColorOpaque(color, opacity), angle, origin, vector2Scale, spriteEffect, depth);
					}
					else
					{
						spriteBatch.Draw(textures[(int)Math.Floor(currentTexture)], position, null, GetColorOpaque(color, opacity), angle, origin, vector2Scale, spriteEffect, depth);
					}
				}
			}

			if (animated)
			{
				if (spriteSheet[(int)Math.Floor(currentTexture)])
				{
					if (currentFrame >= frames[(int)Math.Floor(currentTexture)])
					{
						currentFrame = 0;
						currentTexture = (currentTexture + 1) % (textures.Count);
					}
					else
						currentFrame = (currentFrame + speed);
				}
				else
					currentTexture = (currentTexture + speed) % (textures.Count);
			}

			if (drawOnce && animationEnd)
				draw = false;
		}

		/// <summary>
		/// Draws the sprite
		/// </summary>
		/// <param name="rand">An instance of the class Random</param>
		/// <param name="spriteBatch">The currently used SpriteBatch</param>
		/// <param name="camera">The active camera</param>
		/// <param name="position">The position of the Sprite</param>
		/// <param name="scale">The scale the texture will be drawn with (this doesn't exclude the base-scale)</param>
		/// <param name="angle">The angle with which the texture is to be drawn</param>
		/// <param name="origin">The origin of the rotation</param>
		/// <param name="color">The color of the sprite</param>
		public void Draw(SpriteBatch spriteBatch, Camera camera, Vector2 position, float scale, float angle, Vector2 origin, Color color, float opacity, SpriteEffects spriteEffect, float depth)
		{
			if (currentTexture == textures.Count - 1 && ((int)Math.Floor(currentFrame) == frames[(int)Math.Floor(currentTexture)] || !spriteSheet[(int)Math.Floor(currentTexture)]))
				animationEnd = true;
			else
				animationEnd = false;

			if (draw && (camera.rectangle.Intersects(new Rectangle(Game1.CeilAdv(position.X), Game1.CeilAdv(position.Y), width, height)) || HUD))
			{
				if (!animated)
				{
					spriteBatch.Draw(textures[0], position, null, GetColorOpaque(color, opacity), angle, origin, scale * this.scale, spriteEffect, depth);
				}
				else
				{
					if (spriteSheet[(int)Math.Floor(currentTexture)])
					{
						int x = (int)(currentFrame % Math.Floor((float)(textures[(int)Math.Floor(currentTexture)].Width / width))) * width;
						int y = (int)Math.Floor(currentFrame / (textures[(int)Math.Floor(currentTexture)].Width / width)) * height;
						bool temp = animationEnd;
						spriteBatch.Draw(textures[(int)Math.Floor(currentTexture)], position, new Rectangle(x, y, width, height), GetColorOpaque(color, opacity), angle, origin, scale * this.scale, spriteEffect, depth);
					}
					else
					{
						spriteBatch.Draw(textures[(int)Math.Floor(currentTexture)], position, null, GetColorOpaque(color, opacity), angle, origin, scale * this.scale, spriteEffect, depth);
					}
				}
			}

			if (animated)
			{
				if (spriteSheet[(int)Math.Floor(currentTexture)])
				{
					if (currentFrame >= frames[(int)Math.Floor(currentTexture)])
					{
						currentFrame = 0;
						currentTexture = (currentTexture + 1) % (textures.Count);
					}
					else
						currentFrame = (currentFrame + speed);
				}
				else
					currentTexture = (currentTexture + speed) % (textures.Count);
			}

			if (drawOnce && animationEnd)
				draw = false;
		}

		/// <summary>
		/// Draws the sprite
		/// </summary>
		/// <param name="rand">An instance of the class Random</param>
		/// <param name="spriteBatch">The currently used SpriteBatch</param>
		/// <param name="camera">The active camera</param>
		/// <param name="rectangle">The rectangle the Sprite will be drawn to</param>
		/// <param name="angle">The angle with which the texture is to be drawn</param>
		/// <param name="origin">The origin of the rotation</param>
		/// <param name="color">The color of the sprite</param>
		public void Draw(SpriteBatch spriteBatch, Camera camera, Rectangle rectangle, float angle, Vector2 origin, Color color, float opacity, SpriteEffects spriteEffect, float depth)
		{
			if (currentTexture == textures.Count - 1 && ((int)Math.Floor(currentFrame) == frames[(int)Math.Floor(currentTexture)] || !spriteSheet[(int)Math.Floor(currentTexture)]))
				animationEnd = true;
			else
				animationEnd = false;

			if (draw && (camera.rectangle.Intersects(rectangle) || HUD))
			{
				if (!animated)
				{
					spriteBatch.Draw(textures[0], rectangle, null, GetColorOpaque(color, opacity), angle, origin, spriteEffect, depth);
				}
				else
				{
					if (spriteSheet[(int)Math.Floor(currentTexture)])
					{
						int x = (int)(currentFrame % Math.Floor((float)(textures[(int)Math.Floor(currentTexture)].Width / width))) * width;
						int y = (int)Math.Floor(currentFrame / (textures[(int)Math.Floor(currentTexture)].Width / width)) * height;
						bool temp = animationEnd;
						spriteBatch.Draw(textures[(int)Math.Floor(currentTexture)], rectangle, new Rectangle(x, y, width, height), GetColorOpaque(color, opacity), angle, origin, spriteEffect, depth);
					}
					else
					{
						spriteBatch.Draw(textures[(int)Math.Floor(currentTexture)], rectangle, null, GetColorOpaque(color, opacity), angle, origin, spriteEffect, depth);
					}
				}
			}

			if (animated)
			{
				if (spriteSheet[(int)Math.Floor(currentTexture)])
				{
					if (currentFrame >= frames[(int)Math.Floor(currentTexture)])
					{
						currentFrame = 0;
						currentTexture = (currentTexture + 1) % (textures.Count);
					}
					else
						currentFrame = (currentFrame + speed);
				}
				else
					currentTexture = (currentTexture + speed) % (textures.Count);
			}

			if (drawOnce && animationEnd)
				draw = false;
		}

		/// <summary>
		/// Draws the sprite
		/// </summary>
		/// <param name="rand">An instance of the class Random</param>
		/// <param name="spriteBatch">The currently used SpriteBatch</param>
		/// <param name="camera">The active camera</param>
		/// <param name="position">The position of the Sprite</param>
		/// <param name="scale">The scale the texture will be drawn with (this doesn't exclude the base-scale)</param>
		/// <param name="angle">The angle with which the texture is to be drawn</param>
		/// <param name="origin">The origin of the rotation</param>
		/// <param name="color">The color of the sprite</param>
		public void Draw(SpriteBatch spriteBatch, Camera camera, Vector2 position, Vector2 scale, float angle, Vector2 origin, Color color, float opacity, SpriteEffects spriteEffect, float depth)
		{
			if (currentTexture == textures.Count - 1 && ((int)Math.Floor(currentFrame) == frames[(int)Math.Floor(currentTexture)] || !spriteSheet[(int)Math.Floor(currentTexture)]))
				animationEnd = true;
			else
				animationEnd = false;

			if (draw && (camera.rectangle.Intersects(new Rectangle(Game1.CeilAdv(position.X), Game1.CeilAdv(position.Y), width, height)) || HUD))
			{
				if (!animated)
				{
					spriteBatch.Draw(textures[0], position, null, GetColorOpaque(color, opacity), angle, origin, scale * this.scale, spriteEffect, depth);
				}
				else
				{
					if (spriteSheet[(int)Math.Floor(currentTexture)])
					{
						int x = (int)(currentFrame % Math.Floor((float)(textures[(int)Math.Floor(currentTexture)].Width / width))) * width;
						int y = (int)Math.Floor(currentFrame / (textures[(int)Math.Floor(currentTexture)].Width / width)) * height;
						spriteBatch.Draw(textures[(int)Math.Floor(currentTexture)], position, new Rectangle(x, y, width, height), GetColorOpaque(color, opacity), angle, origin, scale * this.scale, spriteEffect, depth);
					}
					else
					{
						spriteBatch.Draw(textures[(int)Math.Floor(currentTexture)], position, null, GetColorOpaque(color, opacity), angle, origin, scale * this.scale, spriteEffect, depth);
					}
				}
			}

			if (animated)
			{
				if (spriteSheet[(int)Math.Floor(currentTexture)])
				{
					if (currentFrame >= frames[(int)Math.Floor(currentTexture)])
					{
						currentFrame = 0;
						currentTexture = (currentTexture + 1) % (textures.Count);
					}
					else
						currentFrame = (currentFrame + speed);
				}
				else
					currentTexture = (currentTexture + speed) % (textures.Count);
			}

			if (drawOnce && animationEnd)
				draw = false;
		}

		#endregion


		/// <summary>
		/// Draws the sprite
		/// </summary>
		/// <param name="rand">An instance of the class Random</param>
		/// <param name="spriteBatch">The currently used SpriteBatch</param>
		/// <param name="camera">The active camera</param>
		/// <param name="position">The position of the Sprite</param>
		public void Draw(SpriteBatch spriteBatch, Camera camera, Vector2 position, Animation animation, SpriteEffects spriteEffect, float depth)
		{
			if (currentTexture == textures.Count - 1 && ((int)Math.Floor(currentFrame) == frames[(int)Math.Floor(currentTexture)] || !spriteSheet[(int)Math.Floor(currentTexture)]))
				animationEnd = true;
			else
				animationEnd = false;

			if (draw && (camera.rectangle.Intersects(new Rectangle(Game1.CeilAdv(position.X), Game1.CeilAdv(position.Y), width, height)) || HUD))
			{
				if (!animated)
				{
					spriteBatch.Draw(textures[0], position, null, GetColorOpaque(color, opacity), angle, origin, scale, spriteEffect, depth);
				}
				else
				{
					if (spriteSheet[(int)Math.Floor(animation.currentTexture)])
					{
						int x = (int)(animation.currentFrame % Math.Floor((float)(textures[(int)Math.Floor(animation.currentTexture)].Width / width))) * width;
						int y = (int)Math.Floor(animation.currentFrame / (textures[(int)Math.Floor(animation.currentTexture)].Width / width)) * height;
						spriteBatch.Draw(textures[(int)Math.Floor(animation.currentTexture)], position, new Rectangle(x, y, width, height), GetColorOpaque(color, opacity), angle, origin, scale, spriteEffect, depth);
					}
					else
					{
						spriteBatch.Draw(textures[(int)Math.Floor(animation.currentTexture)], position, null, GetColorOpaque(color, opacity), angle, origin, scale, spriteEffect, depth);
					}
				}
			}


			if (animated)
			{
				if (spriteSheet[(int)Math.Floor(animation.currentTexture)])
				{
					if (animation.currentFrame >= frames[(int)Math.Floor(animation.currentTexture)])
					{
						animation.currentFrame = 0;
						animation.currentTexture = (animation.currentTexture + 1) % (textures.Count);
					}
					else
						animation.currentFrame = (animation.currentFrame + animation.speed);
				}
				else
					animation.currentTexture = (animation.currentTexture + animation.speed) % (textures.Count);
			}

			if (drawOnce && animationEnd)
				draw = false;
		}


		/// <summary>
		/// Draws the sprite
		/// </summary>
		/// <param name="rand">An instance of the class Random</param>
		/// <param name="spriteBatch">The currently used SpriteBatch</param>
		/// <param name="camera">The active camera</param>
		/// <param name="position">The position of the Pprite</param>
		/// <param name="vector2Scale">The Vector2 value to rescale the sprite with</param>
		public void Draw(SpriteBatch spriteBatch, Camera camera, Vector2 position, Vector2 vector2Scale, Animation animation, SpriteEffects spriteEffect, float depth)
		{
			if (currentTexture == textures.Count - 1 && ((int)Math.Floor(currentFrame) == frames[(int)Math.Floor(currentTexture)] || !spriteSheet[(int)Math.Floor(currentTexture)]))
				animationEnd = true;
			else
				animationEnd = false;

			if (draw && (camera.rectangle.Intersects(new Rectangle(Game1.CeilAdv(position.X), Game1.CeilAdv(position.Y), width, height)) || HUD))
			{
				if (!animated)
				{
					spriteBatch.Draw(textures[0], position, null, GetColorOpaque(color, opacity), angle, Vector2.Zero, scale, spriteEffect, depth);
				}
				else
				{
					if (spriteSheet[(int)Math.Floor(animation.currentTexture)])
					{
						int x = (int)(animation.currentFrame % Math.Floor((float)(textures[(int)Math.Floor(animation.currentTexture)].Width / width))) * width;
						int y = (int)Math.Floor(animation.currentFrame / (textures[(int)Math.Floor(animation.currentTexture)].Width / width)) * height;
						spriteBatch.Draw(textures[(int)Math.Floor(animation.currentTexture)], position, new Rectangle(x, y, width, height), GetColorOpaque(color, opacity), angle, origin, vector2Scale, spriteEffect, depth);
					}
					else
					{
						spriteBatch.Draw(textures[(int)Math.Floor(animation.currentTexture)], position, null, GetColorOpaque(color, opacity), angle, origin, vector2Scale, spriteEffect, depth);
					}
				}
			}

			if (animated)
			{
				if (spriteSheet[(int)Math.Floor(animation.currentTexture)])
				{
					if (animation.currentFrame >= frames[(int)Math.Floor(animation.currentTexture)])
					{
						animation.currentFrame = 0;
						animation.currentTexture = (animation.currentTexture + 1) % (textures.Count);
					}
					else
						animation.currentFrame = (animation.currentFrame + animation.speed);
				}
				else
					animation.currentTexture = (animation.currentTexture + animation.speed) % (textures.Count);
			}

			if (drawOnce && animationEnd)
				draw = false;
		}

		/// <summary>
		/// Draws the sprite
		/// </summary>
		/// <param name="rand">An instance of the class Random</param>
		/// <param name="spriteBatch">The currently used SpriteBatch</param>
		/// <param name="camera">The active camera</param>
		/// <param name="position">The position of the Sprite</param>
		/// <param name="scale">The scale the texture will be drawn with (this doesn't exclude the base-scale)</param>
		/// <param name="angle">The angle with which the texture is to be drawn</param>
		/// <param name="origin">The origin of the rotation</param>
		/// <param name="color">The color of the sprite</param>
		public void Draw(SpriteBatch spriteBatch, Camera camera, Vector2 position, float scale, float angle, Vector2 origin, Color color, float opacity, Animation animation, SpriteEffects spriteEffect, float depth)
		{
			if (currentTexture == textures.Count - 1 && ((int)Math.Floor(currentFrame) == frames[(int)Math.Floor(currentTexture)] || !spriteSheet[(int)Math.Floor(currentTexture)]))
				animationEnd = true;
			else
				animationEnd = false;

			if (draw && (camera.rectangle.Intersects(new Rectangle(Game1.CeilAdv(position.X), Game1.CeilAdv(position.Y), width, height)) || HUD))
			{
				if (!animated)
				{
					spriteBatch.Draw(textures[0], position, null, GetColorOpaque(color, opacity), angle, origin, scale * this.scale, spriteEffect, depth);
				}
				else
				{
					if (spriteSheet[(int)Math.Floor(animation.currentTexture)])
					{
						int x = (int)(animation.currentFrame % Math.Floor((float)(textures[(int)Math.Floor(animation.currentTexture)].Width / width))) * width;
						int y = (int)Math.Floor(animation.currentFrame / (textures[(int)Math.Floor(animation.currentTexture)].Width / width)) * height;
						spriteBatch.Draw(textures[(int)Math.Floor(animation.currentTexture)], position, new Rectangle(x, y, width, height), GetColorOpaque(color, opacity), angle, origin, scale * this.scale, spriteEffect, depth);
					}
					else
					{
						spriteBatch.Draw(textures[(int)Math.Floor(animation.currentTexture)], position, null, GetColorOpaque(color, opacity), angle, origin, scale * this.scale, spriteEffect, depth);
					}
				}
			}

			if (animated)
			{
				if (spriteSheet[(int)Math.Floor(animation.currentTexture)])
				{
					if (animation.currentFrame >= frames[(int)Math.Floor(animation.currentTexture)])
					{
						animation.currentFrame = 0;
						animation.currentTexture = (animation.currentTexture + 1) % (textures.Count);
					}
					else
						animation.currentFrame = (animation.currentFrame + animation.speed);
				}
				else
					animation.currentTexture = (animation.currentTexture + animation.speed) % (textures.Count);
			}

			if (drawOnce && animationEnd)
				draw = false;
		}

		/// <summary>
		/// Draws the sprite
		/// </summary>
		/// <param name="rand">An instance of the class Random</param>
		/// <param name="spriteBatch">The currently used SpriteBatch</param>
		/// <param name="camera">The active camera</param>
		/// <param name="position">The position of the Sprite</param>
		/// <param name="scale">The scale the texture will be drawn with (this doesn't exclude the base-scale)</param>
		/// <param name="angle">The angle with which the texture is to be drawn</param>
		/// <param name="origin">The origin of the rotation</param>
		/// <param name="color">The color of the sprite</param>
		public void Draw(SpriteBatch spriteBatch, Camera camera, Vector2 position, Vector2 scale, float angle, Vector2 origin, Color color, float opacity, Animation animation, SpriteEffects spriteEffect, float depth)
		{
			if (currentTexture == textures.Count - 1 && ((int)Math.Floor(currentFrame) == frames[(int)Math.Floor(currentTexture)] || !spriteSheet[(int)Math.Floor(currentTexture)]))
				animationEnd = true;
			else
				animationEnd = false;

			if (draw && (camera.rectangle.Intersects(new Rectangle(Game1.CeilAdv(position.X), Game1.CeilAdv(position.Y), width, height)) || HUD))
			{
				if (!animated)
				{
					spriteBatch.Draw(textures[0], position, null, GetColorOpaque(color, opacity), angle, origin, scale * this.scale, spriteEffect, depth);
				}
				else
				{
					if (spriteSheet[(int)Math.Floor(animation.currentTexture)])
					{
						int x = (int)(animation.currentFrame % Math.Floor((float)(textures[(int)Math.Floor(animation.currentTexture)].Width / width))) * width;
						int y = (int)Math.Floor(animation.currentFrame / (textures[(int)Math.Floor(animation.currentTexture)].Width / width)) * height;
						spriteBatch.Draw(textures[(int)Math.Floor(animation.currentTexture)], position, new Rectangle(x, y, width, height), GetColorOpaque(color, opacity), angle, origin, scale * this.scale, spriteEffect, depth);
					}
					else
					{
						spriteBatch.Draw(textures[(int)Math.Floor(animation.currentTexture)], position, null, GetColorOpaque(color, opacity), angle, origin, scale * this.scale, spriteEffect, depth);
					}
				}
			}

			if (animated)
			{
				if (spriteSheet[(int)Math.Floor(animation.currentTexture)])
				{
					if (animation.currentFrame >= frames[(int)Math.Floor(animation.currentTexture)])
					{
						animation.currentFrame = 0;
						animation.currentTexture = (animation.currentTexture + 1) % (textures.Count);
					}
					else
						animation.currentFrame = (animation.currentFrame + animation.speed);
				}
				else
					animation.currentTexture = (animation.currentTexture + animation.speed) % (textures.Count);
			}

			if (drawOnce && animationEnd)
				draw = false;
		}

		public void DrawOnce()
		{
			draw = true;
			drawOnce = true;
		}

		static Color GetColorOpaque(Color color, float opacity)
		{
			return Color.Lerp(color, Color.Transparent, 1 - opacity);
		}

		Color GetColorOpaque()
		{
			return Color.Lerp(color, Color.Transparent, 1 - opacity);
		}
	}
}