using Expressive.Exceptions;
using Expressive.Expressions;
using System.Collections.Generic;
using System.Linq;

namespace Expressive.Functions
{
	internal abstract class FunctionBase : IFunction
	{
		private IDictionary<string, object> _003CVariables_003Ek__BackingField;

		public IDictionary<string, object> Variables
		{
			get
			{
				return _003CVariables_003Ek__BackingField;
			}
			set
			{
				_003CVariables_003Ek__BackingField = value;
			}
		}

		public abstract string Name
		{
			get;
		}

		public abstract object Evaluate(IExpression[] parameters, ExpressiveOptions options);

		protected bool ValidateParameterCount(IExpression[] parameters, int expectedCount, int minimumCount)
		{
			if (expectedCount != -1 && (parameters == null || !Enumerable.Any(parameters) || parameters.Length != expectedCount))
			{
				throw new ParameterCountMismatchException(Name + "() takes only " + expectedCount + " argument(s)");
			}
			if (minimumCount > 0 && (parameters == null || !Enumerable.Any(parameters) || parameters.Length < minimumCount))
			{
				throw new ParameterCountMismatchException(Name + "() expects at least " + minimumCount + " argument(s)");
			}
			return true;
		}
	}
}
