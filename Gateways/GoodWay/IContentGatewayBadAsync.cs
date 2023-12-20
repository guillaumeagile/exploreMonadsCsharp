using System.Linq.Expressions;
using LanguageExt;
using LanguageExt.Common;
using Sample.Core.Models;

namespace Sample.WholeModelTests.Gateways.GoodWay;

// better way using EitherAsync instead of TryAsync
public interface IContentGatewayAsync<T> where T : Content
{
    EitherAsync<Error,T > Get(string uid, CancellationToken token);
    EitherAsync<Error,T > Get(IEnumerable<string> uids, CancellationToken token);
    EitherAsync<Error,T > Get(Expression<Func<T, bool>> predicate, CancellationToken token);

    EitherAsync<Error,T > GetAll(CancellationToken token);

    // for convinience of the fluent calls, it is usefull to return the object that have been saved
    EitherAsync<Error,T > Save(T content, CancellationToken token);

    EitherAsync<Error,T > Delete(T content, CancellationToken token);
}