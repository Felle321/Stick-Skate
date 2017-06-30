using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;

namespace SkateAnimationEditor
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		public static int screenWidth = 1280;
		public static int screenHeight = 720;
		Camera camera = new Camera(Vector2.Zero, 1.3f);
		Random rand = new Random();
		Player player;
		public static SpriteFont fontDebug;
		public static Texture2D pixel;

		//UI
		KeyboardState keyboard, keyboardPrev;
		MouseState mouse, mousePrev;
		int editMode = 0;

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			graphics.PreferredBackBufferWidth = screenWidth;
			graphics.PreferredBackBufferHeight = screenHeight;

			keyboard = Keyboard.GetState();
			keyboardPrev = keyboard;

			mouse = Mouse.GetState();
			mousePrev = mouse;

			player = new Player("Deck_Default", "Tape_Default", "SimpleBoard");
			
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// TODO: Add your initialization logic here

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			SpriteHandler.AddSprite("SimpleBoard", new Sprite(Content.Load<Texture2D>("board")));
			fontDebug = Content.Load<SpriteFont>("Font_Debug");
			pixel = Content.Load<Texture2D>("pixel");

			player.playerAnimations.LoadContent(Content);
			player.board.LoadContent(Content);

			player.SetAnimation("KickFlip");
		}

		public static string FloatToString(float f)
		{
			string text = f.ToString();

			if(text.Contains(","))
			{
				int insert = text.IndexOf(',');

				text = text.Remove(insert, 1);
				text = text.Insert(insert, ".");

				if (insert == 1 && text.IndexOf('0') == 0)
					text = text.Remove(0, 1);
			}

			text += "f";

			return text;
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			mouse = Mouse.GetState();
			keyboard = Keyboard.GetState();

			#region UI
			if(IsKeyPressed(Keys.Space))
			{
				player.animation.currentFrame = 0;
				player.board.frame = 0;
			}

			if (keyboard.IsKeyDown(Keys.LeftControl))
			{
				if(IsKeyPressed(Keys.S))
				{
					SaveAnimation();
				}

				if (mouse.LeftButton == ButtonState.Pressed)
				{
					player.textureOffset += mouse.Position.ToVector2() - mousePrev.Position.ToVector2();
				}

				if (IsKeyPressed(Keys.Down))
				{
					if (player.animation.currentFrame - 1 < 0)
						player.animation.currentFrame = player.animation.framesTotal - 1;
					else
						player.animation.currentFrame = (float)Math.Floor(player.animation.currentFrame) - 1;
				}
				else if (IsKeyPressed(Keys.Up))
				{
					if (player.animation.currentFrame + 1 > player.animation.framesTotal)
						player.animation.currentFrame = 0;
					else
						player.animation.currentFrame = (float)Math.Floor(player.animation.currentFrame) + 1;
				}

				if (keyboard.IsKeyDown(Keys.Left) && player.animation.speed >= 0)
					player.animation.speed -= 0.002f;
				else if (keyboard.IsKeyDown(Keys.Right))
					player.animation.speed += 0.002f;

				if (player.animation.speed < 0)
					player.animation.speed = 0;
			}
			else if(keyboard.IsKeyDown(Keys.LeftShift))
			{
				if (IsKeyPressed(Keys.K))
				{
					player.board.ApplyKeyFrame();
				}

				if (IsKeyPressed(Keys.R))
				{
					player.board.RemoveCurrentKeyFrame();
				}

				if (mouse.LeftButton == ButtonState.Pressed)
				{
					player.board.position += mouse.Position.ToVector2() - mousePrev.Position.ToVector2();
				}

				if (IsKeyPressed(Keys.Down))
				{
					if (player.board.frame - 1 < 0)
						player.board.frame = player.board.totalFrames - 1;
					else
						player.board.frame = player.board.currentFrame - 1;

					if (player.board.frame < 0)
						player.board.frame = 0;
				}
				else if (IsKeyPressed(Keys.Up))
				{
					if (player.board.frame + 1 > player.board.totalFrames)
						player.board.frame = 0;
					else
						player.board.frame = player.board.currentFrame + 1;
				}

				if (keyboard.IsKeyDown(Keys.Left) && player.animation.speed >= 0)
					player.board.speed -= 0.002f;
				else if (keyboard.IsKeyDown(Keys.Right))
					player.board.speed += 0.002f;


				if (player.board.speed < 0)
					player.board.speed = 0;
			}
			else
			{
				if (keyboard.IsKeyDown(Keys.Up) && keyboardPrev.IsKeyUp(Keys.Up))
					editMode++;
				else if (keyboard.IsKeyDown(Keys.Down) && keyboardPrev.IsKeyUp(Keys.Down))
					editMode--;

				switch (editMode)
				{
					case (0):
						
						break;
					case (1):
						
						break;
					case (2):
						if (mouse.LeftButton == ButtonState.Pressed)
						{
							player.textureOrigin += mouse.Position.ToVector2() - mousePrev.Position.ToVector2();
						}
						break;
					case (3):
						if (mouse.LeftButton == ButtonState.Pressed)
						{
							player.board.simpleBoardOrigin += mouse.Position.ToVector2() - mousePrev.Position.ToVector2();
						}
						break;
					case (4):
						if (keyboard.IsKeyDown(Keys.Left))
							player.board.scaleX -= 0.01f;
						else if (keyboard.IsKeyDown(Keys.Right))
							player.board.scaleX += 0.01f;

						if (player.board.scaleX < -1)
							player.board.scaleX = -1;
						else if (player.board.scaleX > 1)
							player.board.scaleX = 1;

						break;
					case (5):
						if (keyboard.IsKeyDown(Keys.Left))
							player.board.scaleY -= 0.01f;
						else if (keyboard.IsKeyDown(Keys.Right))
							player.board.scaleY += 0.01f;

						if (player.board.scaleY < -1)
							player.board.scaleY = -1;
						else if (player.board.scaleY > 1)
							player.board.scaleY = 1;

						break;
					case (6):
						if (keyboard.IsKeyDown(Keys.Left))
							player.board.rotation -= 0.01f;
						else if (keyboard.IsKeyDown(Keys.Right))
							player.board.rotation += 0.01f;
						break;
					case (7):
						if (IsKeyPressed(Keys.Left))
							player.board.totalFrames--;
						else if (IsKeyPressed(Keys.Right))
							player.board.totalFrames++;
						break;
					case (8):
						if (IsKeyPressed(Keys.Left))
							player.board.drawSimpleBoard = false;
						else if (IsKeyPressed(Keys.Right))
							player.board.drawSimpleBoard = true;
						break;
					default:
						break;
				}
			}
			#endregion


			camera.Update(rand);

			player.Update();

			camera.pos = player.Rectangle.Center.ToVector2();


			mousePrev = mouse;
			keyboardPrev = keyboard;
			base.Update(gameTime);
		}

		private void SaveAnimation()
		{
			StreamWriter sw = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/animation.txt");

			sw.WriteLine(player.GetCommand());
			sw.WriteLine(player.board.GetCommand());
			
			sw.Close();
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.get_transformation(GraphicsDevice));

			DrawRectangle(spriteBatch, player.Rectangle, Color.Red);

			player.Draw(spriteBatch, camera);

			spriteBatch.End();

			//HUD
			spriteBatch.Begin();

			player.DrawHUD(spriteBatch);

			int editY = 350;

			switch (editMode)
			{
				case (0):
					spriteBatch.DrawString(Game1.fontDebug, "TextureOffset: " + player.textureOffset, new Vector2(20, editY), Color.Black);
					break;
				case (1):
					spriteBatch.DrawString(Game1.fontDebug, "BoardPosition: " + player.board.position.ToString(), new Vector2(20, editY), Color.Black);
					break;
				case (2):
					spriteBatch.DrawString(Game1.fontDebug, "TextureOrigin: " + player.textureOrigin, new Vector2(20, editY), Color.Black);
					break;
				case (3):
					spriteBatch.DrawString(Game1.fontDebug, "SimpleBoardOrigin: " + player.board.simpleBoardOrigin, new Vector2(20, editY), Color.Black);
					break;
				case (4):
					spriteBatch.DrawString(Game1.fontDebug, "ScaleX: " + player.board.scaleX, new Vector2(20, editY), Color.Black);
					break;
				case (5):
					spriteBatch.DrawString(Game1.fontDebug, "ScaleY: " + player.board.scaleY, new Vector2(20, editY), Color.Black);
					break;
				case (6):
					spriteBatch.DrawString(Game1.fontDebug, "Rotation: " + player.board.rotation, new Vector2(20, editY), Color.Black);
					break;
				case (7):
					spriteBatch.DrawString(Game1.fontDebug, "BoardTotalFrames: " + player.board.totalFrames, new Vector2(20, editY), Color.Black);
					break;
				case (8):
					spriteBatch.DrawString(Game1.fontDebug, "DrawSimpleBoard: " + player.board.drawSimpleBoard.ToString(), new Vector2(20, editY), Color.Black);
					break;
				default:
					break;
			}

			spriteBatch.End();
			
			base.Draw(gameTime);
		}

		public string KeyBoardInputNumbers(string text)
		{
			if (IsKeyPressed(Keys.Back))
				text.Remove(text.Length - 1, 1);
			else
			{
				if (IsKeyPressed(Keys.D0))
					text += "0";
				if (IsKeyPressed(Keys.D1))
					text += "1";
				if (IsKeyPressed(Keys.D2))
					text += "2";
				if (IsKeyPressed(Keys.D3))
					text += "3";
				if (IsKeyPressed(Keys.D4))
					text += "4";
				if (IsKeyPressed(Keys.D5))
					text += "5";
				if (IsKeyPressed(Keys.D6))
					text += "6";
				if (IsKeyPressed(Keys.D7))
					text += "7";
				if (IsKeyPressed(Keys.D8))
					text += "8";
				if (IsKeyPressed(Keys.D9))
					text += "9";
				if (IsKeyPressed(Keys.OemPeriod) || IsKeyPressed(Keys.OemComma))
					text += ".";
			}

			return text;
		}

		public static float StringToFloat(string text)
		{
			char dec;
			if (text.Contains("."))
			{
				dec = '.';
			}
			else
				dec = ',';

			string[] array = text.Split(dec);

			if (array.Length < 2)
			{
				return int.Parse(text);
			}
			else return int.Parse(array[0]) + float.Parse(array[1]) / (float)Math.Pow(10, array[1].Length);

		}

		public bool IsKeyPressed(Keys key)
		{
			if (keyboard.IsKeyDown(key) && keyboardPrev.IsKeyUp(key))
				return true;
			else
				return false;
		}

		/// <summary>
		/// Floors the given number, reverses if negative
		/// </summary>
		/// <param name="number"></param>
		/// <returns></returns>
		public static int FloorAdv(float number)
		{
			if (number < 0)
				return (int)Math.Ceiling(number);
			else
				return (int)Math.Floor(number);
		}

		/// <summary>
		/// Raises the given number to the nearest int, reversed if negative
		/// </summary>
		/// <param name="number"></param>
		/// <returns></returns>
		public static int CeilAdv(float number)
		{
			if (number > 0)
				return (int)Math.Ceiling(number);
			else
				return (int)Math.Floor(number);
		}

		/// <summary>
		/// Draw a line between two Point positions in a chosen color
		/// </summary>
		/// <param name="spriteBatch"></param>
		/// <param name="point0"></param>
		/// <param name="point1"></param>
		/// <param name="color"></param>
		public static void DrawLine(SpriteBatch spriteBatch, Vector2 v0, Vector2 v1, Color color, float thickness)
		{
			float length = Vector2.Distance(v0, v1);
			float deg = 0;
			bool possible = true;

			if (v0.ToPoint() == v1.ToPoint())
				possible = false;

			deg = (float)Math.Atan2(v1.Y - v0.Y, v1.X - v0.X);

			if (possible)
			{
				spriteBatch.Draw(pixel, v0, null, color, deg, Vector2.Zero, new Vector2(length, thickness), SpriteEffects.None, 0f);
			}
		}

		/// <summary>
		/// Draws a rectangle with the given color
		/// </summary>
		/// <param name="spriteBatch"></param>
		/// <param name="rectangle"></param>
		/// <param name="color"></param>
		public static void DrawRectangle(SpriteBatch spriteBatch, Rectangle rectangle, Color color)
		{
			DrawLine(spriteBatch, new Vector2(rectangle.X, rectangle.Y), new Vector2(rectangle.X + rectangle.Width, rectangle.Y), color, 8);
			DrawLine(spriteBatch, new Vector2(rectangle.X + rectangle.Width, rectangle.Y), new Vector2(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height), color, 8);
			DrawLine(spriteBatch, new Vector2(rectangle.X, rectangle.Y + rectangle.Height), new Vector2(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height), color, 8);
			DrawLine(spriteBatch, new Vector2(rectangle.X, rectangle.Y), new Vector2(rectangle.X, rectangle.Y + rectangle.Height), color, 8);
		}
	}
}
