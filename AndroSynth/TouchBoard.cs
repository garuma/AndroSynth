using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Graphics;

using Log = Android.Util.Log;

namespace AndroSynth
{
	struct PointF
	{
		public float X;
		public float Y;
	}

	public class TouchBoard : View
	{
		Color bgColor;
		Paint[] circlePaints;
		PointF?[] pointers;

		SineWaveGenerator generator = new SineWaveGenerator ();
		AsyncPlayer player;
		bool played;

		public TouchBoard (Context context) :
			base (context)
		{
			Initialize ();
		}

		public TouchBoard (Context context, IAttributeSet attrs) :
			base (context, attrs)
		{
			Initialize ();
		}

		public TouchBoard (Context context, IAttributeSet attrs, int defStyle) :
			base (context, attrs, defStyle)
		{
			Initialize ();
		}

		void Initialize ()
		{
			bgColor = Color.Rgb (0, 0x99, 0xcc);
			var colors = new Color[] {
				Color.Rgb (0xaa, 0x66, 0xcc),
				Color.Rgb (0x99, 0xcc, 0x00),
				Color.Rgb (0xff, 0xbb, 0x33),
				Color.Rgb (0xff, 0x44, 0x44)
			};
			circlePaints = colors.Select (c => new Paint () { AntiAlias = true, Color = c }).ToArray ();
			pointers = new PointF?[circlePaints.Length];

			player = new AsyncPlayer (generator);
		}

		public override bool OnTouchEvent (MotionEvent e)
		{
			Log.Info ("Motion", "{0} // {1} // {2} // {3}", e.ActionMasked.ToString (), e.Action.ToString (), e.ActionIndex, e.PointerCount);
			if (e.Action == MotionEventActions.Down || (int)e.ActionMasked == (int)MotionEventActions.PointerDown) {
				var ind = e.Action == MotionEventActions.Down ? 0 : e.ActionIndex;
				pointers [ind] = new PointF { X = e.GetX (ind), Y = e.GetY (ind) };
			} else if (e.Action == MotionEventActions.Up
			           || (int)e.ActionMasked == (int)MotionEventActions.PointerUp) {
				if (e.Action == MotionEventActions.Up)
					for (int i = 0; i < pointers.Length; ++i)
						pointers [i] = null;
				else {
					var id = e.GetPointerId (e.ActionIndex);
					pointers [id] = null;
					generator.SetFingerCoefficient (id, 0);
				}
			} else if (e.Action == MotionEventActions.Move) {
				for (int i = 0; i < e.PointerCount; ++i) {
					var pointerIndex = e.GetPointerId (i);
					pointers[pointerIndex] = new PointF { X = e.GetX (i), Y = e.GetY (i) };
				}
			}

			generator.CarrierFrequency = (e.GetY () / Height) * (600 - 80) + 80;
			for (int i = 0; i < pointers.Length; i++) {
				var amp = pointers [i] == null ? 0 : 6 * (pointers [i].Value.X / Width);
				generator.SetFingerCoefficient (i, amp);
				Log.Info ("NewAmp", "{0} -> {1}", i, amp);
			}

			if (!played) {
				player.Play ();
				played = true;
			}

			Invalidate ();
			return true;
		}

		protected override void OnDraw (Canvas canvas)
		{
			canvas.DrawColor (bgColor);

			for (int i = 0; i < pointers.Length; ++i) {
				if (pointers [i] == null)
					continue;
				var p = pointers [i].Value;
				var c = circlePaints [i];
				canvas.DrawCircle (p.X, p.Y, 80, c);
			}
		}
	}
}

