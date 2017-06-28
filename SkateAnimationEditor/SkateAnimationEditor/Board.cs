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
	public class Board
	{
		string animationKey = "";
		Matrix matrix = new Matrix();
		List<KeyFrame> keyFrames = new List<KeyFrame>();
		float rotationX, rotationY, rotationZ = 0;
		Vector2 position = Vector2.Zero;
		int width, height;
		string deck, tape;
		public bool deckVisible, animationFreeze, simpleBoard = false;

		public Board(string deck, string tape)
		{
			this.deck = deck;
			this.tape = tape;
		}

		struct KeyFrame
		{
			public int frame;
			public float rotationX, rotationY, rotationZ;
			public Vector2 position;
			public bool deckVisible, animationFreeze, simpleBoard;
		}

		public void SetAnimation(string key)
		{
			switch (key)
			{
				case ("Idle"):
					rotationX = 0;
					rotationY = 0;
					rotationZ = 0;
					position = Vector2.Zero;
					deckVisible = false;
					animationFreeze = false;
					simpleBoard = true;
					break;
				case ("JumpCharge"):
					rotationX = 0;
					rotationY = 0;
					rotationZ = 0;
					position = Vector2.Zero;
					deckVisible = false;
					animationFreeze = false;
					simpleBoard = true;
					break;
				case ("Jump"):
					rotationX = 0;
					rotationY = 0;
					rotationZ = 0;
					position = Vector2.Zero;
					deckVisible = false;
					animationFreeze = false;
					simpleBoard = true;
					break;
				default:
					this.animationKey = "Default";
					break;
			}
		}

		public void Update(int frame)
		{
			if (keyFrames.Count > 1)
			{
				for (int i = 0; i < keyFrames.Count; i++)
				{
					if (keyFrames[i].frame < frame)
					{
						int frameDifference = (keyFrames[i].frame - keyFrames[i - 1].frame);
						rotationX += (keyFrames[i].rotationX - keyFrames[i - 1].rotationX) / frameDifference;
						rotationY += (keyFrames[i].rotationY - keyFrames[i - 1].rotationY) / frameDifference;
						rotationZ += (keyFrames[i].rotationZ - keyFrames[i - 1].rotationX) / frameDifference;
						position += (keyFrames[i].position - keyFrames[i - 1].position) / frameDifference;
						break;
					}
					else if (frame == keyFrames[i].frame)
					{
						rotationX = keyFrames[i].rotationX;
						rotationY = keyFrames[i].rotationY;
						rotationZ = keyFrames[i].rotationZ;
						position = keyFrames[i].position;
						deckVisible = keyFrames[i].deckVisible;
						break;
					}
				}
			}
		}

		public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Camera camera, Vector2 playerPosition)
		{
			spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.get_transformation(graphicsDevice) * get_transformation(graphicsDevice));
			SpriteHandler.Draw(deck, spriteBatch, camera, position + playerPosition, SpriteEffects.None, 0f);
			spriteBatch.End();
		}

		/// <summary>
		/// Loads all the decks and tapes, etc
		/// </summary>
		/// <param name="Content"></param>
		public void LoadContent(ContentManager Content)
		{
			SpriteHandler.AddSprite("Tape_Default", new Sprite(Content.Load<Texture2D>("Tape_Default")));
			SpriteHandler.AddSprite("Deck_Default", new Sprite(Content.Load<Texture2D>("Deck_Default")));
		}

		public Matrix get_transformation(GraphicsDevice graphicsDevice)
		{
			matrix =       // Thanks to o KB o for this solution
				Matrix.CreateTranslation(new Vector3(-position.X, -position.Y, 0)) *
				Matrix.CreateRotationX(rotationX) *
				Matrix.CreateRotationY(rotationY) *
				Matrix.CreateRotationZ(rotationZ) *
				Matrix.CreateScale(new Vector3(1, 1, 1)) *
				Matrix.CreateTranslation(new Vector3(Game1.screenWidth * 0.5f, Game1.screenHeight * 0.5f, 0));
			return matrix;
		}
	}
}
