using FluentAssertions;
using Xunit;

namespace Sample.WholeModelTests.LearnAsync
{
    // to learn deeper about await/async, please watch https://github.com/brminnick/AsyncAwaitBestPractices#asyncawaitbestpractices-3
    public class AsyncReturnIntTest
    {
        int Counter = 0;

        [Fact()]
        public void InvokeNonAsync()
        {
            //  Counter = GetQueryResult(Counter); you cannot write it
            Counter.Should().Be(0);
        }

        [Fact()]
        public async void InvokeAsync()
        {
            var res = await GetQueryResult(0);

            res.Should().Be(1);
        }

        [Fact()]
        public void WaitAsyncResult() //similar to previous, except it is no more asynchronuous, it is blocking
        {
            // “Async all the way” means that you shouldn’t mix synchronous and asynchronous code without carefully considering the consequences.
            // In particular, it’s usually a bad idea to block on async code by calling Task.Wait or Task.Result
            var t = GetQueryResult(0);
            var r = t.Wait(1000);

            r.Should().Be(true);
            t.Result.Should().Be(1);
        }

        [Fact()]
        public void ResultAsync() //similar to previous
        {
            // “Async all the way” means that you shouldn’t mix synchronous and asynchronous code without carefully considering the consequences.
            // In particular, it’s usually a bad idea to block on async code by calling Task.Wait or Task.Result
            var res = GetQueryResult(0).Result;
            res.Should().Be(1);
        }

        [Fact()]
        public async void InvokeAsyncWithContinuationNotAwaited()
        {
            Task<int> task = GetQueryResult(0).ContinueWith((c) => { return c.Result + 1; });
            var r = await task.ConfigureAwait(false);
            r.Should().Be(2);
        }

        [Fact()]
        public async void InvokeAsyncWithContinuationAsync()
        {
            Task<int> mainTask = GetQueryResult(1);

            // ContinueWith with an asynchronous delegate
            Task<Task<int>> continuationTask = mainTask.ContinueWith(async antecedent =>
            {
                // Inside this continuation, you can use await
                Console.WriteLine($"The result is: {await antecedent}");
                return antecedent.Result * 2; // Returning a modified result
            });

            // Wait for the continuation task to complete and get the result
            int res = await continuationTask.Result;

            res.Should().Be(4);
        }

        [Fact()]
        public async void InvokeAsyncWithContinuationtAwaited()
        {
            var mainTask = Task.Run(() => GetQueryResult(0));

            var res = await ContinueWithAsync(mainTask).ConfigureAwait(false);

            res.Should().Be(3);
        }

        [Fact()]
        public async void InvokeAsyncWithContinuationtAwaited2()
        {
            var mainTask = GetQueryResult(0);

            var res = await ContinueWithAsync(mainTask).ConfigureAwait(false);

            res.Should().Be(3);
        }



        async Task<int> ContinueWithAsync(Task<int> antecedent)
        {
            Console.WriteLine($"The result is: {await antecedent}");
            return antecedent.Result + 2; // Returning a modified result
        }

        private Task<int> MakeComputation(int result)
        {
            return Task.FromResult(result + 1);
        }


        async Task<int> GetQueryResult(Int32 i)
        {
            await Task.Delay(50).ConfigureAwait(false);
            return i + 1;
        }

        /* * Consider looking at https://devblogs.microsoft.com/dotnet/understanding-the-whys-whats-and-whens-of-valuetask/ 
         * * (...) ValueTask / ValueTask<TResult> are great choices when
         * a) you expect consumers of your API to only await them directly,
         * b) allocation-related overhead is important to avoid for your API, and 
         * c) either you expect synchronous completion to be a very 
         * common case, or you’re able to effectively pool objects for use with asynchronous completion.
         * When adding abstract, virtual, or interface methods, 
         * you also need to consider whether these situations will exist for overrides/implementations of that method.
         * */


    }
}
