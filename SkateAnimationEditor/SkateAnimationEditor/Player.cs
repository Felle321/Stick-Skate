using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SkateAnimationEditor
{
	public class Player
	{
		public Vector2 position;
		public int width = 40;
		public int height = 40;
		public Rectangle Rectangle
		{
			get
			{
				return new Rectangle((int)Math.Round(position.X), (int)Math.Round(position.Y), width, height);
			}
		}

		//AnimationHandling
		public PlayerAnimations playerAnimations = new PlayerAnimations();
		public Board board;

		public string animationKey = "";
		public Vector2 textureOffset = Vector2.Zero;
		public Vector2 wheelOffset = Vector2.Zero;
		public Vector2 textureOrigin = Vector2.Zero;
		public Vector2 wheelOrigin = Vector2.Zero;
		public Animation animation, animationWheels;
		public float animationSpeedPrev = 0;
		
		public bool animationFreeze = false;

		public float angle = 0;

		Color color = Color.Black;
		Color wheelColor = Color.White;

		public enum State
		{
			Ground,
			Air,
			Trick,
			Grind
		}

		public State state = State.Air;
		
		public Player(string deck, string tape, string simpleBoard)
		{
			board = new Board(deck, tape);
			board.simpleBoard = simpleBoard;
		}

		public void Update()
		{
			board.Update((int)Math.Floor(animation.currentFrame));
			
			if (board.animationFreeze && !animationFreeze)
			{
				animationSpeedPrev = animation.speed;
				animation.speed = 0;
			}
			else if(!board.animationFreeze && animationFreeze)
			{
				animation.speed = animationSpeedPrev;
			}
		}

		public void Draw(SpriteBatch spriteBatch, Camera camera)
		{
			animation.Draw(spriteBatch, camera, position + textureOffset, 1, angle, textureOrigin, color, 1, SpriteEffects.None, 0f);
			
			//if(!board.drawSimpleBoard)
				//animationWheels.Draw(spriteBatch, camera, position + textureOffset, 1, angle, wheelOrigin, wheelColor, 1, SpriteEffects.None, 0f);
			
			spriteBatch.Draw(Game1.pixel, position + textureOffset + textureOrigin, null, Color.Red, 0, Vector2.Zero, 4, SpriteEffects.None, 0f);
			//spriteBatch.Draw(Game1.pixel, position + wheelOffset + wheelOrigin, null, Color.Green, 0, Vector2.Zero, 4, SpriteEffects.None, 0f);
		}

		public void DrawBoard(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Camera camera)
		{
			board.Draw(spriteBatch, graphicsDevice, camera, position, wheelColor);
		}

		public void DrawHUD(SpriteBatch spriteBatch)
		{
			spriteBatch.DrawString(Game1.fontDebug, "Speed: " + animation.speed, new Vector2(20, 50), Color.Black);
			spriteBatch.DrawString(Game1.fontDebug, "Angle: " + angle, new Vector2(20, 100), Color.Black);
			spriteBatch.DrawString(Game1.fontDebug, "Frame: " + Math.Floor(animation.currentFrame).ToString(), new Vector2(20, 200), Color.Black);

			board.DrawHUD(spriteBatch);
		}

		public void SetAnimation(string key)
		{

			animationKey = key;
			PlayerAnimations.ReturnValue value = playerAnimations.GetAnimation(key);
			textureOffset = value.textureOffset;
			textureOrigin = value.textureOrigin;
			wheelOffset = value.wheelsOffset;
			wheelOrigin = value.wheelsOrigin;
			animation = value.animation;
			animationWheels = value.animationWheels;

			board.SetAnimation(key, (int)Math.Floor(animation.framesTotal), animation.speed);
			
			if (board.animationFreeze && !animationFreeze)
			{
				animationSpeedPrev = animation.speed;
				animation.speed = 0;
			}
			else if (!board.animationFreeze && animationFreeze)
			{
				animation.speed = animationSpeedPrev;
			}
		}
	}
}