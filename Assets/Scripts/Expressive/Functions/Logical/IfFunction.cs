using Expressive.Expressions;
using System;

namespace Expressive.Functions.Logical
{
	internal class IfFunction : FunctionBase
	{
		public override string Name => "If";

		public override object Evaluate(IExpression[] parameters, ExpressiveOptions options)
		{
			ValidateParameterCount(parameters, 3, 3);
			if (!Convert.ToBoolean(parameters[0].Evaluate(base.Variables)))
			{
				return parameters[2].Evaluate(base.Variables);
			}
			return parameters[1].Evaluate(base.Variables);
		}
	}
}
