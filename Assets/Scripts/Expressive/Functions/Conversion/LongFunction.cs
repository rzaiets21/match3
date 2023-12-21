using Expressive.Expressions;
using System;

namespace Expressive.Functions.Conversion
{
	internal sealed class LongFunction : FunctionBase
	{
		public override string Name => "Long";

		public override object Evaluate(IExpression[] parameters, ExpressiveOptions options)
		{
			ValidateParameterCount(parameters, 1, 1);
			object obj = parameters[0].Evaluate(base.Variables);
			if (obj == null)
			{
				return null;
			}
			return Convert.ToInt64(obj);
		}
	}
}
