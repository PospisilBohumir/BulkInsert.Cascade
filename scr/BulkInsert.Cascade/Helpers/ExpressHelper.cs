﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BulkInsert.Cascade.Helpers
{
    /// <summary>
    ///     Helper used for parsing expressions
    /// </summary>
    internal static class ExpressHelper
    {
        /// <summary>
        ///     Creates property value getter from the property name
        /// </summary>
        /// <typeparam name="TObject">Type containing the property</typeparam>
        /// <param name="path">Path to the property</param>
        /// <returns>Property value getter expression</returns>
        internal static Expression<Func<TObject, object>> GetPropGetter<TObject>(string path)
        {
            var paramExpression = Expression.Parameter(typeof(TObject), "value");
            var expressionTree = path.Split('.').Aggregate<string, Expression>(paramExpression, Expression.Property);

            var body = Expression.Convert(expressionTree, typeof(object));
            return Expression.Lambda<Func<TObject, object>>(body, paramExpression);
        }

        internal static Expression<Action<TObject, TProperty>> GetPropSetter<TObject, TProperty>(string propertyName)
        {
            var paramObject = Expression.Parameter(typeof(TObject), "o");
            var paramValue = Expression.Parameter(typeof(TProperty), "value");
            var property = Expression.Property(paramObject, propertyName);
            var assign = Expression.Assign(property, paramValue);
            return Expression.Lambda<Action<TObject, TProperty>>(assign, paramObject, paramValue);
        }


        internal static Expression<Action<TSource, TDestination>> CreateCopy<TSource, TDestination>(string sourceProperty,
            string destinationProperty)
        {
            var source = Expression.Parameter(typeof(TSource), "source");
            var destinationType = typeof(TDestination);
            var destination = Expression.Parameter(destinationType, "destination");
            var destAccess = Expression.MakeMemberAccess(destination, destinationType.GetProperty(destinationProperty)!);
            var body = Expression.Assign(destAccess, Expression.Property(source, sourceProperty));
            return Expression.Lambda<Action<TSource, TDestination>>(body, source, destination);
        }

        public static Expression<Func<TObject, bool>> IsPropertyEmpty<TObject>(string propertyName, Type propertyType)
        {
            var input = Expression.Parameter(typeof(TObject), "o");
            var body = Expression.Equal(Expression.Property(input, propertyName), Expression.Default(propertyType));
            return Expression.Lambda<Func<TObject, bool>>(body, input);
        }

        /// <summary>
        ///     Gets path for expression <paramref name="exp" />
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <typeparam name="TProperty">Property type</typeparam>
        /// <param name="exp">Property expression</param>
        /// <returns>string representation of path to property</returns>
        internal static string GetPath<T, TProperty>(Expression<Func<T, TProperty>> exp)
        {
            return string.Join(".", GetItemsInPath(exp).Reverse());
        }

        private static IEnumerable<string> GetItemsInPath<T, TProperty>(Expression<Func<T, TProperty>> exp)
        {
            if (exp == null)
            {
                yield break;
            }
            var memberExp = FindMemberExpression(exp.Body);
            while (memberExp != null)
            {
                yield return memberExp.Member.Name;
                memberExp = FindMemberExpression(memberExp.Expression);
            }
        }


        private static bool IsConversion(this Expression exp)
        {
            return exp.NodeType == ExpressionType.Convert || exp.NodeType == ExpressionType.ConvertChecked;
        }

        private static MemberExpression FindMemberExpression(this Expression exp)
        {
            if (exp is MemberExpression)
            {
                return (MemberExpression)exp;
            }
            if (IsConversion(exp) && exp is UnaryExpression)
            {
                return ((UnaryExpression)exp).Operand as MemberExpression;
            }
            return null;
        }
    }
}