using FluentAssertions;
using Sample.Core.Models;
using Xunit;

namespace Sample.WholeModelTests.Gateways
{
    public class FakeGatewayBadAsyncTests
    {
        [Fact()]
        public async void DeleteSucess()
        {
            // ARRANGE
            var fakeCOntent = new Content();
            var sut = new FakeGatewayBadAsync<Content>();
            // ACT
            await sut.Save(fakeCOntent, CancellationToken.None);
            var res = await sut.Delete(fakeCOntent, CancellationToken.None);
            // ASSERT
            var a = await res.MapAsync(
                (val) =>
                {
                   val.Should().NotBeNull();
                    val.Uid.Should().Be(fakeCOntent.Uid);
                    return Task.FromResult(true);// this is required by MapAsync
                })
                .IfFail(
                (e) =>
                {
                    e.Should().BeNull("we should not go here");
                    return false;
                });
        }

        [Fact()]
        public async void DeleteFailure()
        {
            // ARRANGE
            var fakeCOntent = new Content() { Uid = "fail"};
            var sut = new FakeGatewayBadAsync<Content>();
            // ACT
            await sut.Save(fakeCOntent, CancellationToken.None);
            var res = await sut.Delete(fakeCOntent, CancellationToken.None);
            // ASSERT
            var a = await res.MapAsync(
                (val) =>
                {
                    val.Should().BeNull("we should not go here");
                   
                    return Task.FromResult(true);// this is required by MapAsync
                })
                .IfFail(
                (e) =>
                {
                    e.Message.Should().Be("content Uid should not start with f");
                    return false;
                });
        }

       [Fact()]  // to be continued
        async void SaveSucessThenReadOk()
        {
            // ARRANGE
            var fakeCOntent = new Content();
            var sut = new FakeGatewayBadAsync<Content>();
            // ACT
            await sut.Save(fakeCOntent, CancellationToken.None);
           
            var res = await sut.Get(fakeCOntent.Uid, CancellationToken.None);
            // ASSERT
            await res.MapAsync(
                (val) =>
                {
                  
                    return Task.FromResult(false);// this is required by MapAsync
                })
                .IfFail(
                (e) =>
                {
                    e.Should().BeNull("we should not go here");
                    return true;
                });
        }

         [Fact()] //to be continued
        async void DeleteSucessThenReadFailBeceauseTheContentHasBeenDeleted()
        {
            // ARRANGE
            var fakeCOntent = new Content();
            var sut = new FakeGatewayBadAsync<Content>();
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
    }
}
