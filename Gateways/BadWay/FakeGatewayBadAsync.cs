
using Sample.Core.Models;
using LanguageExt;
using System.Linq.Expressions;
using Sample.WholeModelTests.Learn;
using static LanguageExt.Prelude;

namespace Sample.WholeModelTests.Gateways
{
    /// <summary>
    /// things will fail when the content uid start with "f"
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class FakeGatewayBadAsync<T>: IContentGatewayBadAsync<T>  where T : Content //
    {

        private System.Collections.Generic.HashSet<T> _contents = new System.Collections.Generic.HashSet<T>();

        public TryAsync<T> Delete(T content, CancellationToken token)
        {
            return TryAsync(() =>
            {
                if (content.Uid.StartsWith("f"))
                    throw new Exception("content Uid should not start with f");
                this._contents.Remove(content);
                return  Task.FromResult(content);
            });
        }

        public TryAsync<Either<IEnumerable<T>, IEnumerable<T>>> Delete(IEnumerable<T> contents, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public TryAsync<T> Get(string uid, CancellationToken token)
        {
            return TryAsync(() =>
            {
                if (_contents.Filter(e => e.Uid == uid).Any())
                    return Task.FromResult(  _contents.Filter(e => e.Uid == uid).First());
                
                throw new Exception("Content not found");
            });
        }

        public TryAsync<IEnumerable<T>> Get(IEnumerable<string> uids, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public TryAsync<IEnumerable<T>> Get(Expression<Func<T, bool>> predicate, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public TryAsync<IEnumerable<T>> GetAll(CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public TryAsync<T> Save(T content, CancellationToken token)
        {
            return TryAsync(() =>
            {
                if (content.Uid.StartsWith("f"))
                    throw new Exception();
                _contents.Add(content);
                return Task.FromResult(content);
            });
        }

        public TryAsync<Either<IEnumerable<T>, IEnumerable<T>>> Save(IEnumerable<T> contents, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
