using FluentAssertions;
using Sample.Core.Models;

using Sample.WholeModelTests.Gateways;
using Xunit;

namespace Sample.WholeModelTests.Services
{
    public class ContentServiceTest  // NEED TO create code so that the tests shall pass
    {
        [Fact] 
        public async void TestContentServiceSave()
        {
            var failingGateway = new FakeGatewayBadAsync<Content>();
            var service = new ContentService<Content>(failingGateway);
            var content1 = new Content() { Uid = "ok"};
            var res = await service.Save(content1,CancellationToken.None);
            res.Match((x)=> Assert.True(true),(x)=> Assert.True(false));
           
        }
        [Fact]
        public async void TestContentServiceSaveFail()
        {
            var failingGateway = new FakeGatewayBadAsync<Content>();
            var service = new ContentService<Content>(failingGateway);
            var content1 = new Content() { Uid = "fail" };
            var res = await service.Save(content1, CancellationToken.None);
            res.Match((x) => Assert.True(false), (x) => Assert.True(true));
        }
        [Fact]
        public async void TestContentServiceSaveContents()
        {
            var failingGateway = new FakeGatewayBadAsync<Content>();
            var service = new ContentService<Content>(failingGateway);
            var howMany = 10;
            var contents = from x in Enumerable.Range(0, howMany) select new Content() { Uid = "ok" };
            var res = await service.Save(contents, CancellationToken.None);
            res.Match(
                (x) => {
                    Assert.Equal(x.Count(), howMany);
                    }, 
                (x) => Assert.True(false));
        }
        [Fact]
        public async void TestContentServiceSaveFailingContents()
        {
            var failingGateway = new FakeGatewayBadAsync<Content>();
            var service = new ContentService<Content>(failingGateway);
            var howMany = 10;
            var contents = from x in Enumerable.Range(0, howMany) select new Content() { Uid = "ok" };
            contents = contents.Union(new List<Content>{ new Content() { Uid = "fail" } });
            var res = await service.Save(contents, CancellationToken.None);
            res.Match(
                (x) => Assert.True(false),
                (x) => x.Count().Should().Be(1));
        }
    }
}
