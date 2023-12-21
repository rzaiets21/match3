using Expressive.Expressions;
using System;

namespace Expressive.Functions.Mathematical
{
	internal class AtanFunction : FunctionBase
	{
		public override string Name => "Atan";

		public override object Evaluate(IExpression[] parameters, ExpressiveOptions options)
		{
			ValidateParameterCount(parameters, 1, 1);
			return Math.Atan(Convert.ToDouble(parameters[0].Evaluate(base.Variables)));
		}
	}
}
