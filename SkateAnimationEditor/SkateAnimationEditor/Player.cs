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
		public Color color = Color.Black;

		//AnimationHandling
		public PlayerAnimations playerAnimations = new PlayerAnimations();
		public Board board;

		string animationKey = "";
		Vector2 textureOffset = Vector2.Zero;
		Vector2 wheelOffset = Vector2.Zero;
		Animation animation, animationWheels;
		float animationSpeedPrev = 0;

		Vector2 simpleBoardOffset = new Vector2(-10, 20);
		bool drawSimpleBoard, animationFreeze = false;
		string simpleBoard = "";

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
			this.simpleBoard = simpleBoard;
		}

		public void Update()
		{
			board.Update((int)Math.Floor(animation.currentFrame));

			drawSimpleBoard = board.simpleBoard;
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
			animation.Draw(spriteBatch, camera, position + textureOffset, SpriteEffects.None, 0f);

			if (drawSimpleBoard)
				SpriteHandler.Draw(simpleBoard, spriteBatch, camera, position + simpleBoardOffset, SpriteEffects.None, 0f);
			else
				animationWheels.Draw(spriteBatch, camera, position + textureOffset, SpriteEffects.None, 0f);
		}

		public void DrawBoard(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Camera camera)
		{
			if(!drawSimpleBoard)
				board.Draw(spriteBatch, graphicsDevice, camera, position);
		}

		public void SetAnimation(string key)
		{
			board.SetAnimation(key);

			animationKey = key;
			PlayerAnimations.ReturnValue value = playerAnimations.GetAnimation(key);
			textureOffset = value.textureOffset;
			wheelOffset = value.wheelsOffset;
			animation = value.animation;
			animationWheels = value.animationWheels;
			drawSimpleBoard = board.simpleBoard;
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