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
		TouchCollection touchCollection, touchCollectionPrev = new TouchCollection();
		Dictionary<int, Vector2> touchInitialPositions = new Dictionary<int, Vector2>();
		Dictionary<int, int> touchStationaryTimer = new Dictionary<int, int>();
		public static Texture2D pixel;
		Camera camera = new Camera(Vector2.Zero, 1);
		Random rand = new Random();
		Player player;
		List<Platform> platforms = new List<Platform>();
		List<Slope> slopes = new List<Slope>();
		public static float gravity = .7f;
		float friction = .98f;
		SpriteFont fontDebug;
		public enum TouchAction
		{
			None,
			SwipeLeft,
			SwipeRight,
			SwipeUp,
			SwipeDown,
			Touch,
			Hold
		}

		List<Trick.Flip> AvailibleFlipTricks = new List<Trick.Flip>();
		List<Trick.Grind> AvailibleGrindTricks = new List<Trick.Grind>();
		List<Trick.Grab> AvailibleGrabTricks = new List<Trick.Grab>();

		Combo combo = new Combo();
		List<Trick> trickQueue = new List<Trick>();
		
		int jumpChargeMax = 20;
		int jumpStrength = 18;

		int tryGrind = 0;
		int tryGrindMax = 40;

		int score = 0;
		string lastTrick = "";
		int lastScore = 0;

		float debugLastAngle = 0f;
		TouchAction debugLastSwipe = TouchAction.None;

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

			InitializeNewGame();

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
			screenScale.X = deviceWidth / (float)screenWidth;
			screenScale.Y = deviceHeight / (float)screenHeight;
			pixel = Content.Load<Texture2D>("pixel");
			fontDebug = Content.Load<SpriteFont>("font_debug");

			PlayerAnimation.LoadContent(Content);
			Board.LoadContent(Content);

			player.SetAnimation("KickFlip");

		}

		private void InitializeNewGame()
		{
			player = new Player("Deck_Default", "Tape_Default", "SimpleBoard");
			platforms.Add(new Platform(0, new Rectangle(0, 400, 800, 40), Platform.Type.Rectangle));
			score = 0;
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

			if (tryGrind > 0)
				tryGrind--;

			if (player.onGround)
			{
				if (player.grind)
				{
					if(combo.tricks.Count == 0)
						combo.tricks.Add(player.GrindTrick.GetName());
					else if (combo.tricks[combo.tricks.Count - 1] != player.GrindTrick.GetName())
						combo.tricks.Add(player.GrindTrick.GetName());

					combo.lastScore += player.GrindTrick.GetScore();
					combo.score += player.GrindTrick.GetScore();
					player.speed += combo.availibleSpeed;
					combo.availibleSpeed = 0;
				}

				if (!player.onGroundPrev)
				{
					if (trickQueue.Count > 1)
					{
						//LOSE
					}
					else if (trickQueue.Count == 1)
					{
						if (trickQueue[0].totalFrames - trickQueue[0].frame < 10)
						{
							combo.lastTrickEnd = combo.time + trickQueue[0].totalFrames - trickQueue[0].frame;
							trickQueue[0].frame = trickQueue[0].totalFrames;
						}
						else
						{
							//LOSE
						}
					}

					if (!player.grind)
					{
						score += combo.Finish();
						trickQueue.Clear();
					}
				}
				else
				{

				}
			}
			else
			{
				if (trickQueue.Count > 0)
				{
					if (trickQueue[0].frame == 0)
					{
						combo.lastScore = trickQueue[0].GetScore();
						combo.tricks.Add(trickQueue[0].GetName());
						combo.multiplier++;
						combo.availibleSpeed += trickQueue[0].GetSpeed();
						combo.lastQuality = trickQueue[0].quality.ToString();
						trickQueue[0].Update();
					}
					else if(trickQueue[0].frame == trickQueue[0].totalFrames)
					{
						combo.lastTrickEnd = combo.time;
						combo.score += combo.lastScore;
						trickQueue.RemoveAt(0);
					}
					else
					{
						trickQueue[0].Update();
					}
				}

				player.jumpCharge = 0;
			}

			switch (GetTouchAction())
			{
				case TouchAction.None:
					if(player.state == Player.State.Ground || player.state == Player.State.Grind)
					{
						if (player.jumpCharge > 0.2f * jumpChargeMax)
						{
							player.movement.Y = -jumpStrength * (player.jumpCharge / (float)jumpChargeMax);
							player.onSlope = false;
							player.onGround = false;
							player.jumpCharge = 0;
							player.SetAnimation("Jump");
							player.state = Player.State.Air;
						}
						else
							player.jumpCharge = 0;
					}
					break;
				case TouchAction.SwipeLeft:
					debugLastSwipe = TouchAction.SwipeLeft;

					player.jumpCharge = 0;

					if (player.onGround)
					{
						player.SetAnimation("Accelerate");
					}
					else
					{
						PerformTrickFlip();
					}

					break;
				case TouchAction.SwipeRight:
					debugLastSwipe = TouchAction.SwipeRight;

					if (!player.onGround)
					{
						PerformTrickGrab();
					}
					break;
				case TouchAction.SwipeUp:
					debugLastSwipe = TouchAction.SwipeUp;

					break;
				case TouchAction.SwipeDown:
					debugLastSwipe = TouchAction.SwipeDown;

					if(player.state == Player.State.Air)
					{
						tryGrind = tryGrindMax;
						player.tryGrind = true;
					}

					break;
				case TouchAction.Touch:
					if(player.state == Player.State.Ground)
						player.SetAnimation("JumpCharge");
					break;
				case TouchAction.Hold:
					if (player.jumpCharge < jumpChargeMax && player.onGround)
					{
						if(player.animationKey != "JumpCharge")
							player.SetAnimation("JumpCharge");

						player.jumpCharge++;

						player.animation.currentFrame = (player.jumpCharge / (float)jumpChargeMax) * player.animation.framesTotal;
					}
					else if (player.jumpCharge == jumpChargeMax && player.onGround)
					{
						player.animation.currentFrame = player.animation.framesTotal;
					}
					break;
				default:
					break;
			}

			GenerateTerrain();

			combo.Update();

			UpdatePlayerState();

			player.Update();

			MoveObject(ref player.position, player.Rectangle, ref player.movement, ref player.onGround, player.onGroundPrev, ref player.slope, ref player.onSlope, player.bounceFactor, ref player.platform, ref player.platformPrev, player.tryGrind);

			camera.zoomTarget = 1.1f - (player.speed / player.maxSpeed) * .3f;
			camera.target = player.Centre + new Vector2(600 + (player.speed / player.maxSpeed) * 200, -100);
			camera.Update(rand);
			touchCollectionPrev = touchCollection;
			base.Update(gameTime);
		}

		private void UpdatePlayerState()
		{
			if (!player.grind && tryGrind == 0)
				player.tryGrind = false;

			if (trickQueue.Count == 0)
			{
				if (player.onGround || player.onSlope)
				{
					player.state = Player.State.Ground;
					if (player.animationKey != "Idle" && player.jumpCharge == 0 && (player.animationKey != "Accelerate" || (player.animationKey == "Accelerate" && player.animation.animationEnd)) && !player.grind)
					{
						player.SetAnimation("Idle");
					}
					else if (player.animationKey == "Accelerate" && (int)Math.Floor(player.animation.currentFrame) == 8)
						player.speed += 3;
				}
				else
				{
					player.state = Player.State.Air;
					if (player.animationKey != "Jump")
					{
						player.SetAnimation("Jump");
						player.animation.currentFrame = player.animation.framesTotal;
					}

					player.grind = false;

					if (player.animation.animationEnd)
					{
						player.animation.speed = 0;
						player.animation.currentFrame = player.animation.framesTotal;
						player.board.speed = 0;
						player.board.frame = player.board.totalFrames;
					}
				}
			}
			else if (!player.grind)
			{
				player.state = Player.State.Trick;
			}
			else
			{
				player.state = Player.State.Grind;
			}
		}

		public void PerformTrickFlip()
		{
			Trick.FlipTricks trick = Trick.FlipTricks.KickFlip;
			
			if(!player.onGround)
			{
				if(trickQueue.Count == 1)
				{
					trickQueue.Add(new Trick.Flip(trick, trickQueue[0].GetTimingMultiplier()));
				}
				else if(trickQueue.Count == 0)
				{
					trickQueue.Add(new Trick.Flip(trick, combo.GetTimingMultiplier()));
				}
				else
				{
					//BALANCE PENALTY
				}
			}

			player.SetAnimation(trickQueue[0].GetName());
		}

		public void PerformTrickGrab()
		{

		}

		public void PerformTrickGrind()
		{
			player.GrindTrick = new Trick.Grind(Trick.GrindTricks.FiftyFifty, 1.2f - .4f * (40 / (float)tryGrind));

			player.SetAnimation(player.GrindTrick.GetName());
		}

		private void GenerateTerrain()
		{
			int i = platforms.Count - 1;

			if (platforms[i].type == Platform.Type.Grind)
				i--;

			if (platforms[i].rectangle.Right <= camera.rectangle.Right + 40)
			{
				if (rand.Next(4) == 0)
				{
					int height = rand.Next(240) + 40;
					int width = rand.Next(100) + 100;
					slopes.Add(new Slope(new Rectangle(platforms[i].rectangle.Right - width, platforms[i].rectangle.Y - height, width, height), true));
					platforms.Add(new Platform(platforms.Count, new Rectangle(platforms[i].rectangle.Right + 1, platforms[i].rectangle.Y - height, rand.Next(400) + 500, 40), Platform.Type.Rectangle));
				}
				else
					platforms.Add(new Platform(platforms.Count, new Rectangle(platforms[i].rectangle.Right + 1, platforms[i].rectangle.Y, rand.Next(400) + 500, 40), Platform.Type.Rectangle));

				if(rand.Next(5) == 0)
				{
					platforms.Add(new Platform(platforms.Count, new Rectangle(platforms[platforms.Count - 1].rectangle.Left, platforms[platforms.Count - 1].rectangle.Y - 70, rand.Next(160) + 340, 40), Platform.Type.Grind));
				}
			}
		}

		private TouchAction GetTouchAction()
		{
			TouchAction touchAction = TouchAction.None;

			for (int i = 0; i < touchCollection.Count; i++)
			{
				//Getting prev touch and getting a TouchAction.Touch
				int touchPrev = -1;
				for (int j = 0; j < touchCollectionPrev.Count; j++)
				{
					if (touchCollection[i].Id == touchCollectionPrev[j].Id)
					{
						touchPrev = j;
						break;
					}
				}

				//An absence of a previous touch would indicate a new touchposition (for swipes)
				if (touchPrev == -1)
				{
					touchStationaryTimer.Add(touchCollection[i].Id, 0);
					touchInitialPositions.Add(touchCollection[i].Id, touchCollection[i].Position);
					return TouchAction.Touch;
				}
				else
				{
					//A touch is "held down" and the touchAction will be .Hold until further changes
					touchAction = TouchAction.Hold;

					if(Vector2.Distance(touchInitialPositions[touchCollectionPrev[i].Id], touchCollectionPrev[i].Position) > 350 * screenScale.Length())
					{
						touchInitialPositions[touchCollectionPrev[i].Id] = GetMidpoint(touchInitialPositions[touchCollectionPrev[i].Id], touchCollectionPrev[i].Position);
					}

					if (touchStationaryTimer[touchCollection[i].Id] > 5)
					{
						touchStationaryTimer[touchCollection[i].Id] = 0;
						touchInitialPositions[touchCollection[i].Id] = touchCollection[i].Position;
					}

					//Reset positions if touch is stationary for too long
					if (Vector2.Distance(touchCollection[i].Position, touchCollectionPrev[touchPrev].Position) < 20 * screenScale.Length())
					{
						touchStationaryTimer[touchCollection[i].Id]++;
					}
					else
					{
						touchStationaryTimer[touchCollection[i].Id] = 0;
					}
				}
			}

			for (int i = 0; i < touchCollectionPrev.Count; i++)
			{
				//Getting prev touch and getting a TouchAction.Touch
				int touchCurrent = -1;
				for (int j = 0; j < touchCollection.Count; j++)
				{
					if (touchCollection[j].Id == touchCollectionPrev[i].Id)
					{
						touchCurrent = j;
						break;
					}
				}

				//This would indicate a swipe
				if (touchCurrent == -1)
				{
					if(Vector2.Distance(touchInitialPositions[touchCollectionPrev[i].Id], touchCollectionPrev[i].Position) > 30 * screenScale.Length())
					{
						float angle = MathHelper.ToDegrees(GetAngle(touchInitialPositions[touchCollectionPrev[i].Id], touchCollectionPrev[i].Position));
						debugLastAngle = angle;
						touchInitialPositions.Remove(touchCollectionPrev[i].Id);
						touchStationaryTimer.Remove(touchCollectionPrev[i].Id);

						if (angle < 45
							|| angle >= 315)
							return TouchAction.SwipeRight;
						else if (angle >= 45 && angle < 135)
							return TouchAction.SwipeUp;
						else if (angle >= 135 && angle < 215)
							return TouchAction.SwipeLeft;
						else if (angle >= 215 && angle < 315)
							return TouchAction.SwipeDown;
					}
				}
			}

			return touchAction;
		}
		
		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			DrawGame();

			DrawHUD();

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

			for (int i = 0; i < platforms.Count; i++)
			{
				platforms[i].Draw(spriteBatch);
			}

			for (int i = 0; i < slopes.Count; i++)
			{
				slopes[i].Draw(spriteBatch);
			}

			player.Draw(spriteBatch, camera);

			spriteBatch.End();
		}

		protected void DrawHUD()
		{
			spriteBatch.Begin();

			//DrawTouches();

			spriteBatch.DrawString(fontDebug, "Ground: " + player.onGround.ToString(), new Vector2(600, 50), Color.Black);
			spriteBatch.DrawString(fontDebug, "Slope: " + player.onSlope.ToString(), new Vector2(600, 100), Color.Black);
			spriteBatch.DrawString(fontDebug, "JumpCharge: " + player.jumpCharge.ToString(), new Vector2(600, 150), Color.Black);
			spriteBatch.DrawString(fontDebug, "TryGrind: " + tryGrind.ToString(), new Vector2(600, 200), Color.Black);
			spriteBatch.DrawString(fontDebug, "Grind: " + player.grind.ToString(), new Vector2(600, 250), Color.Black);
			spriteBatch.DrawString(fontDebug, "Player.TryGrind: " + player.tryGrind.ToString(), new Vector2(600, 300), Color.Black);
			spriteBatch.DrawString(fontDebug, "AnimationKey: " + player.animationKey.ToString(), new Vector2(600, 350), Color.Black);
			spriteBatch.DrawString(fontDebug, "State: " + player.state.ToString(), new Vector2(600, 400), Color.Black);

			DrawCombo();

			spriteBatch.End();
		}

		private void DrawCombo()
		{
			if (combo.tricks.Count > 0)
			{
				spriteBatch.DrawString(fontDebug, combo.tricks[combo.tricks.Count - 1].ToString(), new Vector2(30, 50), Color.Black);
				spriteBatch.DrawString(fontDebug, combo.lastQuality.ToString(), new Vector2(30, 100), Color.Black);
				spriteBatch.DrawString(fontDebug, player.state.ToString(), new Vector2(30, 150), Color.Black);
				spriteBatch.DrawString(fontDebug, combo.lastScore.ToString(), new Vector2(30, 220), Color.Black);
				spriteBatch.DrawString(fontDebug, combo.score.ToString(), new Vector2(30, 270), Color.Black);
				spriteBatch.DrawString(fontDebug, combo.multiplier.ToString(), new Vector2(30, 320), Color.Black);

				for (int i = 0; i < combo.tricks.Count; i++)
				{
					spriteBatch.DrawString(fontDebug, combo.tricks[i], new Vector2(30, 400 + i * 50), Color.Black);
				}
			}
		}

		private void DrawTouches()
		{
			spriteBatch.DrawString(fontDebug, debugLastAngle.ToString(), new Vector2(30, 50), Color.Black);
			spriteBatch.DrawString(fontDebug, debugLastSwipe.ToString(), new Vector2(30, 100), Color.Black);
			spriteBatch.DrawString(fontDebug, GetTouchAction().ToString(), new Vector2(30, 150), Color.Black);

			for (int i = 0; i < touchCollectionPrev.Count; i++)
			{
				spriteBatch.DrawString(fontDebug, touchCollectionPrev[i].Position.ToString(), new Vector2(30, 200 + i * 50), Color.Black);

				if (touchInitialPositions.ContainsKey(touchCollectionPrev[i].Id))
					DrawLine(spriteBatch, touchInitialPositions[touchCollectionPrev[i].Id] / screenScale, touchCollectionPrev[i].Position / screenScale, Color.Blue, 5);
			}
		}

		/// <summary>
		/// Moves an object according to a Vector2 and returns an offset for the texture (slopes)
		/// </summary>
		/// <param name="position">Position of the object</param>
		/// <param name="rectangle">Rectangle of the object</param>
		/// <param name="movement">Vector to move by</param>
		/// <param name="onGround">OnGround bool</param>
		/// <returns></returns>
		public void MoveObject(ref Vector2 position, Rectangle rectangle, ref Vector2 movement, ref bool onGround, bool onGroundPrev, ref Slope slope, ref bool onSlope, float bounceFactor, ref int platformID, ref int platformIDPrev, bool grind)
		{
			int platformPrev = platformIDPrev;
			if (platformID != -1)
				platformPrev = platformID;
			onGround = false;
			bool onSlopePrev = onSlope;
			onSlope = false;
			bool skip = true;
			Vector2 oldPosition = new Vector2(position.X, position.Y);
			Vector2 newPosition = new Vector2(position.X + movement.X, position.Y + movement.Y);
			Rectangle oldRectangle = rectangle;
			Rectangle newRectangle = new Rectangle(FloorAdv(newPosition.X), FloorAdv(newPosition.Y), rectangle.Width, rectangle.Height);

			if (player.grind)
				grind = true;

			//Checks if we can skip the entire process
			#region Skip
			for (int i = 0; i < platforms.Count; i++)
			{
				if (newRectangle.Intersects(platforms[i].rectangle))
				{
					skip = false;
					break;
				}
			}
			if (skip)
			{
				for (int i = 0; i < slopes.Count; i++)
				{
					if (newRectangle.Intersects(slopes[i].rectangle))
					{
						skip = false;
						break;
					}
				}
			}
			#endregion

			if (!skip)
			{
				//Checks for collision with slopes
				for (int j = 0; j < slopes.Count; j++)
				{
					if (rectangle.Intersects(slopes[j].rectangle) || Game1.LineIntersectsLine(new Vector2(slopes[j].rectangle.X, slopes[j].rectangle.Y + slopes[j].rectangle.Height), new Vector2(slopes[j].rectangle.X + slopes[j].rectangle.Width, slopes[j].rectangle.Y), oldPosition, newPosition))
					{
						if (slopes[j].faceRight)
						{
							//RIGHT
							float anglePosition = Game1.GetAngle(new Vector2(slopes[j].rectangle.X, slopes[j].rectangle.Bottom), new Vector2(position.X + FloorAdv(movement.X) + rectangle.Width, position.Y + FloorAdv(movement.Y)));

							if (Game1.LineIntersectsRect(new Vector2(slopes[j].rectangle.X, slopes[j].rectangle.Y + slopes[j].rectangle.Height), new Vector2(slopes[j].rectangle.X + slopes[j].rectangle.Width, slopes[j].rectangle.Y), newRectangle)
								|| (onGroundPrev && movement.Y == 0 && Game1.LineIntersectsRect(new Vector2(slopes[j].rectangle.X, slopes[j].rectangle.Y + slopes[j].rectangle.Height), new Vector2(slopes[j].rectangle.X + slopes[j].rectangle.Width, slopes[j].rectangle.Y), new Rectangle((int)position.X + FloorAdv(movement.X), (int)position.Y + FloorAdv(-movement.X), rectangle.Width, rectangle.Height)))
								|| slopes[j].angle > anglePosition
								|| Game1.LineIntersectsLine(new Vector2(slopes[j].rectangle.X, slopes[j].rectangle.Y + slopes[j].rectangle.Height), new Vector2(slopes[j].rectangle.X + slopes[j].rectangle.Width, slopes[j].rectangle.Y), oldPosition, newPosition))
							{
								onSlope = true;
								slope = slopes[j];
								float offset = slopes[j].Function(position.X - slopes[j].rectangle.X + rectangle.Width);
								if (offset < 0)
									offset = -offset;
								offset = FloorAdv(offset);
								position.X = position.X + movement.X;
								position.Y = slopes[j].rectangle.Y + slopes[j].rectangle.Height - offset - rectangle.Height;

								if (bounceFactor == 0)
								{
									onGround = true;
									platformID = slopes[j].platformID;
									movement.Y = 0;
								}
								else
								{
									float length = movement.Length() * bounceFactor * friction * 1 / rectangle.Width * rectangle.Height;
									if (length < 1)
									{
										movement = Vector2.Zero;
									}
									Vector2 normal = Game1.GetVector2(slopes[j].angle + MathHelper.ToRadians(90)) * movement.Length() / 2;
									movement = new Vector2(normal.X + movement.X, normal.Y + movement.Y);
									if (movement.X < 0)
										movement.X *= friction * 0.8f;
								}
							}
							else if (bounceFactor == 0 && onGroundPrev && movement.Y >= 0)
							{
								onSlope = true;
								slope = slopes[j];
								position.X = position.X + movement.X;
								int offset = FloorAdv(slopes[j].Function(position.X - slopes[j].rectangle.X + rectangle.Width));
								if (offset < 0)
									offset = -offset;
								offset = FloorAdv(offset);
								position.Y = slopes[j].rectangle.Y + slopes[j].rectangle.Height - offset - rectangle.Height;
								onGround = true;
								platformID = slopes[j].platformID;
								movement.Y = 0;
							}
						}
						else
						{
							//LEFT
							float anglePosition = Game1.GetAngle(new Vector2(slopes[j].rectangle.Right, slopes[j].rectangle.Bottom), position + new Vector2(FloorAdv(movement.X), FloorAdv(movement.Y))) + MathHelper.ToRadians(180);

							if (Game1.LineIntersectsRect(new Vector2(slopes[j].rectangle.X, slopes[j].rectangle.Y), new Vector2(slopes[j].rectangle.X + slopes[j].rectangle.Width, slopes[j].rectangle.Y + slopes[j].rectangle.Height), new Rectangle((int)position.X + FloorAdv(movement.X), (int)position.Y + FloorAdv(movement.Y), rectangle.Width, rectangle.Height))
								|| (onGroundPrev && movement.Y == 0 && Game1.LineIntersectsRect(new Vector2(slopes[j].rectangle.X, slopes[j].rectangle.Y), new Vector2(slopes[j].rectangle.X + slopes[j].rectangle.Width, slopes[j].rectangle.Y + slopes[j].rectangle.Height), new Rectangle((int)position.X + FloorAdv(movement.X), (int)position.Y + FloorAdv(movement.X), rectangle.Width, rectangle.Height)))
								|| anglePosition > slopes[j].angle)
							{
								slope = slopes[j];
								onSlope = true;
								position.X = position.X + movement.X;
								float offset = slopes[j].Function(position.X - slopes[j].rectangle.X);
								if (offset > 0)
									offset = -offset;
								offset = FloorAdv(offset);
								position.Y = slopes[j].rectangle.Y - offset - rectangle.Height;

								if (bounceFactor == 0)
								{
									onGround = true;
									platformID = slopes[j].platformID;
									movement.Y = 0;
								}
								else
								{
									float length = movement.Length() * bounceFactor * friction * 1 / rectangle.Width * rectangle.Height;
									if (length < 1)
									{
										movement = Vector2.Zero;
									}
									Vector2 normal = Game1.GetVector2(slopes[j].angle - MathHelper.ToRadians(90)) * movement.Length() / 2;
									movement = new Vector2(normal.X + movement.X, normal.Y + movement.Y);
									if (movement.X > 0)
										movement.X *= friction * 0.8f;
								}
							}
							else if (bounceFactor == 0 && onGroundPrev && movement.Y >= 0)
							{
								slope = slopes[j];
								onSlope = true;
								position.X = position.X + movement.X;
								float offset = (float)Math.Floor(slopes[j].Function(position.X - slopes[j].rectangle.X));
								if (offset > 0)
									offset = -offset;
								offset = FloorAdv(offset);
								position.Y = slopes[j].rectangle.Y - offset - rectangle.Height;
								onGround = true;
								platformID = slopes[j].platformID;
								movement.Y = 0;
							}
						}
					}
					else if (new Rectangle(FloorAdv(newPosition.X), FloorAdv(newPosition.Y + 1), rectangle.Width, rectangle.Height).Intersects(slopes[j].rectangle))
					{
						if ((slopes[j].faceRight && rectangle.Right >= slopes[j].rectangle.Right) || (!slopes[j].faceRight && rectangle.Left <= slopes[j].rectangle.Left))
						{
							onGround = true;
							platformID = slopes[j].platformID;
							movement.Y = 0;
						}
					}
				}


				//Checks for collisions with normal rectangles
				for (int j = 0; j < platforms.Count; j++)
				{
					if (new Rectangle(FloorAdv(oldPosition.X), FloorAdv(newPosition.Y), rectangle.Width, rectangle.Height).Intersects(platforms[j].rectangle) && platforms[j].type == Platform.Type.Rectangle)
					{
						if (movement.Y > 0 && rectangle.Bottom - platforms[j].rectangle.Y < movement.Y)
						{
							position.Y = platforms[j].rectangle.Y - rectangle.Height;
							onGround = true;
							platformID = j;
						}
						else if (movement.Y < 0)
						{
							position.Y = platforms[j].rectangle.Y + platforms[j].rectangle.Height;
						}
						if (bounceFactor <= 0)
							movement.Y = 0;
						else
						{
							movement.Y = movement.Y * bounceFactor * -1;
						}
					}
					else if (movement.Y != 0)
					{
						onGround = false;
					}
					else if (new Rectangle(FloorAdv(oldPosition.X), FloorAdv(newPosition.Y + 1), rectangle.Width, rectangle.Height).Intersects(platforms[j].rectangle))
					{
						onGround = true;
						platformID = j;
					}

					if (new Rectangle(FloorAdv(newPosition.X), FloorAdv(oldPosition.Y), rectangle.Width, rectangle.Height).Intersects(platforms[j].rectangle) && platforms[j].type == Platform.Type.Rectangle)
					{
						if (movement.X > 0 && (rectangle.Right - platforms[j].rectangle.X) / 2 < movement.X)
						{
							position.X = platforms[j].rectangle.X - rectangle.Width;
						}
						else if (movement.X < 0 && (platforms[j].rectangle.Right - rectangle.X) / 2 < Math.Abs(movement.X))
						{
							position.X = platforms[j].rectangle.X + platforms[j].rectangle.Width;
						}

						if (bounceFactor <= 0)
						{
							movement.X = 0;
						}
						else
						{
							movement.X = movement.X * bounceFactor * -1;
						}

						if (movement.Y > 0)
							movement.Y *= friction;
					}
					else if (platforms[j].rectangle.Intersects(new Rectangle(FloorAdv(newPosition.X), FloorAdv(oldPosition.Y), rectangle.Width + 1, rectangle.Height)) && platforms[j].type == Platform.Type.Rectangle)
					{
						if (bounceFactor <= 0)
						{
							movement.X = 0;
						}
						else
						{
							movement.X = movement.X = movement.X * bounceFactor * -1;
						}
					}
					else if (platforms[j].rectangle.Intersects(new Rectangle(FloorAdv(newPosition.X - 1), FloorAdv(oldPosition.Y), rectangle.Width + 1, rectangle.Height)) && platforms[j].type == Platform.Type.Rectangle)
					{
						if (bounceFactor <= 0)
						{
							movement.X = 0;
						}
						else
						{
							movement.X = movement.X = movement.X * bounceFactor * -1;
						}
					}
				}


				//Checks for collision with jump-through rectangles
				for (int j = 0; j < platforms.Count; j++)
				{
					if (newRectangle.Intersects(platforms[j].rectangle) && platforms[j].type == Platform.Type.Grind && grind)
					{
						if (oldRectangle.Bottom - 1 < platforms[j].rectangle.Y && movement.Y > 0 && rectangle.Center.Y < platforms[j].rectangle.Center.Y)
						{
							position.Y = platforms[j].rectangle.Y - rectangle.Height;
							movement.Y = 0;
							onGround = true;
							platformID = j;
						}
					}
				}
			}
			else
			{
				if (movement.Y >= 0)
				{
					for (int i = 0; i < platforms.Count; i++)
					{
						if (new Rectangle((int)(newPosition.X), (int)(newPosition.Y + 1), rectangle.Width, rectangle.Height).Intersects(platforms[i].rectangle) && (platforms[i].type == Platform.Type.Rectangle ||(platforms[i].type == Platform.Type.Grind && grind)))
						{
							onGround = true;
							if (platforms[i].type == Platform.Type.Grind)
							{
								if (!player.grind)
									PerformTrickGrind();
								player.grind = true;
							}
							platformID = i;
							movement.Y = 0;
							position.Y = platforms[i].rectangle.Y - rectangle.Height;
							break;
						}
					}


					for (int i = 0; i < slopes.Count; i++)
					{
						if (slopes[i].faceRight)
						{
							if (Game1.LineIntersectsRect(new Vector2(slopes[i].rectangle.X, slopes[i].rectangle.Y + slopes[i].rectangle.Height), new Vector2(slopes[i].rectangle.X + slopes[i].rectangle.Width, slopes[i].rectangle.Y), new Rectangle((int)position.X + FloorAdv(movement.X), (int)position.Y + 1, rectangle.Width, rectangle.Height)))
							{
								onGround = true;
								movement.Y = 0;
								//platformID = i;
							}
						}
						else
						{
							if (Game1.LineIntersectsRect(new Vector2(slopes[i].rectangle.X, slopes[i].rectangle.Y), new Vector2(slopes[i].rectangle.X + slopes[i].rectangle.Width, slopes[i].rectangle.Y + slopes[i].rectangle.Height), new Rectangle((int)position.X + FloorAdv(movement.X), (int)position.Y + 1, rectangle.Width, rectangle.Height)))
							{
								onGround = true;
								movement.Y = 0;
								//platformID = i;
							}
						}
					}
				}

				for (int i = 0; i < platforms.Count; i++)
				{
					if (platforms[i].type == Platform.Type.Rectangle)
					{
						if (platforms[i].rectangle.Intersects(new Rectangle(FloorAdv(newPosition.X), FloorAdv(oldPosition.Y), rectangle.Width + 1, rectangle.Height)))
						{
							if (bounceFactor <= 0)
							{
								movement.X = 0;
							}
							else
							{
								movement.X = movement.X = movement.X * bounceFactor * -1;
							}
						}
						else if (platforms[i].rectangle.Intersects(new Rectangle(FloorAdv(newPosition.X - 1), FloorAdv(oldPosition.Y), rectangle.Width + 1, rectangle.Height)))
						{
							if (bounceFactor <= 0)
							{
								movement.X = 0;
							}
							else
							{
								movement.X = movement.X = movement.X * bounceFactor * -1;
							}
						}
					}
				}
			}
			if (onSlopePrev && !onSlope)
				movement.Y = -slope.k * movement.X;

			position += movement;
			platformIDPrev = platformPrev;
		}

		public static string FloatToString(float f)
		{
			string text = f.ToString();

			if (text.Contains(","))
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


		/// <summary>
		/// Returns the point in the middle of two vector2 positions
		/// </summary>
		/// <param name="point0"></param>
		/// <param name="point1"></param>
		/// <returns></returns>
		public static Vector2 GetMidpoint(Vector2 point0, Vector2 point1)
		{
			return new Vector2((point0.X + point1.X) / 2, (point0.Y + point1.Y) / 2);
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

		public static Vector2 GetVector2(float angle)
		{
			return new Vector2((float)Math.Cos(angle), -(float)Math.Sin(angle));
		}

		/// <summary>
		/// Returns a resulting vector, pointing from v0 to v1
		/// </summary>
		/// <param name="v0">Source</param>
		/// <param name="v1">Target</param>
		/// <returns></returns>
		public static Vector2 GetVector2(Vector2 v0, Vector2 v1)
		{
			return new Vector2(v1.X - v0.X, v1.Y - v0.Y);
		}

		/// <summary>
		/// Returns a resulting vector, pointing from v0 to v1
		/// </summary>
		/// <param name="v0">Source</param>
		/// <param name="v1">Target</param>
		/// <returns></returns>
		public static Vector2 GetVector2(Point v0, Point v1)
		{
			return new Vector2(v1.X - v0.X, v1.Y - v0.Y);
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

		/// <summary>
		/// AngleBetween - the angle between 2 vectors
		/// </summary>
		/// <returns>
		/// Returns the the angle in degrees between vector1 and vector2
		/// </returns>
		/// <param name="vector1"> The first Vector </param>
		/// <param name="vector2"> The second Vector </param>
		public static float GetAngle(Vector2 a, Vector2 b)
		{
			if (a.X == b.X)
			{
				if (a.Y - b.Y < 0)
					return MathHelper.ToRadians(270);
				else
					return MathHelper.ToRadians(90);
			}
			else if (a.Y == b.Y)
			{
				if (a.X - b.X < 0)
					return MathHelper.ToRadians(0);
				else
					return MathHelper.ToRadians(180);
			}
			else
			{
				float angle = (float)(Math.Atan2(-(b.Y - a.Y), b.X - a.X));

				if (angle < 0)
					angle += (float)Math.PI * 2;

				return angle;
			}
		}

		public static float GetAngle(Vector2 vector)
		{
			return GetAngle(Vector2.Zero, vector);
		}

		public static float GetAngle(Point a, Point b)
		{
			return GetAngle(a.ToVector2(), b.ToVector2());
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