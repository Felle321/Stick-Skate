using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Skate
{
	public class Game1 : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		public static int deviceWidth, deviceHeight, screenWidth, screenHeight;
		Vector2 screenScale;
		RenderTarget2D mainRenderTarget;
		TouchCollection touchCollection = new TouchCollection();
		public static Texture2D pixel;
		Camera camera = new Camera(Vector2.Zero, 1);
		Random rand = new Random();
		Player player;

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			screenWidth = 1280;
			screenHeight = 720;
			graphics.IsFullScreen = true;
			graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
		}
		

		protected override void Initialize()
		{

			base.Initialize();
		}
		

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);
			graphics.PreferredBackBufferWidth = screenWidth;
			graphics.PreferredBackBufferHeight = screenHeight;
			mainRenderTarget = new RenderTarget2D(GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
			deviceWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
			deviceHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
			screenScale.X = deviceWidth / screenWidth;
			screenScale.Y = deviceHeight / screenHeight;
			pixel = Content.Load<Texture2D>("pixel");

			InitializeNewGame();
		}

		private void InitializeNewGame()
		{
			player = new Player(Content.Load<Texture2D>("guy"));
		}

		protected override void UnloadContent()
		{

		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			touchCollection = TouchPanel.GetState();




			camera.pos = player.Centre;
			camera.Update(rand);
			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			DrawGame();

			GraphicsDevice.SetRenderTarget(null);
			GraphicsDevice.Clear(Color.Red);
			spriteBatch.Begin();
			spriteBatch.Draw((Texture2D)mainRenderTarget, new Rectangle(0, 0, deviceWidth, deviceHeight), Color.White);
			spriteBatch.End();
			base.Draw(gameTime);
		}

		protected void DrawGame()
		{
			GraphicsDevice.SetRenderTarget(mainRenderTarget);
			GraphicsDevice.Clear(Color.White);

			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, null, null, camera.get_transformation(GraphicsDevice));

			player.Draw(spriteBatch);

			spriteBatch.End();
		}
	}
}
