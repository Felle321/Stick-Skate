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
		Matrix matrix = new Matrix();
		List<KeyFrame> keyFrames = new List<KeyFrame>();
		public float rotationX, rotationY, rotationZ = 0;
		public Vector2 position = Vector2.Zero;
		int width, height;
		string deck, tape;
		public bool deckVisible, animationFreeze, drawSimpleBoard = false;
		public string simpleBoard = "";
		public Vector2 simpleBoardOrigin = new Vector2(23, 12);
		int totalFrames = 0;
		int framePrev = -1;
		float speed, internalFrame = 0;

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
			public bool deckVisible, animationFreeze, drawSimpleBoard;
		}

		KeyFrame NewKeyFrame(int frame)
		{
			KeyFrame keyFrame = new KeyFrame();

			keyFrame.rotationX = 0;
			keyFrame.rotationY = 0;
			keyFrame.rotationZ = 0;
			keyFrame.position = new Vector2(18, 48);
			keyFrame.deckVisible = false;
			keyFrame.animationFreeze = false;
			keyFrame.drawSimpleBoard = true;
			keyFrame.frame = frame;
			return keyFrame;
		}

		public void SetAnimation(string key, int totalFrames, float speed)
		{
			keyFrames.Clear();
			KeyFrame temp;
			this.speed = speed;

			this.totalFrames = totalFrames;
			framePrev = -1;

			switch (key)
			{
				case ("Idle"):
					rotationX = 0;
					rotationY = 0;
					rotationZ = 0;
					position = new Vector2(18, 48);
					deckVisible = false;
					animationFreeze = false;
					drawSimpleBoard = true;
					break;
				case ("JumpCharge"):
					rotationX = 0;
					rotationY = 0;
					rotationZ = 0;
					position = new Vector2(18, 48); ;
					deckVisible = false;
					animationFreeze = false;
					drawSimpleBoard = true;
					break;
				case ("Jump"):
					temp = NewKeyFrame(0);
					temp.rotationZ = -.135f;
					temp.position = new Vector2(17, 48);
					keyFrames.Add(temp);

					temp = NewKeyFrame(1);
					temp.rotationZ = -.095f;
					temp.position = new Vector2(16, 47);
					keyFrames.Add(temp);

					temp = NewKeyFrame(2);
					temp.rotationZ = -.2f;
					temp.position = new Vector2(16, 46);
					keyFrames.Add(temp);

					temp = NewKeyFrame(3);
					temp.rotationZ = -.13f;
					temp.position = new Vector2(19, 46);
					keyFrames.Add(temp);

					temp = NewKeyFrame(5);
					temp.rotationZ = .07f;
					temp.position = new Vector2(26, 45);
					keyFrames.Add(temp);

					temp = NewKeyFrame(8);
					temp.rotationZ = 0;
					temp.position = new Vector2(24, 44);
					keyFrames.Add(temp);
					break;
				case ("KickFlip"):
					temp = NewKeyFrame(8);
					temp.drawSimpleBoard = false;
					temp.deckVisible = true;
					keyFrames.Add(temp);
					break;
				default:
					rotationX = 0;
					rotationY = 0;
					rotationZ = 0;
					position = new Vector2(18, 48);
					deckVisible = false;
					animationFreeze = false;
					drawSimpleBoard = true;
					break;
			}

			if (totalFrames < keyFrames[keyFrames.Count - 1].frame)
				totalFrames = keyFrames[keyFrames.Count - 1].frame;

			Update(0);
		}

		public void Update(int animationFrame)
		{
			if (animationFrame == 0)
				internalFrame = 0;
			
			if (animationFreeze)
				internalFrame += speed;

			int frame = animationFrame + (int)Math.Floor(internalFrame);

			if (frame != framePrev)
			{
				framePrev = frame;

				if (keyFrames.Count > 1)
				{
					for (int i = keyFrames.Count - 1; i > 0; i--)
					{
						if (keyFrames[i].frame < frame)
						{
							if (keyFrames.Count - 1 == i)
							{
								int frameDifference = (totalFrames - keyFrames[i].frame);
								rotationX += (keyFrames[0].rotationX - keyFrames[i].rotationX) / (float)frameDifference;
								rotationY += (keyFrames[0].rotationY - keyFrames[i].rotationY) / (float)frameDifference;
								rotationZ += (keyFrames[0].rotationZ - keyFrames[i].rotationZ) / (float)frameDifference;
								position += (keyFrames[0].position - keyFrames[i].position) / (float)frameDifference;
								break;
							}
							else
							{
								int frameDifference = (keyFrames[i + 1].frame - keyFrames[i].frame);
								rotationX += (keyFrames[i + 1].rotationX - keyFrames[i].rotationX) / frameDifference;
								rotationY += (keyFrames[i + 1].rotationY - keyFrames[i].rotationY) / frameDifference;
								rotationZ += (keyFrames[i + 1].rotationZ - keyFrames[i].rotationZ) / frameDifference;
								position += (keyFrames[i + 1].position - keyFrames[i].position) / frameDifference;
								break;
							}
						}
						else if (frame == keyFrames[i].frame)
						{
							rotationX = keyFrames[i].rotationX;
							rotationY = keyFrames[i].rotationY;
							rotationZ = keyFrames[i].rotationZ;
							position = keyFrames[i].position;
							deckVisible = keyFrames[i].deckVisible;
							drawSimpleBoard = keyFrames[i].drawSimpleBoard;
							break;
						}
					}
				}
				else if (keyFrames.Count == 1)
				{
					rotationX = keyFrames[0].rotationX;
					rotationY = keyFrames[0].rotationY;
					rotationZ = keyFrames[0].rotationZ;
					position = keyFrames[0].position;
					deckVisible = keyFrames[0].deckVisible;
					drawSimpleBoard = keyFrames[0].drawSimpleBoard;
				}
			}
		}

		public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Camera camera, Vector2 playerPosition, Color wheelColor)
		{
			if (drawSimpleBoard)
			{
				spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.get_transformation(graphicsDevice));
				SpriteHandler.Draw(simpleBoard, spriteBatch, camera, position, 1, rotationZ, simpleBoardOrigin, wheelColor, 1f, SpriteEffects.None, 0f);
				spriteBatch.End();
			}
			else
			{
				spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.get_transformation(graphicsDevice));
				SpriteHandler.Draw(deck, spriteBatch, camera, position + playerPosition, new Vector2((float)Math.Cos(rotationX), (float)Math.Sin(rotationY)), rotationZ, new Vector2(width / 2, height / 2), Color.White, 1f, SpriteEffects.None, 0f);
				spriteBatch.End();
			}
		}

		public void DrawHUD(SpriteBatch spriteBatch)
		{

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
				Matrix.CreateTranslation(new Vector3(width * 0.5f, height * 0.5f, 0));
			return matrix;
		}
	}
}
