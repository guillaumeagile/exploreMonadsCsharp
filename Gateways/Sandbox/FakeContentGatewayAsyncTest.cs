using FluentAssertions;
using Sample.Core.Models;
using LanguageExt;
using static LanguageExt.Prelude;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Sample.WholeModelTests.Gateways.CleanVersion
{

    public class FakeContentGatewayAsyncTest
    {
        bool hasBeenCorrectlyLogged = false;

        public FakeContentGatewayAsyncTest()
        {
            hasBeenCorrectlyLogged = false;
        }

        // this serie of tests in an concrete and working example of Railway Programming
        // many articles and books refer to it 
        // 19mn video: https://www.youtube.com/watch?v=dDasAmowFts  some approximation in it, but he makes the concept easier to understand
        // https://naveenkumarmuguda.medium.com/railway-oriented-programming-a-powerful-functional-programming-pattern-ab454e467f31
        // examples in Ruby: https://medium.com/geekculture/better-architecture-with-railway-oriented-programming-ad4288a273ce

        [Fact()]
        public async void DeleteSucess()
        {
            // ARRANGE
            var fakeCOntent = new Content();
            var sut = new FakeContentGatewayAsync<Content>();
            // ACT
            await sut.Save(fakeCOntent, CancellationToken.None);
            var res = await sut.Delete(fakeCOntent, CancellationToken.None);
            // ASSERT
            var a = await res.MapAsync(
                (val) =>
                {
                    Assert.NotNull(val);
                    return Task.FromResult(true);// this is required by MapAsync
                })
                .IfFail(
                (e) =>
                {
                    Assert.Null("we should not go here");
                    return false;
                });
        }

        [Fact()]
        public async void Delete2Sucess()
        {
            // ARRANGE
            var content1 = new Content();   
            var content2 = new Content();
            var fakeCOntent = new List<Content>() { content1 , content2};
            var sut = new FakeContentGatewayAsync<Content>();
            // ACT
            await sut.Save(content1, CancellationToken.None);
            await sut.Save(content2, CancellationToken.None);
            Aff<IEnumerable<Content>> res =  sut.Delete2(fakeCOntent, CancellationToken.None);
           
            // ASSERT

            var a =  res.Match(
                (succ) =>
                {
                    Assert.NotNull(succ);
                    succ.Should().NotBeEmpty();
                    return "ok";
                },
                (fail) => {
                    fail.Should().BeNull();
                    return "ko";
                });
                
               
        }


        [Fact()]
        public async void DeleteSucessThenReadFail()
        {
            // ARRANGE
            var fakeCOntent = new Content();
            var sut = new FakeContentGatewayAsync<Content>();
            // ACT
            await sut.Save(fakeCOntent, CancellationToken.None);
            await sut.Delete(fakeCOntent, CancellationToken.None);
            var res = await sut.Get(fakeCOntent.Uid, CancellationToken.None);
            // ASSERT
            await res.MapAsync(
                (val) =>
                {
                    Assert.Null(val); //"we should not go here"
                    return Task.FromResult(false);// this is required by MapAsync
                })
                .IfFail(
                (e) =>
                {
                    e.Message.Should().Be("Content not found");
                    return true;
                });
        }

        [Fact()]
        public async void DeleteFailure()
        {
            // ARRANGE
            var fakeCOntent = new Content();
            var sut = new FakeContentGatewayAsync<Content>();
            // ACT
            // await sut.Save(fakeCOntent, CancellationToken.None);
            var res = await sut.Delete(fakeCOntent, CancellationToken.None).ToEither();
            // ASSERT
            var finalResult = await res.Match(
                 (val) =>
                 {
                     val.Should().BeNull(); // "we should not go here"
                     return Task.FromResult(false);
                 },
                 (e) =>
                 {
                     e.Message.Should().Be("Content not found");
                     return Task.FromResult(true);
                 });
            finalResult.Should().BeTrue();
        }

        // LEARN:  a test who demonstrates the difference between Apply and Bind <<<<<<<<<<<<<<<<<<<<
        [Fact()]
        public async void DeleteFailureAndLog()
        {
            // ARRANGE
            var fakeCOntent = new Content();
            var sut = new FakeContentGatewayAsync<Content>();

            // ACT
            // var res0 = await  sut.Save(fakeCOntent, CancellationToken.None); //<-- add this line to watch the test failing  (DO NOT ADD await HERE)         
            // Warning: if you want to synchronize 2 task, we should use Task.WhenAll
            //
            var res = await sut.Delete(fakeCOntent, CancellationToken.None).Apply(async c => await SpyLog(c));

            var res2 = res.ToEither();
            // ASSERT
            var finalResult = res2.Match(
                (val) =>
                {
                    Assert.Empty("we should not go here");
                },
                (e) =>
                {
                    e.Message.Should().StartWith("Content not found"); //only the first action did execute, not the 2nd in the Bind
                });

        }

        private TryAsync<Content> SpyLog(TryAsync<Content> tryMe)
        {
            var r = tryMe.MapAsync(
                  (val) =>
                  {
                      SpyLog2($"content {val.Uid} have been deleted");
                      return Task.FromResult(true);// this is required by MapAsync
                  })
                  .IfFail(
                  (e) =>
                  {
                      SpyLog2(e.Message);
                      return true;
                  });
            return tryMe;
        }

        private void SpyLog2(string s)
        {
            var nl = NullLoggerFactory.Instance; // https://codeburst.io/unit-testing-with-net-core-ilogger-t-e8c16c503a80
            var l = nl.CreateLogger("for tests");
            l.LogDebug(s);
            hasBeenCorrectlyLogged = true;

        }

        private TryAsync<Content> SpyLog2(Content c)
        {
            var nl = NullLoggerFactory.Instance; // https://codeburst.io/unit-testing-with-net-core-ilogger-t-e8c16c503a80
            var l = nl.CreateLogger("for tests");
            l.LogDebug($"content {c.Uid} have been deleted");
            hasBeenCorrectlyLogged = true;
            return TryAsync(async () =>
            {
                await Task.Delay(50);
                return (c); // if you don't havr an exception you will have tha result (see tests)              
            });
        }
        // yeah, we could Mock the logger, like https://copyprogramming.com/howto/c-create-ilogger-for-unit-test
        // but it's an anti pattern, as explained here: https://indexoutofrange.com/Stop-trying-to-mock-the-ILogger-methods/
        // we could simply write a fake logger like this small example https://github.com/nsubstitute/NSubstitute/issues/597#issuecomment-569273714


        [Fact()]
        public async void GetFailure()
        {
            // ARRANGE
            var fakeCOntent = new Content();
            var sut = new FakeContentGatewayAsync<Content>();
            // ACT
            var res = await sut.Get("", CancellationToken.None);
            // ASSERT
            var resOfMap = await res.MapAsync(
                (val) =>
                {
                    Assert.Empty("we should not go here");
                    // beware that if assert fails here, an exception will be raised, and we'll jump into the IfFail below !
                    return Task.FromResult(false);// this is required by MapAsync
                })
                .IfFail(
                (e) =>
                {
                    e.Message.Should().Be("Content not found");
                    // you can see that Apply have propagated the failure raised from Delete
                    return true;// this is required by MapAsync
                });
            resOfMap.Should().BeTrue();
        }

        [Fact()]
        public async void SaveThenGetWthBind_ShowUsageOfEitherAsync()
        {
            // ARRANGE
            var sampleContent = new Content();
            var sut = new FakeContentGatewayAsync<Content>();

            // ACT
            var resOfSave = sut.Save(sampleContent, CancellationToken.None).ToEither(); ;
            var resOfGet = resOfSave.Bind(previousResult => sut.Get(previousResult.Uid, CancellationToken.None).ToEither());

            // ASSERT
            await resOfGet.Match(
                (val) =>
                {
                    val.Uid.Should().Be(sampleContent.Uid);
                },
                (e) =>
                {
                    e.Message.Should().BeEmpty(); // "we should not go here"
                });
        }

        [Fact()]
        public async void SaveTwoThenGetTheLastInChain()
        {
            var sampleContent = new Content();
            var sut = new FakeContentGatewayAsync<Content>();
            var sampleContent2 = new Content();
            //1st save
            await sut.Save(sampleContent, CancellationToken.None);
            //2nd save
            var resOfSave = await sut.Save(sampleContent2, CancellationToken.None); ;
            // using Map will produce a Try of Try
            var resOfGet = resOfSave.MapAsync(async (c) => await sut.Get(sampleContent2.Uid, CancellationToken.None));

            await resOfGet.MapAsync( // we look inside the first Try
                 (val) =>
                 {
                     val.Match( // then we observe the 2nd try
                         (val2) =>
                         {
                             val2.Uid.Should().Be(sampleContent2.Uid); //if an error here, it will be propagated to the parent Try
                         },
                         (e) =>
                         {
                             Assert.Null(e.Message); //never go here
                         });
                     return Task.FromResult(true); // this is required by MapAsync

                 }).IfFail(
                 (e) =>
                 {
                     Assert.Null(e.Message); //never go here
                     return false;// this is required by IfFail
                 });
        }

        // this test explains how Result and Either can be combined, and how we can Chain/Bind operation
        // this is called Railway Programming https://blog.logrocket.com/what-is-railway-oriented-programming/
        [Fact()]
        public async void SaveWithAnErrorThenGet()
        {
            // ARRANGE
            var voidContent = Content.Void();
            var sut = new FakeContentGatewayAsync<Content>();

            // ACT
            var resOfSave = sut.Save(voidContent, CancellationToken.None).ToEither(); ;
            var resOfGet = resOfSave.Bind(previousResult => sut.Get(previousResult.Uid, CancellationToken.None).ToEither());

            // ASSERT
            await resOfGet.Match(
                (val) =>
                {
                    Assert.Empty("we should not go here");
                },
                (e) =>
                {
                    e.Message.Should().StartWith("cannot save a void content"); //only the first action did execute, not the 2nd in the Bind
                });
        }
    }
}
