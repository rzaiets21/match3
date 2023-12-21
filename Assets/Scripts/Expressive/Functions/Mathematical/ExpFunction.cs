using Expressive.Expressions;
using System;

namespace Expressive.Functions.Mathematical
{
	internal class ExpFunction : FunctionBase
	{
		public override string Name => "Exp";

		public override object Evaluate(IExpression[] parameters, ExpressiveOptions options)
		{
			ValidateParameterCount(parameters, 1, 1);
			return Math.Exp(Convert.ToDouble(parameters[0].Evaluate(base.Variables)));
		}
	}
}
