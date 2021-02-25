using System;

namespace Katoa.Functional
{
    public abstract record Result<T, TFailure>
    {
        public record Ok(T Value) : Result<T, TFailure>;

        public record Error(TFailure Failure) : Result<T, TFailure>;

        // Map a value to a result
        public static Result<T, TFailure> Map(T t) => new Ok(t);

        // Map an Action to a result
        public static Func<T, Result<T, TFailure>> Map(Action<T> action)
        {
            Result<T, TFailure> Func(T r)
            {
                action(r);
                return new Ok(r);
            }
            return Func;
        }

        public static Func<T, Result<B, TFailure>> Map<B>(Func<T, B> f)
        {
            Result<B, TFailure> Func(T r)
            {
                return new Result<B, TFailure>.Ok(f(r));
            }

            return Func;
        }
    }

    public static class ResultExtensions
    {
        public static Result<B, TF> Then<A, B, TF>(this Result<A, TF> r, Func<A, B> f1) =>
            r.Then(Result<A, TF>.Map(f1));

        public static Result<B, TF> Then<A, B, TF>(this Result<A, TF> r, Func<A, Result<B, TF>> f1)
        {
            return r switch
            {
                Result<A, TF>.Ok (var value) => f1(value),
                Result<A, TF>.Error error => new Result<B, TF>.Error(error.Failure)
            };
        }

        public static Result<B, TF> Pipe<A, B, TF>(this Result<A, TF> r, Func<A, Result<B, TF>> f1) => Then(r, f1);

        public static Result<C, TF> Pipe<A, B, C, TF>(this Result<A, TF> r, Func<A, Result<B, TF>> f1,
            Func<B, Result<C, TF>> f2)
        {
            return Pipe(r switch
            {
                Result<A, TF>.Ok (var value) => f1(value),
                Result<A, TF>.Error error => new Result<B, TF>.Error(error.Failure)
            }, f2);
        }
    }
}