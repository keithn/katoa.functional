using System;
using System.Linq;
using Katoa.Functional;

namespace Demo
{
    public abstract record Failure
    {
        public record Fail(string Message) : Failure;
    }
    
    class Program
    {
        public static void DemoPiping()
        {
            Result<int, Failure> AsInt(string s) => int.TryParse(s, out var n)
                ? new Result<int, Failure>.Ok(n)
                : new Result<int, Failure>.Error(new Failure.Fail($"{s} is not a number"));

            int Multiply(int n) => n * 2;
            
            Result<int, Failure> Equation(string s) => Result<string, Failure>.Map(s)
                .Then(AsInt)
                .Then(Result<int,Failure>.Map(Multiply));

            void DisplayResult(Result<int, Failure> result)
            {
                switch (result)
                {
                    case Result<int,Failure>.Error (var error): Console.WriteLine($"Got an error: {error}");
                        break;
                    case Result<int,Failure>.Ok (var value): Console.WriteLine($"Calculated: {value}");
                        break;
                }
            }

            DisplayResult(Equation("abc"));
            DisplayResult(Equation("123"));
        }


        static void Main(string[] args)
        {
            DemoPiping();
        }
    }
}