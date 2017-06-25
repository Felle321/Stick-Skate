using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Skate
{
	public abstract class Trick
	{
		public enum FlipTricks
		{
			Kickflip
		}
		public enum GrabTricks
		{
			Cannonball
		}
		public enum GrindTricks
		{
			NoseGrind
		}
		public enum Quality
		{
			Weak,
			OK,
			Good,
			Great,
			Perfect,
		}

		public Quality quality;
		public int frame, totalFrames, score;
		public float queueTimingMultiplier = 1f;
		float queueMultiplierMin = .7f;
		float queueMultiplierMax = 1f;


		public Trick(float queueTimingMultiplier)
		{
			frame = 0;
			this.queueTimingMultiplier = queueTimingMultiplier;

			float quality = (queueTimingMultiplier - queueMultiplierMin) / (queueMultiplierMax - queueMultiplierMin);

			if (quality < .25f)
				this.quality = Quality.Weak;
			else if (quality < .5f)
				this.quality = Quality.OK;
			else if (quality < .75f)
				this.quality = Quality.Good;
			else if (quality <= 1)
				this.quality = Quality.Great;
			else if (quality > 1)
				this.quality = Quality.Perfect;
		}

		public virtual void Update()
		{
			frame++;
		}

		public virtual int GetScore()
		{
			return (int)(score * queueTimingMultiplier);
		}

		public virtual string GetName()
		{
			return "";
		}

		public virtual float GetTimingMultiplier()
		{
			return queueMultiplierMin + (frame / totalFrames) * (queueMultiplierMax - queueMultiplierMin);
		}

		public virtual float GetSpeed()
		{
			return 1 * queueTimingMultiplier;
		}

		public class Flip : Trick
		{
			FlipTricks trick;

			public Flip(FlipTricks trick, float queueTimingMultiplier) : base(queueTimingMultiplier)
			{
				this.trick = trick;

				switch (trick)
				{
					case FlipTricks.Kickflip:
						totalFrames = 15;
						score = 150;
						break;
					default:
						break;
				}
			}

			public override void Update()
			{
				base.Update();
			}

			public override string GetName()
			{
				return trick.ToString();
			}
		}

		public class Grab : Trick
		{
			GrabTricks trick;

			public Grab(GrabTricks trick, float queueTimingMultiplier) : base(queueTimingMultiplier)
			{
				this.trick = trick;
			}

			public override void Update()
			{
				base.Update();
			}

			public override string GetName()
			{
				return trick.ToString();
			}
		}

		public class Grind : Trick
		{
			GrindTricks trick;

			public Grind(GrindTricks trick, float queueTimingMultiplier) : base(queueTimingMultiplier)
			{
				this.trick = trick;
			}

			public override void Update()
			{
				base.Update();
			}

			public override string GetName()
			{
				return trick.ToString();
			}
		}
	}
}