﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BigQuery.Linq
{
    public static partial class BqFunc
    {
        // https://developers.google.com/bigquery/query-reference#castingfunctions
        // =, !=, >, <, >=, <=, IS NULL, IFNULL is in BigQueryTranslateVisitor.VisitBinary

        /// <summary>
        /// Returns true if the value of expr1 is greater than or equal to expr2, and less than or equal to expr3.
        /// </summary>
        [FunctionName("BETWEEN", SpecifiedFormatterType = typeof(BetweenFormatter))]
        public static bool Between(object expr1, object expr2, object expr3) { throw Invalid(); }

        /// <summary>
        /// <para>Returns true if expr matches expr1, expr2, or any value in the parentheses.</para>
        /// <para>The IN keyword is an efficient shorthand for (expr = expr1 || expr = expr2 || ...).</para>
        /// <para>The expressions used with the IN keyword must be constants and they must match the data type of expr</para>
        /// </summary>
        [FunctionName("IN", SpecifiedFormatterType = typeof(InFormatter))]
        public static bool In(object expr, params object[] exprs) { throw Invalid(); }

        /// <summary>
        /// Returns the largest numeric_expr parameter. All parameters must be numeric, and all parameters must be the same type. If any parameter is NULL, this function returns NULL.
        /// </summary>
        [FunctionName("GREATEST", SpecifiedFormatterType = typeof(GreatestFormatter))]
        public static long Greatest(params long[] exprs)
        {
            throw Invalid();
        }

        /// <summary>
        /// Returns the largest numeric_expr parameter. All parameters must be numeric, and all parameters must be the same type. If any parameter is NULL, this function returns NULL.
        /// </summary>
        [FunctionName("GREATEST", SpecifiedFormatterType = typeof(GreatestFormatter))]
        public static double Greatest(params double[] exprs)
        {
            throw Invalid();
        }

        /// <summary>Returns true if numeric_expr is positive or negative infinity.</summary>
        [FunctionName("IS_INF")]
        public static bool IsInfinity(double numericExpr) { throw Invalid(); }

        /// <summary>Returns true if numeric_expr is the special NaN numeric value.</summary>
        [FunctionName("IS_NAN")]
        public static bool IsNAN(double numericExpr) { throw Invalid(); }

        // IS_EXPLICITLY_DEFINED is Deprecated, expr IS NOT NULL instead.

        /// <summary>
        /// Returns the smallest numeric_expr parameter. All parameters must be numeric, and all parameters must be the same type. If any parameter is NULL, this function returns NULL.
        /// </summary>
        [FunctionName("LEAST", SpecifiedFormatterType = typeof(LeastFormatter))]
        public static long Least(params long[] exprs)
        {
            throw Invalid();
        }

        /// <summary>
        /// Returns the smallest numeric_expr parameter. All parameters must be numeric, and all parameters must be the same type. If any parameter is NULL, this function returns NULL.
        /// </summary>
        [FunctionName("LEAST", SpecifiedFormatterType = typeof(LeastFormatter))]
        public static double Least(params double[] exprs)
        {
            throw Invalid();
        }

        class BetweenFormatter : ISpeficiedFormatter
        {
            public string Format(System.Linq.Expressions.MethodCallExpression node)
            {
                var innerTranslator = new BigQueryTranslateVisitor();
                var expr1 = innerTranslator.VisitAndClearBuffer(node.Arguments[0]);
                var expr2 = innerTranslator.VisitAndClearBuffer(node.Arguments[1]);
                var expr3 = innerTranslator.VisitAndClearBuffer(node.Arguments[2]);

                return string.Format("{0} BETWEEN {1} AND {2}", expr1, expr2, expr3);
            }
        }

        class InFormatter : ISpeficiedFormatter
        {
            public string Format(System.Linq.Expressions.MethodCallExpression node)
            {
                var innerTranslator = new BigQueryTranslateVisitor();
                var expr1 = innerTranslator.VisitAndClearBuffer(node.Arguments[0]);
                var arg = node.Arguments[1] as NewArrayExpression;
                var expr2 = string.Join(", ", arg.Expressions.Select(x => innerTranslator.VisitAndClearBuffer(x)));

                return string.Format("{0} IN({1})", expr1, expr2);
            }
        }

        class GreatestFormatter : ISpeficiedFormatter
        {
            public string Format(System.Linq.Expressions.MethodCallExpression node)
            {
                var innerTranslator = new BigQueryTranslateVisitor();
                var arg = node.Arguments[0] as NewArrayExpression;
                var expr = string.Join(", ", arg.Expressions.Select(x => innerTranslator.VisitAndClearBuffer(x)));

                return string.Format("GREATEST({0})", expr);
            }
        }

        class LeastFormatter : ISpeficiedFormatter
        {
            public string Format(System.Linq.Expressions.MethodCallExpression node)
            {
                var innerTranslator = new BigQueryTranslateVisitor();
                var arg = node.Arguments[0] as NewArrayExpression;
                var expr = string.Join(", ", arg.Expressions.Select(x => innerTranslator.VisitAndClearBuffer(x)));

                return string.Format("LEAST({0})", expr);
            }
        }
    }
}