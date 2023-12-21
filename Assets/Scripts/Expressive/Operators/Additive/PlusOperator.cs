using Expressive.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressive.Operators.Additive
{
	internal class PlusOperator : OperatorBase
	{
		public override string[] Tags => new string[1]
		{
			"+"
		};

		public override IExpression BuildExpression(Token previousToken, IExpression[] expressions, ExpressiveOptions options)
		{
			if (IsUnary(previousToken))
			{
				return new UnaryExpression(UnaryExpressionType.Plus, expressions[0] ?? expressions[1]);
			}
			return new BinaryExpression(BinaryExpressionType.Add, expressions[0], expressions[1], options);
		}

		public override bool CanGetCaptiveTokens(Token previousToken, Token token, Queue<Token> remainingTokens)
		{
			Queue<Token> remainingTokens2 = new Queue<Token>(remainingTokens.ToArray());
			return Enumerable.Any(GetCaptiveTokens(previousToken, token, remainingTokens2));
		}

		public override Token[] GetInnerCaptiveTokens(Token[] allCaptiveTokens)
		{
			return Enumerable.ToArray(Enumerable.Skip(allCaptiveTokens, 1));
		}

		public override OperatorPrecedence GetPrecedence(Token previousToken)
		{
			if (IsUnary(previousToken))
			{
				return OperatorPrecedence.UnaryPlus;
			}
			return OperatorPrecedence.Add;
		}

		private bool IsUnary(Token previousToken)
		{
			if (!string.IsNullOrEmpty(previousToken?.CurrentToken) && !string.Equals(previousToken.CurrentToken, "(", StringComparison.Ordinal))
			{
				return ExtensionMethods.IsArithmeticOperator(previousToken.CurrentToken);
			}
			return true;
		}
	}
}
