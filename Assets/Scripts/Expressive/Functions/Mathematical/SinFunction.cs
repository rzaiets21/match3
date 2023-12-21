using Expressive.Expressions;
using System;

namespace Expressive.Functions.Mathematical
{
	internal class SinFunction : FunctionBase
	{
		public override string Name => "Sin";

		public override object Evaluate(IExpression[] parameters, ExpressiveOptions options)
		{
			ValidateParameterCount(parameters, 1, 1);
			return Math.Sin(Convert.ToDouble(parameters[0].Evaluate(base.Variables)));
		}
	}
}
