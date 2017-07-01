using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Skate
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
		public Vector2 Centre
		{
			get
			{
				return position + new Vector2(width / 2, height / 2);
			}
		}
		public float speed = 25f;
		public Vector2 movement;
		public bool onGround, onGroundPrev, onSlope, tryGrind, grind = false;
		public Slope slope;
		public int solidRef, platform, platformPrev;
		public float bounceFactor = 0f;
		public int minSpeed = 8;
		public int maxSpeed = 25;

		public Trick.Grind GrindTrick = null;

		//AnimationHandling
		public PlayerAnimation playerAnimations = new PlayerAnimation();
		public Board board;

		public string animationKey = "";
		public Vector2 textureOffset = Vector2.Zero;
		public Vector2 textureOrigin = Vector2.Zero;
		public Animation animation;
		public float animationSpeedPrev = 0;

		public bool animationFreeze = false;

		public float angle = 0;

		Color color = Color.Black;

		public int jumpCharge;

		public enum State
		{
			Ground,
			Air,
			Trick,
			Grind
		}

		public State state = State.Air;

		public Vector2 ContactPos
		{
			get
			{
				return new Vector2(Rectangle.X + (float)Rectangle.Width / 2, Rectangle.Y + (float)Rectangle.Height);
			}
		}

		public Texture2D texture;

		public Player(string deck, string tape, string simpleBoard)
		{
			board = new Board(deck, tape);
			board.simpleBoard = simpleBoard;
		}

		public void Update()
		{
			if (onGround)
			{
				movement.X = speed;

				if (speed < minSpeed)
					speed = minSpeed;

				if (onSlope)
				{
					angle = -slope.angle;
					speed -= slope.k;
					onGround = true;
				}
				else
					angle = 0;

				if(!grind)
					speed *= 0.99999f;
			}
			else
			{
				movement.Y += Game1.gravity;

				if (Math.Abs(angle) < .02f)
					angle = 0;
				else if (angle < 0)
					angle += 0.001f;
				else
					angle -= 0.001f;
			}

			//Animations
			board.Update(animation.currentFrame);

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

		public void Draw(SpriteBatch spriteBatch, Camera camera)
		{
			animation.Draw(spriteBatch, camera, ContactPos + textureOffset, 1, angle, textureOrigin, color, 1, SpriteEffects.None, 0f);

			board.Draw(ContactPos, spriteBatch, camera, position);
		}

		public void SetAnimation(string key)
		{
			animationKey = key;
			PlayerAnimation value = PlayerAnimation.GetAnimation(key);
			textureOffset = value.textureOffset;
			textureOrigin = value.textureOrigin;
			animation = value.animation;

			board.SetAnimation(key);

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

		internal void SetGround(bool ground)
		{
			this.onGround = ground;
		}
	}
}