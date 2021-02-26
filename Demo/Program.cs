using System;
using System.Linq;
using Katoa.Functional;

namespace Demo
{
    public abstract record Failure : Katoa.Functional.Failure
    {
        public record Fail(string Message) : Failure;
        public record Thrown(Exception Exception) : Failure;
    }

    class Program
    {
        public static void DemoPiping()
        {
            Result<int> AsInt(string s) => int.TryParse(s, out var n)
                ? new Result<int>.Ok(n)
                : new Result<int>.Error(new Failure.Fail($"{s} is not a number"));

            int Multiply(int n) => n * 2;

            Result<int> Equation(string s) => Result<string>.Map(s)
                .Then(AsInt)
                .Then(Result<int>.Map(Multiply));

            string DescribeResult(Result<int> result) => result switch
            {
                Result<int>.Error (var error) => $"Got an error: {error}",
                Result<int>.Ok (var value) => $"Calculated: {value}",
                _ => "Unknown Result"
            };

            Console.WriteLine(DescribeResult(Equation("abc")));
            Console.WriteLine(DescribeResult(Equation("123")));
        }


        static void Main(string[] args)
        {
            DemoPiping();
        }
    }
}