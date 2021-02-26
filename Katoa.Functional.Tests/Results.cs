using System;
using Xunit;

namespace Katoa.Functional.Tests
{
    public abstract record Failure : Katoa.Functional.Failure
    {
        public record Fail(string Message) : Failure;
    }
    
    public class Results
    {
        [Fact]
        public void Map_Non_Result_TransForm_To_Result()
        {
            string simpleTransform(string s)  => s + "_";

            var resultified = Result<string>.Map(simpleTransform);
            var result = Result<string>.Map("test").Then(resultified);
            Assert.IsType<Result<string>.Ok>(result);
            Assert.Equal("test_", ((Result<string>.Ok) result).Value);
        }
        [Fact]
        public void Then_Maps_To_Transform()
        {
            string simpleTransform(string s)  => s + "_";
            var result = Result<string>.Map("test").Then(simpleTransform);
            Assert.IsType<Result<string>.Ok>(result);
            Assert.Equal("test_", ((Result<string>.Ok) result).Value);
        }
        [Fact]
        public void Pipe_With_Successful_Transforms()
        {
            Result<string> func(string s) => new Result<string>.Ok(s + "func");
            Result<string> func2(string s) => new Result<string>.Ok(s + "func2");
            var result = func("begin").Then(func2);
            Assert.IsType<Result<string>.Ok>(result);
            Assert.Equal("beginfuncfunc2", ((Result<string>.Ok) result).Value);
        }
        [Fact]
        public void Pipe_With_Failure_And_Ensure_Skipping_Of_Rest_Of_Pipe()
        {
            Result<string> func(string s) => new Result<string>.Error(new Failure.Fail("failed"));

            // never should get called
            Result<string> func2(string s) => throw new Exception();

            var result = func("begin").Then(func2);
            Assert.IsType<Result<string>.Error>(result);
            Assert.Equal("failed", (((Result<string>.Error) result).Failure as Failure.Fail)?.Message);
        }
    }
}