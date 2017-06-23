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
		List<Solid> solids = new List<Solid>();
		public static float gravity = 1f;

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

			solids.Add(new Solid(new Point(player.Rectangle.Left, 200), new Point(player.Rectangle.Left + 400, 200)));
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

			GenerateTerrain();


			PlayerCollisionsCalculation();

			player.Update();

			camera.pos = player.Centre;
			camera.Update(rand);
			base.Update(gameTime);
		}

		private void GenerateTerrain()
		{
			if (solids[solids.Count - 1].b.X <= camera.rectangle.Right)
				solids.Add(new Solid(solids[solids.Count - 1].b, solids[solids.Count - 1].b + new Point(rand.Next(200), rand.Next(100) - 50)));
		}

		private void PlayerCollisionsCalculation()
		{
			if(player.onGround && player.solidRef != -1)
			{
				if(LineIntersectsRect(solids[player.solidRef].a.ToVector2(), solids[player.solidRef].b.ToVector2(), new Rectangle(player.Rectangle.X, player.Rectangle.Y, player.Rectangle.Width, player.Rectangle.Height + 1)))
				{
					player.onGround = false;
					player.solidRef = -1;
				}
				else
				{
					player.angle = (float)Math.Atan(solids[player.solidRef].k);
				}
			}

			for (int i = 0; i < solids.Count; i++)
			{
				if(solids[i].b.X < camera.rectangle.X)
				{
					solids.RemoveAt(i);
					i--;

					if(player.solidRef > i)
					{
						player.solidRef--;
					}
				}

				if(!player.onGround)
				{
					if (LineIntersectsRect(solids[i].a.ToVector2(), solids[i].b.ToVector2(), player.Rectangle))
					{
						player.SetGround(true);
						player.SetContactYPos(solids[i].GetY(player.Centre.X));
						player.solidRef = i;
					}
				}
			}
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


			for (int i = 0; i < solids.Count; i++)
			{
				solids[i].Draw(spriteBatch);
			}


			player.Draw(spriteBatch);

			spriteBatch.End();
		}



		/// <summary>
		/// Checks if a line intersects a rectangle
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <param name="r"></param>
		/// <returns></returns>
		public static bool LineIntersectsRect(Vector2 p1, Vector2 p2, Rectangle r)
		{
			return LineIntersectsLine(p1, p2, new Vector2(r.X, r.Y), new Vector2(r.X + r.Width, r.Y)) ||
				   LineIntersectsLine(p1, p2, new Vector2(r.X + r.Width, r.Y), new Vector2(r.X + r.Width, r.Y + r.Height)) ||
				   LineIntersectsLine(p1, p2, new Vector2(r.X + r.Width, r.Y + r.Height), new Vector2(r.X, r.Y + r.Height)) ||
				   LineIntersectsLine(p1, p2, new Vector2(r.X, r.Y + r.Height), new Vector2(r.X, r.Y)) ||
				   (r.Contains(new Point((int)p1.X, (int)p1.Y)) && r.Contains(new Point((int)p2.X, (int)p2.Y)));
		}

		/// <summary>
		/// Checks if a line intersects a rectangle
		/// </summary>
		/// <param name="l1p1"></param>
		/// <param name="l1p2"></param>
		/// <param name="l2p1"></param>
		/// <param name="l2p2"></param>
		/// <returns></returns>
		public static bool LineIntersectsLine(Vector2 l1p1, Vector2 l1p2, Vector2 l2p1, Vector2 l2p2)
		{
			float q = (l1p1.Y - l2p1.Y) * (l2p2.X - l2p1.X) - (l1p1.X - l2p1.X) * (l2p2.Y - l2p1.Y);
			float d = (l1p2.X - l1p1.X) * (l2p2.Y - l2p1.Y) - (l1p2.Y - l1p1.Y) * (l2p2.X - l2p1.X);

			if (d == 0)
			{
				return false;
			}

			float r = q / d;

			q = (l1p1.Y - l2p1.Y) * (l1p2.X - l1p1.X) - (l1p1.X - l2p1.X) * (l1p2.Y - l1p1.Y);
			float s = q / d;

			if (r < 0 || r > 1 || s < 0 || s > 1)
			{
				return false;
			}

			return true;
		}
	}
}
