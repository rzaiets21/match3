using Expressive.Expressions;
using System;

namespace Expressive.Functions.Mathematical
{
	internal class CosFunction : FunctionBase
	{
		public override string Name => "Cos";

		public override object Evaluate(IExpression[] parameters, ExpressiveOptions options)
		{
			ValidateParameterCount(parameters, 1, 1);
			return Math.Cos(Convert.ToDouble(parameters[0].Evaluate(base.Variables)));
		}
	}
}
