
using Sample.Core.Models;

using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace Sample.WholeModelTests.Gateways.CleanVersion
{
    // https://medium.com/c-sharp-progarmming/async-await-in-net-best-practices-4154408c9779
    class FakeContentGatewayAsync<T>  where T : Content
    {
        private const int MillisecondsDelay = 50;

        private List<T> inMemoryPersistence = new();

        public TryAsync<T> Delete(T content, CancellationToken token)
        {
            var a = (Content)content;
            return TryAsync(async () =>
            {
                await Task.Delay(MillisecondsDelay);
                if (inMemoryPersistence.Filter(x => x.Uid == a.Uid).Any())
                    inMemoryPersistence.Remove(content);
                else
                    throw new InvalidOperationException("Content not found"); // do things here that can raise an exceptiion

                return content; // if you don't havr an exception you will have tha result (see tests)              
            }
            );
        }


        public TryAsync<T> Get(string uid, CancellationToken token)
        {
            return TryAsync(async () =>
            {
                await Task.Delay(MillisecondsDelay);
                if (inMemoryPersistence.Filter(x => x.Uid == uid).Any())
                    return inMemoryPersistence.Filter(x => x.Uid == uid).First();
                throw new InvalidOperationException("Content not found");
            }
           );
        }

        public TryAsync<IEnumerable<T>> Get(IEnumerable<string> uids, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public TryAsync<IEnumerable<T>> Get(System.Linq.Expressions.Expression<Func<T, bool>> predicate, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public TryAsync<IEnumerable<T>> GetAll(CancellationToken token)
        {
            return TryAsync(async () =>
            {
                await Task.Delay(MillisecondsDelay);
                //return inMemoryPersistence.Map(x => x); SAME AS BELOW
                return from x in inMemoryPersistence select x;
            }
           );
        }


        public TryAsync<T> Save(TryAsync<T> content, CancellationToken token)
        {
            var res = content.Bind((t) => Save(t, token));
            return res;
        }

        public TryAsync<T> Save(T content, CancellationToken token)
        {
            return InternSave(content);
        }

        public TryAsync<Either<IEnumerable<T>, IEnumerable<T>>> Save(IEnumerable<T> contents, CancellationToken token)
        {
            throw new NotImplementedException();
        }


        // we have to write that function to avoid signature confusion
        TryAsync<T> InternSave(T content)
        {
            return TryAsync(async () =>
            {
                await Task.Delay(MillisecondsDelay);
                // TODO: transform Uid into a value object to handle all cases of non valid Uid
                if (content.Uid == Content.Void().Uid)
                    throw new Exception("cannot save a void content");
                inMemoryPersistence.Add(content);
                return content
                ;
            }
         );
        }

        public EitherAsync<T, T> Delete2(T content, CancellationToken token)
        {
            var task = Delete(content, token).ToEither();
            return task.BiMapAsync<T, T>((x) => x, (err) => Task.FromResult(content));
        }

        public Aff<IEnumerable<T>> Delete2(IEnumerable<T> contents, CancellationToken token)
        {

            IEnumerable<Aff<T>> tasks = from x in contents
                                        select Delete2(x, token).ToAff(x => new IcpError());

            Aff<IEnumerable<T>> resultOfTheEffectAsync = tasks.SequenceSerial();

            // let's continue
            // now , we can Match or BiMap on resultOfTheEffectAsync to do what you want

            // I can write a toEitherAsync  on resultOfTheEffectAsync (which is of type Aff) if you prefer

            return resultOfTheEffectAsync;
        }

     

        public TryAsync<Either<IEnumerable<T>, IEnumerable<T>>> Delete(IEnumerable<T> contents, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        private record IcpError : Error
        {
            public override string Message => throw new NotImplementedException();

            public override bool IsExceptional => throw new NotImplementedException();

            public override bool IsExpected => throw new NotImplementedException();

            public override bool Is<E>()
            {
                throw new NotImplementedException();
            }

            public override ErrorException ToErrorException()
            {
                throw new NotImplementedException();
            }
        }
    }
    //  {            }

}
