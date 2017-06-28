using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

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
		SpriteFont fontDebug;

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

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

			player.playerAnimations.LoadContent(Content);
			player.board.LoadContent(Content);

			player.SetAnimation("Idle");
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
			camera.Update(rand);

			player.Update();

			camera.pos = player.Rectangle.Center.ToVector2();

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.get_transformation(GraphicsDevice));
			player.Draw(spriteBatch, camera);
			spriteBatch.End();

			player.DrawBoard(spriteBatch, GraphicsDevice, camera);

			base.Draw(gameTime);
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
