using Expressive.Expressions;
using System;

namespace Expressive.Functions.Mathematical
{
	internal class TruncateFunction : FunctionBase
	{
		public override string Name => "Truncate";

		public override object Evaluate(IExpression[] parameters, ExpressiveOptions options)
		{
			ValidateParameterCount(parameters, 1, 1);
			return Math.Truncate(Convert.ToDouble(parameters[0].Evaluate(base.Variables)));
		}
	}
}
