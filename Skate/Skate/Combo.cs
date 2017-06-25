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
	public class Combo
	{
		public List<string> tricks = new List<string>();
		public int score, multiplier, time, lastTrickEnd = 0;
		public int lastScore = 0;
		public string lastQuality = "";

		public float availibleSpeed = 0;

		float landingMultiplierMin = .9f;
		float landingMultiplierMax = 1.2f;
		float queueMultiplierMin = .7f;
		float queueMultiplierMax = 1.2f;

		public int Finish()
		{
			Clear();
			return (int)((score * multiplier) * GetLandingMultiplier());
		}

		public void Update()
		{
			time++;
		}

		protected float GetLandingMultiplier()
		{
			int difference = time - lastTrickEnd;

			if (difference > 20)
				return landingMultiplierMax;
			else
			{
				difference += 10;

				return (int)((difference / 30) * (landingMultiplierMax - landingMultiplierMin) + landingMultiplierMin);
			}
		}

		public float GetTimingMultiplier()
		{
			if (tricks.Count == 0)
				return 1f;
			else if (time - lastTrickEnd < 20 && lastTrickEnd != 0)
			{
				return queueMultiplierMax;
			}
			else
				return 1f;
		}

		public void Clear()
		{
			score = 0;
			multiplier = 0;
			time = 0;
			lastTrickEnd = 0;
			tricks.Clear();
		}
	}
}