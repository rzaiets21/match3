using System;

namespace GGMatch3
{
	[Serializable]
	public struct IntensityChange
	{
		public float delay;

		public FloatRange intensityRange;

		public GGMath.CssCubicBezier easeCurve;

		public float duration;

		public IntensityChange IntensityRange(float min, float max)
		{
			intensityRange.min = min;
			intensityRange.max = max;
			return this;
		}

		public float Intensity(float t)
		{
			return intensityRange.Lerp(easeCurve.Eval(t));
		}

		public IntensityChange Duration(float duration)
		{
			this.duration = duration;
			return this;
		}

		public IntensityChange Delay(float delay)
		{
			this.delay = delay;
			return this;
		}

		public IntensityChange EaseCurve(GGMath.CssCubicBezier easeCurve)
		{
			this.easeCurve = easeCurve;
			return this;
		}
	}
}
