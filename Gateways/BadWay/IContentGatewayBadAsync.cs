using System.Linq.Expressions;
using Sample.Core.Models;
using LanguageExt;

namespace Sample.WholeModelTests.Learn;

// a bad version using TryAsync ----   to be replaced by
public interface IContentGatewayBadAsync<T> where T : Content
{
    TryAsync<T> Get(string uid, CancellationToken token);
    TryAsync<IEnumerable<T>> Get(IEnumerable<string> uids, CancellationToken token);
    TryAsync<IEnumerable<T>> Get(Expression<Func<T, bool>> predicate, CancellationToken token);

    TryAsync<IEnumerable<T>> GetAll(CancellationToken token);

    // for convinience of the fluent calls, it is usefull to return the object that have been saved
    TryAsync<T> Save(T content, CancellationToken token);

    TryAsync<T> Delete(T content, CancellationToken token);

    TryAsync<Either<IEnumerable<T>, IEnumerable<T>>> Save(IEnumerable<T> contents, CancellationToken token);

    TryAsync<Either<IEnumerable<T>, IEnumerable<T>>> Delete(IEnumerable<T> contents, CancellationToken token);

}