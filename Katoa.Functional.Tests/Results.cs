using System;
using Xunit;

namespace Katoa.Functional.Tests
{
    public abstract record Failure
    {
        public record Fail(string Message) : Failure;
    }
    
    public class Results
    {
        [Fact]
        public void Map_Non_Result_TransForm_To_Result()
        {
            Func<string, string> simpleTransform = s => s + "_";

            var resultified = Result<string, Failure>.Map(simpleTransform);
            var result = Result<string,Failure>.Map("test").Then(resultified);
            Assert.IsType<Result<string,Failure>.Ok>(result);
            Assert.Equal("test_", ((Result<string, Failure>.Ok) result).Value);
        }
        [Fact]
        public void Then_Maps_To_Transform()
        {
            Func<string, string> simpleTransform = s => s + "_";
            var result = Result<string, Failure>.Map("test").Then(simpleTransform);
            Assert.IsType<Result<string,Failure>.Ok>(result);
            Assert.Equal("test_", ((Result<string, Failure>.Ok) result).Value);
        }
        [Fact]
        public void Pipe_With_Successful_Transforms()
        {
            Func<string, Result<string, Failure>> func = s =>
                new Result<string, Failure>.Ok(s + "func");

            Func<string, Result<string, Failure>> func2 = s =>
                new Result<string, Failure>.Ok(s +"func2");
            
            var result = func("begin").Then(func2);
            Assert.IsType<Result<string,Failure>.Ok>(result);
            Assert.Equal("beginfuncfunc2", ((Result<string, Failure>.Ok) result).Value);
        }
        [Fact]
        public void Pipe_With_Failure_And_Ensure_Skipping_Of_Rest_Of_Pipe()
        {
            Func<string, Result<string, Failure>> func = s =>
                new Result<string, Failure>.Error(new Failure.Fail("failed"));

            // never should get called
            Func<string, Result<string, Failure>> func2 = s => throw new Exception();

            var result = func("begin").Then(func2);
            Assert.IsType<Result<string,Failure>.Error>(result);
            Assert.Equal("failed", (((Result<string, Failure>.Error) result).Failure as Failure.Fail)?.Message);
        }
    }
}