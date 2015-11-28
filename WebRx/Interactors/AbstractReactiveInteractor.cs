using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using WebRx.Boundary;

namespace WebRx.Interactors
{
  public abstract class AbstractReactiveInteractor<TRequest, TResponse> : IInteractor, IDisposable
  {
    private readonly IReactiveBoundary boundary;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    protected AbstractReactiveInteractor(IReactiveBoundary boundary)
    {
      this.boundary = boundary;
    }

    public void Dispose()
    {
      this.disposables.Dispose();
    }

    public void Activate()
    {
      var requests = this.boundary.Requests.OfType<TRequest>();
      var transformed = this.TransformRequests(requests);
      this.disposables.Add(transformed.Subscribe(this.boundary.PublishResponse));
    }

    public abstract IObservable<TResponse> TransformRequests(IObservable<TRequest> requests);
  }
}