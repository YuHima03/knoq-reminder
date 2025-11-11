using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace KnoqReminder.Utilities.Helpers;

static class GenericThrowHelper
{
    static class CachedInitializer<TException> where TException : Exception
    {
        public static readonly Lazy<Func<TException>> Initializer = new(isThreadSafe: true, valueFactory: () =>
        {
            return Expression.Lambda<Func<TException>>(
                Expression.New(typeof(TException).GetConstructor([])!)
            ).Compile();
        });

        public static readonly Lazy<Func<string?, Exception>> InitializerWithMessage = new(isThreadSafe: true, valueFactory: () =>
        {
            var msgParam = Expression.Parameter(typeof(string), "message");
            return Expression.Lambda<Func<string?, TException>>(
                Expression.New(
                    typeof(TException).GetConstructor([typeof(string)])!,
                    msgParam
                ),
                msgParam
            ).Compile();
        });

        public static readonly Lazy<Func<string?, Exception?, Exception>> InitializerWithMessageAndInnerException = new(isThreadSafe: true, valueFactory: () =>
        {
            var msgParam = Expression.Parameter(typeof(string), "message");
            var exParam = Expression.Parameter(typeof(Exception), "innerException");
            return Expression.Lambda<Func<string?, Exception?, TException>>(
                Expression.New(
                    typeof(TException).GetConstructor([typeof(string), typeof(Exception)])!,
                    msgParam,
                    exParam
                ),
                msgParam,
                exParam
            ).Compile();
        });
    }

    [DoesNotReturn]
    public static void Throw<TException>()
        where TException : Exception
    {
        throw CachedInitializer<TException>.Initializer.Value();
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult Throw<TException, TResult>()
        where TException : Exception
        where TResult : allows ref struct
    {
        Throw<TException>();
        return default;
    }

    [DoesNotReturn]
    public static void Throw<TException>(string? message)
        where TException : Exception
    {
        throw CachedInitializer<TException>.InitializerWithMessage.Value(message);
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult Throw<TException, TResult>(string? message)
        where TException : Exception
        where TResult : allows ref struct
    {
        Throw<TException>(message);
        return default;
    }

    [DoesNotReturn]
    public static void Throw<TException>(string? message, Exception? innerException)
        where TException : Exception
    {
        throw CachedInitializer<TException>.InitializerWithMessageAndInnerException.Value(message, innerException);
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult Throw<TException, TResult>(string? message, Exception? innerException)
        where TException : Exception
        where TResult : allows ref struct
    {
        Throw<TException>(message, innerException);
        return default;
    }
}
