using Expressive.Expressions;
using System;

namespace Expressive.Functions.Mathematical
{
	internal class TanFunction : FunctionBase
	{
		public override string Name => "Tan";

		public override object Evaluate(IExpression[] parameters, ExpressiveOptions options)
		{
			ValidateParameterCount(parameters, 1, 1);
			return Math.Tan(Convert.ToDouble(parameters[0].Evaluate(base.Variables)));
		}
	}
}
