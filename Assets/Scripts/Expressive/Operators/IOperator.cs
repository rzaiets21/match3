using Expressive.Expressions;
using System.Collections.Generic;

namespace Expressive.Operators
{
	internal interface IOperator
	{
		string[] Tags
		{
			get;
		}

		IExpression BuildExpression(Token previousToken, IExpression[] expressions, ExpressiveOptions options);

		bool CanGetCaptiveTokens(Token previousToken, Token token, Queue<Token> remainingTokens);

		Token[] GetCaptiveTokens(Token previousToken, Token token, Queue<Token> remainingTokens);

		Token[] GetInnerCaptiveTokens(Token[] allCaptiveTokens);

		OperatorPrecedence GetPrecedence(Token previousToken);
	}
}
