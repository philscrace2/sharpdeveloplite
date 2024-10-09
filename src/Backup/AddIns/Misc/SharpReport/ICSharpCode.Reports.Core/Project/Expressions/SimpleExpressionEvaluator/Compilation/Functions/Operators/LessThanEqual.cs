using System;
using System.Collections;
using SimpleExpressionEvaluator.Compilation;

namespace SimpleExpressionEvaluator.Compilation.Functions.Operators
{
    /// <summary>
    /// Summary description for LessThanEqual.
    /// </summary>
    [Tokens("<=")]
    public class LessThanEqual:BinaryOperator<bool>
    {
        protected override bool EvaluateOperation(object leftVal, object rightVal)
        {
            if (leftVal == null && rightVal == null)
                return true;

            if (leftVal == null)
                return true;

            if (rightVal == null)
                return false;

            if (leftVal is string || rightVal is string)
                return StringComparer.CurrentCultureIgnoreCase.Compare(leftVal, rightVal) <= 0;

            try
            {
                return Comparer.Default.Compare(leftVal, rightVal) <= 0;
            }
            catch
            {
                return false;
            }
        }

    }
}