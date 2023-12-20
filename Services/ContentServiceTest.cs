using System.Linq.Expressions;
using FluentAssertions;
using Sample.Core.Abstractions.Services;
using Sample.Core.Models;

using Sample.WholeModelTests.Gateways;
using Sample.WholeModelTests.Learn;
using LanguageExt;
using Xunit;

namespace Sample.WholeModelTests.Services
{
    public class ContentServiceTest
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

    public class ContentService<T> : IContentServiceAsync<T>      where T : Content
    {
        public ContentService(IContentGatewayBadAsync<Content> failingGatewayBad)
        {
            throw new NotImplementedException();
        }


        Task<Either<string, Content>> IContentServiceAsync.Get(string uid, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<Either<IEnumerable<string>, IEnumerable<T>>> Get(IEnumerable<string> uids, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<Either<Exception, IEnumerable<T>>> Get(Expression<Func<T, bool>> predicate, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<Either<Exception, IEnumerable<T>>> GetAll(CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<Either<T, T>> Save(T content, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<Either<T, T>> Delete(T content, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<Either<IEnumerable<T>, IEnumerable<T>>> Save(IEnumerable<T> contents, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<Either<IEnumerable<T>, IEnumerable<T>>> Delete(IEnumerable<T> contents, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        Task<Either<string, T>> IContentServiceAsync<T>.Get(string uid, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
