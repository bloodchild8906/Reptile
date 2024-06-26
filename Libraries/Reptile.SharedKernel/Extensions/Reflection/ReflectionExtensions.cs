﻿using System.Linq.Expressions;
using System.Reflection;

namespace Reptile.SharedKernel.Extensions.Reflection;

public static class ReflectionExtensions
{
    public static PropertyInfo GetProperty<TX, TY>(this TX obj, Expression<Func<TX, TY>> selector)
    {
        Expression body = selector;
        if (body is LambdaExpression) body = ((LambdaExpression)body).Body;
        switch (body.NodeType)
        {
            case ExpressionType.MemberAccess:
                return (PropertyInfo)((MemberExpression)body).Member;
            default:
                throw new InvalidOperationException();
        }
    }

	public static T? GetAttribute<T>(this PropertyInfo property) where T : Attribute => property.GetCustomAttributes(typeof(T), false).FirstOrDefault() as T;

	public static bool IsSystem(this PropertyInfo property) => property.PropertyType.Namespace?.StartsWith("System") == true;

	public static string GetMemberName<T>(this T instance, Expression<Func<T, object>> expression) => StaticReflection.GetMemberName(expression);

	public static string GetMemberName<T>(this T instance, Expression<Action<T>> expression) => StaticReflection.GetMemberName(expression);

	public static bool PublicInstancePropertiesEqual<T>(this T? self, T? to, params string[] ignore) where T : class
    {
        if (self != null && to != null)
        {
            var type = typeof(T);
            var ignoreList = new List<string>(ignore);
            var unequalProperties =
                from pi in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                where !ignoreList.Contains(pi.Name)
                let selfValue = type.GetProperty(pi.Name).GetValue(self, null)
                let toValue = type.GetProperty(pi.Name).GetValue(to, null)
                where selfValue != toValue && (selfValue == null || !selfValue.Equals(toValue))
                select selfValue;
            return !unequalProperties.Any();
        }

        return self == to;
    }
}

public static class StaticReflection
{
	public static string GetMemberName<T>(
		this T instance,
		Expression<Func<T, object>> expression) => GetMemberName(expression);

	public static string GetMemberName<T>(
        Expression<Func<T, object>> expression)
    {
        if (expression == null)
            throw new ArgumentException(
                "The expression cannot be null.");

        return GetMemberName(expression.Body);
    }

	public static string GetMemberName<T>(
		this T instance,
		Expression<Action<T>> expression) => GetMemberName(expression);

	public static string GetMemberName<T>(
        Expression<Action<T>> expression)
    {
        if (expression == null)
            throw new ArgumentException(
                "The expression cannot be null.");

        return GetMemberName(expression.Body);
    }

    private static string GetMemberName(
        Expression expression)
    {
        if (expression == null)
            throw new ArgumentException(
                "The expression cannot be null.");

        if (expression is MemberExpression)
        {
            // Reference type property or field
            var memberExpression =
                (MemberExpression)expression;
            return memberExpression.Member.Name;
        }

        if (expression is MethodCallExpression)
        {
            // Reference type method
            var methodCallExpression =
                (MethodCallExpression)expression;
            return methodCallExpression.Method.Name;
        }

        if (expression is UnaryExpression)
        {
            // Property, field of method returning value type
            var unaryExpression = (UnaryExpression)expression;
            return GetMemberName(unaryExpression);
        }

        throw new ArgumentException("Invalid expression");
    }

    private static string GetMemberName(
        UnaryExpression unaryExpression)
    {
        if (unaryExpression.Operand is MethodCallExpression)
        {
            var methodExpression =
                (MethodCallExpression)unaryExpression.Operand;
            return methodExpression.Method.Name;
        }

        return ((MemberExpression)unaryExpression.Operand)
            .Member.Name;
    }
}