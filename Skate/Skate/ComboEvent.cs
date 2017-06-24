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
	public class ComboEvent
	{
		public enum Event
		{
			TrickStart,
			TrickEnd,
			Jump,
			Ground,
			Grind
		}
		public Event type;
		public int timeStamp;
		

		public ComboEvent(int timeStamp, Event type)
		{
			this.timeStamp = timeStamp;
			this.type = type;
		}
	}
}