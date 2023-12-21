using Expressive.Expressions;
using System;

namespace Expressive.Functions.Mathematical
{
	internal class IEEERemainderFunction : FunctionBase
	{
		public override string Name => "IEEERemainder";

		public override object Evaluate(IExpression[] parameters, ExpressiveOptions options)
		{
			ValidateParameterCount(parameters, 2, 2);
			return Math.IEEERemainder(Convert.ToDouble(parameters[0].Evaluate(base.Variables)), Convert.ToDouble(parameters[1].Evaluate(base.Variables)));
		}
	}
}
