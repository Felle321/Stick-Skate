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
		int score;
		List<ComboEvent> events = new List<ComboEvent>();
	}
}