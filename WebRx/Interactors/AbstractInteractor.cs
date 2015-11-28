using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using WebRx.Boundary;
using WebRx.Models;

namespace WebRx.Interactors
{
  public abstract class AbstractInteractor<TRequest, TResponse> : IInteractor, IDisposable
  {
    private readonly IReactiveBoundary boundary;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    protected AbstractInteractor(IReactiveBoundary boundary)
    {
      this.boundary = boundary;
    }

    public void Dispose()
    {
      this.disposables.Dispose();
    }

    public void Activate()
    {
      this.disposables.Add(this.boundary.Requests.OfType<TRequest>().Subscribe(this.ProcessRequest));
    }

    public abstract Task<Choice<TResponse, IImmutableList<Error>>> ProcessRequestAsync(TRequest request);

    protected Choice<TResponse, IImmutableList<Error>> Success(TResponse response) =>
      new Choice<TResponse, IImmutableList<Error>>(response);

    protected Choice<TResponse, IImmutableList<Error>> Error(Exception ex) =>
      new Choice<TResponse, IImmutableList<Error>>(ImmutableList.Create(new Error(ErrorKind.Unknown, ex.Message)));

    protected Choice<TResponse, IImmutableList<Error>> Error(Error e) =>
      new Choice<TResponse, IImmutableList<Error>>(ImmutableList.Create(e));

    protected Choice<TResponse, IImmutableList<Error>> Error(IEnumerable<Error> e) =>
      new Choice<TResponse, IImmutableList<Error>>(e.ToImmutableList());

    private async void ProcessRequest(TRequest request)
    {
      var response = await this.ProcessRequestAsync(request);
      this.boundary.PublishResponse(response);
    }
  }
}