using Expressive.Expressions;
using System.Collections;

namespace Expressive.Functions.Mathematical
{
	internal class CountFunction : FunctionBase
	{
		public override string Name => "Count";

		public override object Evaluate(IExpression[] parameters, ExpressiveOptions options)
		{
			ValidateParameterCount(parameters, -1, 1);
			int num = 0;
			foreach (IExpression obj in parameters)
			{
				int num2 = 1;
				IEnumerable enumerable = obj.Evaluate(base.Variables) as IEnumerable;
				if (enumerable != null)
				{
					int num3 = 0;
					foreach (object item in enumerable)
					{
						object obj2 = item;
						num3++;
					}
					num2 = num3;
				}
				num += num2;
			}
			return num;
		}
	}
}
