using FluentAssertions;
using LanguageExt;
using Xunit;
using static LanguageExt.Prelude;

namespace Sample.WholeModelTests.LearnAsync
{
    public class TryAsyncReturnIntTest
    {
        [Fact()]
        public async void InvokeAsync()
        {

            // Using TryAsync to handle asynchronous operations with exception handling
            var result = await TryAsync<int>(async () =>
            {
                int computationResult = await GetQueryResult(0);
                Console.WriteLine($"The result is: {computationResult}");
                return computationResult * 2; // Returning a modified result
            });

            // Check if the TryAsync was successful
            result.Match(
                Succ: finalResult => finalResult.Should().Be(2),
                Fail: ex => throw new Xunit.Sdk.XunitException("we should not go here")
            );
        }

        [Fact()]
        public async void InvokeAsyncWithException()
        {

            // Using TryAsync to handle asynchronous operations with exception raising
            var result = await TryAsync<int>(async () =>
            {
                int computationResult = await GetQueryResult(2);
                return await MakeAnotherComputation(computationResult); // throws an exception
            });

            // Check if the TryAsync was failure
            result.Match(
                Succ: finalResult => throw new Xunit.Sdk.XunitException("we should not go here"),
                Fail: ex => ex.Message.Should().Be("booh")
            );
        }

        [Fact()]
        public async void InvokeAsyncWithMethod()
        {
            Try<int> result = await NewMethodAsync(2);

            // Check if the TryAsync was failure
            result.Match(
                Succ: finalResult => throw new Xunit.Sdk.XunitException("we should not go here"),
                Fail: ex => ex.Message.Should().Be("booh")
            );
        }

        [Fact()]
        public async void InvokeTryAsyncWithMethod()
        {
            Try<int> result = await NewMethodTryAsync(1);

            // Check if the TryAsync was failure
            result.Match(
                Succ: finalResult => finalResult.Should().Be(3),
                Fail: ex => throw new Xunit.Sdk.XunitException("we should not go here")// ex.Message.Should().Be("booh")
            );
        }

        private async Task<Try<int>> NewMethodAsync(int query)
        {

            // Using TryAsync to handle asynchronous operations with exception raising
            var result = await TryAsync<int>(async () =>
            {
                int computationResult = await GetQueryResult(query).ConfigureAwait(false);
                return await MakeAnotherComputation(computationResult).ConfigureAwait(false); // throws an exception
            });
            return result;
        }

        private TryAsync<int> NewMethodTryAsync(int query)
        {
            return TryAsync(async () =>
            {
                int computationResult = await GetQueryResult(query).ConfigureAwait(false);
                return await MakeAnotherComputation(computationResult).ConfigureAwait(false); // throws an exception});
                                                                        // Using TryAsync to handle asynchronous operations with exception raising
            });
        }

        Task<int> MakeAnotherComputation(int result)
        {
            if (result > 2)
                throw new Exception("booh");
            return Task.FromResult(result + 1);
        }

        async Task<int> GetQueryResult(Int32 i)
        {
            await Task.Delay(50);
            return i + 1;
        }

        async Task<int> ContinueWithAsync(Task<int> antecedent)
        {
            Console.WriteLine($"The result is: {await antecedent}");
            return antecedent.Result + 2; // Returning a modified result
        }

    }
}
