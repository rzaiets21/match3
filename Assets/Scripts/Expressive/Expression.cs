using Expressive.Exceptions;
using Expressive.Expressions;
using Expressive.Functions;
using System;
using System.Collections.Generic;

namespace Expressive
{
	public sealed class Expression
	{
		private IExpression _compiledExpression;

		private readonly ExpressiveOptions _options;

		private readonly string _originalExpression;

		private readonly ExpressionParser _parser;

		private string[] _variables;

		public Expression(string expression)
			: this(expression, ExpressiveOptions.None)
		{
		}

		public Expression(string expression, ExpressiveOptions options)
		{
			_originalExpression = expression;
			_options = options;
			_parser = new ExpressionParser(_options);
		}

		public object Evaluate()
		{
			return Evaluate(null);
		}

		public object Evaluate(IDictionary<string, object> variables)
		{
			try
			{
				CompileExpression();
				if (variables != null && _options.HasFlag(ExpressiveOptions.IgnoreCase))
				{
					variables = new Dictionary<string, object>(variables, StringComparer.OrdinalIgnoreCase);
				}
				return _compiledExpression?.Evaluate(variables);
			}
			catch (Exception innerException)
			{
				throw new ExpressiveException(innerException);
			}
		}

		public void RegisterFunction(IFunction function)
		{
			_parser.RegisterFunction(function);
		}

		private void CompileExpression()
		{
			if (_compiledExpression == null || _options.HasFlag(ExpressiveOptions.NoCache))
			{
				List<string> list = new List<string>();
				_compiledExpression = _parser.CompileExpression(_originalExpression, list);
				_variables = list.ToArray();
			}
		}
	}
}
