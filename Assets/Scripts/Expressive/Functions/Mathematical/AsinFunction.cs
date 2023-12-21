using Expressive.Expressions;
using System;

namespace Expressive.Functions.Mathematical
{
	internal class AsinFunction : FunctionBase
	{
		public override string Name => "Asin";

		public override object Evaluate(IExpression[] parameters, ExpressiveOptions options)
		{
			ValidateParameterCount(parameters, 1, 1);
			return Math.Asin(Convert.ToDouble(parameters[0].Evaluate(base.Variables)));
		}
	}
}
