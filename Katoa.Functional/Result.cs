using System;

namespace Katoa.Functional
{
    public abstract record Failure;
    public abstract record Result<T >
    {
        public record Ok(T Value) : Result<T>;

        public record Error(Failure Failure) : Result<T>;

        // Map a value to a result
        public static Result<T> Map(T t) => new Ok(t);

        // Map an Action to a result
        public static Func<T, Result<T>> Map(Action<T> action)
        {
            Result<T> Func(T r)
            {
                action(r);
                return new Ok(r);
            }
            return Func;
        }

        public static Func<T, Result<B>> Map<B>(Func<T, B> f)
        {
            Result<B> Func(T r)
            {
                return new Result<B>.Ok(f(r));
            }

            return Func;
        }
    }

    public static class ResultExtensions
    {
        public static Result<B> Then<A, B>(this Result<A> r, Func<A, B> f1) =>
            r.Then(Result<A>.Map(f1));

        public static Result<B> Then<A, B>(this Result<A> r, Func<A, Result<B>> f1)
        {
            return r switch
            {
                Result<A>.Ok (var value) => f1(value),
                Result<A>.Error error => new Result<B>.Error(error.Failure)
            };
        }

        public static Result<B> Pipe<A, B>(this Result<A> r, Func<A, Result<B>> f1) => Then(r, f1);

        public static Result<C> Pipe<A, B, C>(this Result<A> r, Func<A, Result<B>> f1,
            Func<B, Result<C>> f2)
        {
            return Pipe(r switch
            {
                Result<A>.Ok (var value) => f1(value),
                Result<A>.Error error => new Result<B>.Error(error.Failure)
            }, f2);
        }
    }
}