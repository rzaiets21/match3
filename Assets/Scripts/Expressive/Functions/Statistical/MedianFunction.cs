using Expressive.Expressions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Expressive.Functions.Statistical
{
	internal class MedianFunction : FunctionBase
	{
		[Serializable]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Func<decimal, decimal> _003C_003E9__3_0;

			internal decimal _003CMedian_003Eb__3_0(decimal x)
			{
				return x;
			}
		}

		public override string Name => "Median";

		public override object Evaluate(IExpression[] parameters, ExpressiveOptions options)
		{
			ValidateParameterCount(parameters, -1, 1);
			IList<decimal> list = new List<decimal>();
			for (int i = 0; i < parameters.Length; i++)
			{
				object obj = parameters[i].Evaluate(base.Variables);
				IEnumerable enumerable = obj as IEnumerable;
				if (enumerable != null)
				{
					foreach (object item in enumerable)
					{
						list.Add(Convert.ToDecimal(item));
					}
				}
				else
				{
					list.Add(Convert.ToDecimal(obj));
				}
			}
			return Median(Enumerable.ToArray(list));
		}

		private decimal Median(decimal[] xs)
		{
			List<decimal> list = Enumerable.ToList(Enumerable.OrderBy(xs, _003C_003Ec._003C_003E9._003CMedian_003Eb__3_0));
			double num = (double)(list.Count - 1) / 2.0;
			return (list[(int)num] + list[(int)(num + 0.5)]) / 2m;
		}
	}
}
