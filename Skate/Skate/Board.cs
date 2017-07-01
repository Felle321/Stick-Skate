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
	public class Board
	{
		List<BoardKeyFrame> keyFrames = new List<BoardKeyFrame>();
		public float scaleX, scaleY, rotation = 0;
		public Vector2 position = Vector2.Zero;
		int width, height;
		string deck, tape;
		public bool animationFreeze, drawSimpleBoard = false;
		public string simpleBoard = "";
		public Vector2 simpleBoardOrigin = new Vector2(23, 12);
		public int totalFrames = 0;
		int framePrev = -1;
		public float speed, frame = 0;
		public bool keyFrameActive = false;
		public int currentFrame = 0;

		public Board(string deck, string tape)
		{
			this.deck = deck;
			this.tape = tape;
		}

		public void SetAnimation(string key)
		{
			keyFrames.Clear();
			framePrev = -1;

			width = SpriteHandler.sprites[deck].width;
			height = SpriteHandler.sprites[deck].height;

			switch (key)
			{
				case ("Idle"):
					InitializeAnimation(0, 0);
					break;
				case ("JumpCharge"):
					InitializeAnimation(0, 0);
					break;
				case ("Jump"):
					InitializeAnimation(11, .8f);
					keyFrames.Add(new BoardKeyFrame(0, 0, 0, -.135f, new Vector2(17, 48), false, true));
					keyFrames.Add(new BoardKeyFrame(1, 0, 0, -.095f, new Vector2(16, 47), false, true));
					keyFrames.Add(new BoardKeyFrame(2, 0, 0, -.2f, new Vector2(16, 46), false, true));
					keyFrames.Add(new BoardKeyFrame(3, 0, 0, -.13f, new Vector2(19, 46), false, true));
					keyFrames.Add(new BoardKeyFrame(5, 0, 0, .07f, new Vector2(26, 45), false, true));
					keyFrames.Add(new BoardKeyFrame(8, 0, 0, 0, new Vector2(24, 44), false, true));
					break;
				case ("KickFlip"):
					InitializeAnimation(20, .8f);
					keyFrames.Add(new BoardKeyFrame(0, 0, 0, 0, new Vector2(-5, 8), false, true));
					keyFrames.Add(new BoardKeyFrame(2, 0, 0, -.17f, new Vector2(-3, 3), false, true));
					keyFrames.Add(new BoardKeyFrame(3, 1, .2300007f, .03944445f, new Vector2(-.1111112f, .2777777f), false, false));
					keyFrames.Add(new BoardKeyFrame(6, 1, 1, .03248366f, new Vector2(.02614379f, -6.359477f), false, false));
					keyFrames.Add(new BoardKeyFrame(12, 1, -1, .01856209f, new Vector2(.8720822f, -6.205416f), false, false));
					keyFrames.Add(new BoardKeyFrame(15, 1, -.3550003f, .0916013f, new Vector2(-1.329948f, -5.878385f), false, false));
					keyFrames.Add(new BoardKeyFrame(16, 1, -.2840002f, .123281f, new Vector2(-1.063959f, 3.897292f), false, true));
					break;
				default:
					InitializeAnimation(0, 0);
					break;
			}

			if (keyFrames.Count > 0)
				if (totalFrames < keyFrames[keyFrames.Count - 1].frame)
					totalFrames = keyFrames[keyFrames.Count - 1].frame;

			Update(0);
		}

		public void InitializeAnimation(int totalFrames, float speed)
		{
			this.speed = speed;
			this.totalFrames = totalFrames;
			scaleX = 0;
			scaleY = 0;
			rotation = 0;
			position = new Vector2(18, 48);
			animationFreeze = false;
			drawSimpleBoard = true;
		}

		public string GetCommand()
		{
			string text = "InitializeAnimation(" + totalFrames + ", " + Game1.FloatToString(speed) + ");";

			for (int i = 0; i < keyFrames.Count; i++)
			{
				text += "\r\n" + keyFrames[i].GetCommand();
			}

			return text;
		}

		public void ApplyKeyFrame()
		{
			int insert = -1;

			for (int i = 0; i < keyFrames.Count; i++)
			{
				if (keyFrames[i].frame == currentFrame)
				{
					keyFrames[i] = new BoardKeyFrame(currentFrame, scaleX, scaleY, rotation, position, animationFreeze, drawSimpleBoard);
					return;
				}
				else if (keyFrames[i].frame < currentFrame)
					insert = i + 1;
			}

			if (insert < 0)
				insert = 0;

			keyFrames.Insert(insert, new BoardKeyFrame(currentFrame, scaleX, scaleY, rotation, position, animationFreeze, drawSimpleBoard));
		}

		public void Update(float animationFrame)
		{
			keyFrameActive = false;

			if (animationFrame == 0)
				frame = 0;

			for (int i = 0; i < keyFrames.Count; i++)
			{
				if (keyFrames[i].frame == currentFrame)
					keyFrameActive = true;
			}

			if (currentFrame != framePrev)
			{
				framePrev = currentFrame;

				if (keyFrames.Count > 1)
				{
					for (int i = keyFrames.Count - 1; i >= 0; i--)
					{
						if (keyFrames[i].frame < currentFrame)
						{
							if (keyFrames.Count - 1 == i)
							{
								int frameDifference = (totalFrames - keyFrames[i].frame);
								scaleX += (keyFrames[0].scaleX - keyFrames[i].scaleX) / (float)frameDifference;
								scaleY += (keyFrames[0].scaleY - keyFrames[i].scaleY) / (float)frameDifference;
								rotation += (keyFrames[0].rotation - keyFrames[i].rotation) / (float)frameDifference;
								position += (keyFrames[0].position - keyFrames[i].position) / (float)frameDifference;
								break;
							}
							else
							{
								int frameDifference = (keyFrames[i + 1].frame - keyFrames[i].frame);
								scaleX += (keyFrames[i + 1].scaleX - keyFrames[i].scaleX) / frameDifference;
								scaleY += (keyFrames[i + 1].scaleY - keyFrames[i].scaleY) / frameDifference;
								rotation += (keyFrames[i + 1].rotation - keyFrames[i].rotation) / frameDifference;
								position += (keyFrames[i + 1].position - keyFrames[i].position) / frameDifference;
								break;
							}
						}
						else if (currentFrame == keyFrames[i].frame)
						{
							scaleX = keyFrames[i].scaleX;
							scaleY = keyFrames[i].scaleY;
							rotation = keyFrames[i].rotation;
							position = keyFrames[i].position;
							drawSimpleBoard = keyFrames[i].drawSimpleBoard;
							break;
						}
					}
				}
				else if (keyFrames.Count == 1)
				{
					scaleX = keyFrames[0].scaleX;
					scaleY = keyFrames[0].scaleY;
					rotation = keyFrames[0].rotation;
					position = keyFrames[0].position;
					drawSimpleBoard = keyFrames[0].drawSimpleBoard;
				}
			}
		}

		internal void RemoveCurrentKeyFrame()
		{
			for (int i = 0; i < keyFrames.Count; i++)
			{
				if (keyFrames[i].frame == currentFrame)
				{
					keyFrames.RemoveAt(i);
					return;
				}
			}
		}

		public void Draw(Vector2 contactPos, SpriteBatch spriteBatch, Camera camera, Vector2 playerPosition)
		{
			frame += speed;

			if (frame >= totalFrames)
				frame -= totalFrames;

			currentFrame = (int)Math.Floor(frame);

			if (drawSimpleBoard)
			{
				SpriteHandler.Draw(simpleBoard, spriteBatch, camera, contactPos + position, 1, rotation, simpleBoardOrigin, Color.White, 1f, SpriteEffects.None, 0f);
			}
			else
			{
				string key;

				if (Game1.CeilAdv(scaleX) == Game1.CeilAdv(scaleY))
					key = deck;
				else
					key = tape;

				SpriteHandler.sprites[key].Draw(spriteBatch, camera, new Rectangle((int)(position.X + contactPos.X), (int)(position.Y + contactPos.Y), (int)(width * Math.Abs(scaleX)), (int)(height * Math.Abs(scaleY))), rotation, new Vector2(width * Math.Abs(scaleX) * .5f, height * Math.Abs(scaleY) * .5f), Color.White, 1f, SpriteEffects.None, 0f);
			}
		}

		public void DrawHUD(SpriteBatch spriteBatch)
		{

		}

		/// <summary>
		/// Loads all the decks and tapes, etc
		/// </summary>
		/// <param name="Content"></param>
		public static void LoadContent(ContentManager Content)
		{
			SpriteHandler.AddSprite("Tape_Default", new Sprite(Content.Load<Texture2D>("Tape_Default")));
			SpriteHandler.AddSprite("Deck_Default", new Sprite(Content.Load<Texture2D>("Deck_Default")));
			SpriteHandler.AddSprite("SimpleBoard", new Sprite(Content.Load<Texture2D>("board")));
		}
	}
}
