using System.Linq.Expressions;
using LanguageExt;
using Sample.Core.Abstractions.Services;
using Sample.Core.Models;
using Sample.WholeModelTests.Learn;

namespace Sample.WholeModelTests.Services;

public class ContentService<T> : IContentServiceAsync<T>      where T : Content
{
    public ContentService(IContentGatewayBadAsync<Content> failingGatewayBad)
    {
        throw new NotImplementedException();
    }
    

    public Task<Either<IEnumerable<Exception>, IEnumerable<T>>> Get(IEnumerable<string> uids, CancellationToken token)
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

    Task<Either<Exception, T>> IContentServiceAsync<T>.Get(string uid, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task<Either<string, Content>> Get(string uid, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}