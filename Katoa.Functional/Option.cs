namespace Katoa.Functional
{
    public abstract record Option<T>
    {
        public record Some(T value) : Option<T>;

        public record None() : Option<T>;
    }
}