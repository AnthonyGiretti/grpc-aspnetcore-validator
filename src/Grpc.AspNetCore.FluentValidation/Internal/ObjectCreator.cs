using System;
using System.Linq.Expressions;

namespace Grpc.AspNetCore.FluentValidation.Internal
{
    internal static class ObjectCreator<T>
    {
        private static readonly Type Type = typeof(T);

        private static readonly Func<T> Builder =
            Expression.Lambda<Func<T>>(Expression.Block(Type, Expression.New(Type))).Compile();

        public static readonly T Empty = Builder();
    }
}