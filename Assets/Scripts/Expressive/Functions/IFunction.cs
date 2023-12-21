using Expressive.Expressions;
using System.Collections.Generic;

namespace Expressive.Functions
{
	public interface IFunction
	{
		IDictionary<string, object> Variables
		{
			set;
		}

		string Name
		{
			get;
		}

		object Evaluate(IExpression[] parameters, ExpressiveOptions options);
	}
}
