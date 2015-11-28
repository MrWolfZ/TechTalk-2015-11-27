﻿using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

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
                                 .SelectMany(list => list.OfType<IObservable<TResponse>>())
                                 .SelectMany(s => s)
                                 .Where(predicate)
                                 .FirstAsync();

      var responseAsync = this.responses
                              .OfType<TResponse>()
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

    public void PublishResponse<TResponse>(TResponse response) => this.responses.OnNext(response);
    public void PublishResponseStream<TResponse>(IObservable<TResponse> rs) where TResponse : class => this.responseStreams.OnNext(rs);
  }
}