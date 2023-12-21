
using Sample.Core.Models;
using LanguageExt;
using System.Linq.Expressions;

namespace Sample.Core.Abstractions.Services
{
    public interface IContentServiceAsync {
        Task<Either<string, Content>> Get(string uid, CancellationToken token);
    }
    public interface IContentServiceAsync<T>:IContentServiceAsync
        where T : Content
    {
        /// <summary>
        /// get entity
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="token"></param>
        /// <returns>left:uid, right: Entity</returns>
        Task<Either<Exception,T>> Get(string uid, CancellationToken token);
        Task<Either<IEnumerable<Exception>,IEnumerable<T>>> Get(IEnumerable<string> uids, CancellationToken token);
        Task<Either<Exception, IEnumerable<T>>> Get(Expression<Func<T, bool>> predicate, CancellationToken token);
        Task<Either<Exception, IEnumerable<T>>> GetAll(CancellationToken token);
        Task<Either<T,T>> Save(T content, CancellationToken token);
        Task<Either<T,T>> Delete(T content, CancellationToken token);
        Task<Either<IEnumerable<T>, IEnumerable<T>>> Save(IEnumerable<T> contents, CancellationToken token);
        Task<Either<IEnumerable<T>, IEnumerable<T>>> Delete(IEnumerable<T> contents, CancellationToken token);

    }

    public class ContentValidation<T>
    {
    }
}
