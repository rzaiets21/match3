using Expressive.Expressions;
using System;

namespace Expressive.Functions.Mathematical
{
	internal class SqrtFunction : FunctionBase
	{
		public override string Name => "Sqrt";

		public override object Evaluate(IExpression[] parameters, ExpressiveOptions options)
		{
			ValidateParameterCount(parameters, 1, 1);
			return Math.Sqrt(Convert.ToDouble(parameters[0].Evaluate(base.Variables)));
		}
	}
}
