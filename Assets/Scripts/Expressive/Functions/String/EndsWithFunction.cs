using Expressive.Expressions;
using System;

namespace Expressive.Functions.String
{
	internal class EndsWithFunction : FunctionBase
	{
		public override string Name => "EndsWith";

		public override object Evaluate(IExpression[] parameters, ExpressiveOptions options)
		{
			ValidateParameterCount(parameters, 2, 2);
			string text = (string)parameters[0].Evaluate(base.Variables);
			string text2 = (string)parameters[1].Evaluate(base.Variables);
			if (text2 == null)
			{
				return false;
			}
			return text?.EndsWith(text2, options.HasFlag(ExpressiveOptions.IgnoreCase) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) ?? false;
		}
	}
}
