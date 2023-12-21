using Expressive.Expressions;
using System;
using System.Globalization;

namespace Expressive.Functions.Conversion
{
	internal sealed class DateFunction : FunctionBase
	{
		public override string Name => "Date";

		public override object Evaluate(IExpression[] parameters, ExpressiveOptions options)
		{
			ValidateParameterCount(parameters, -1, 1);
			object obj = parameters[0].Evaluate(base.Variables);
			if (obj == null)
			{
				return null;
			}
			string format;
			string s;
			if (parameters.Length > 1 && (s = (obj as string)) != null && (format = (parameters[1].Evaluate(base.Variables) as string)) != null)
			{
				return DateTime.ParseExact(s, format, CultureInfo.CurrentCulture);
			}
			return Convert.ToDateTime(obj);
		}
	}
}
