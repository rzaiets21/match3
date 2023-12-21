using Expressive.Expressions;
using Expressive.Helpers;
using System.Collections;
using System.Linq;

namespace Expressive.Functions.Mathematical
{
	internal class MaxFunction : FunctionBase
	{
		public override string Name => "Max";

		public override object Evaluate(IExpression[] parameters, ExpressiveOptions options)
		{
			ValidateParameterCount(parameters, -1, 1);
			object obj = parameters[0].Evaluate(base.Variables);
			if (obj is IEnumerable)
			{
				obj = Max((IEnumerable)obj);
			}
			if (obj == null)
			{
				return null;
			}
			foreach (IExpression item in Enumerable.Skip(parameters, 1))
			{
				object obj2 = item.Evaluate(base.Variables);
				IEnumerable enumerable = obj2 as IEnumerable;
				if (enumerable != null)
				{
					obj2 = Max(enumerable);
				}
				obj = Numbers.Max(obj, obj2);
				if (obj == null)
				{
					return null;
				}
			}
			return obj;
		}

		private object Max(IEnumerable enumerable)
		{
			object obj = null;
			foreach (object item in enumerable)
			{
				if (item == null)
				{
					return null;
				}
				obj = ((obj != null) ? Numbers.Max(obj, item) : item);
			}
			return obj;
		}
	}
}
