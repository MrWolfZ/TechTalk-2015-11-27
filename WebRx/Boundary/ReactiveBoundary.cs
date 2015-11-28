using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using WebRx.Models;

namespace WebRx.Boundary
{
  public class ReactiveBoundary : IReactiveBoundary, IDisposable
  {
    private readonly Subject<object> requests = new Subject<object>();
    private readonly Subject<object> responses = new Subject<object>();
    private readonly Subject<IObservable<object>> responseStreams = new Subject<IObservable<object>>();
    private readonly IConnectableObservable<ImmutableList<IObservable<object>>> streams;
    private readonly IDisposable disposable;

    public ReactiveBoundary()
    {
      this.streams = this.responseStreams
                         .Scan(ImmutableList<IObservable<object>>.Empty, (list, s) => list.Add(s))
                         .StartWith(ImmutableList<IObservable<object>>.Empty)
                         .Replay(1);

      this.disposable = this.streams.Connect();
    }

    public IObservable<object> Requests => this.requests.AsObservable();

    public void Dispose()
    {
      this.disposable.Dispose();
    }

    public async Task<TResponse> ProcessRequest<TRequest, TResponse>(TRequest request, Func<TResponse, bool> predicate = null)
    {
      predicate = predicate ?? (_ => true);
      var responseReactive = this.streams
                                 .SelectMany(list => list.OfType<IObservable<Choice<TResponse, IImmutableList<Error>>>>())
                                 .SelectMany(seq => seq)
                                 .SelectMany(c => c.Get(Observable.Return, errors => Observable.Throw<TResponse>(new BusinessException(errors))))
                                 .Where(predicate)
                                 .FirstAsync();

      var responseAsync = this.responses
                              .OfType<Choice<TResponse, IImmutableList<Error>>>()
                              .SelectMany(c => c.Get(Observable.Return, errors => Observable.Throw<TResponse>(new BusinessException(errors))))
                              .Where(predicate)
                              .FirstAsync();

      var response = responseReactive.Amb(responseAsync)
                                     .Replay(1);

      using (response.Connect())
      {
        this.requests.OnNext(request);
        return await response;
      }
    }

    public void PublishResponse<TResponse>(Choice<TResponse, IImmutableList<Error>> response) => this.responses.OnNext(response);
    public void PublishResponseStream<TResponse>(IObservable<Choice<TResponse, IImmutableList<Error>>> rs) where TResponse : class => this.responseStreams.OnNext(rs);
  }
}