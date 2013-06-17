using System;
using System.Linq;

using Android.Media;

namespace AndroSynth
{
	public class SineWaveGenerator
	{
		const float PI2 = (float)(2 * Math.PI);
		static readonly float Step = PI2 / AudioTrack.GetNativeOutputSampleRate (Stream.Music);

		float[] coefficient = new float[4];
		float currentAngle;

		public float CarrierFrequency {
			get;
			set;
		}

		public void SetFingerCoefficient (int finger, float coeff)
		{
			coefficient [finger] = coeff;
		}

		float Period {
			get {
				return 24 * (float)Math.Pow (CarrierFrequency, 5);
			}
		}

		public void FillBuffer (short[] buffer)
		{
			int i = 0;
			for (; i < buffer.Length; ++i) {
				currentAngle = (currentAngle + Step) % short.MaxValue;
				var preMul = CarrierFrequency * currentAngle;
				var value = (coefficient[0] * (float)Math.Sin (preMul + (float)Math.Sin (preMul * coefficient[1]))
				             + (float)Math.Sin (preMul * (coefficient[2] + 1))
				             + Math.Abs ((float)Math.Sin (preMul * (coefficient[3] + 2)))) / 4;
				buffer [i] = (short)(value * (short.MaxValue / 2));
			}
		}
	}
}

