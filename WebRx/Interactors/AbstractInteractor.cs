using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using WebRx.Boundary;

namespace WebRx.Interactors
{
  public abstract class AbstractInteractor<TRequest, TResponse> : IDisposable
  {
    private readonly IReactiveBoundary boundary;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    public AbstractInteractor(IReactiveBoundary boundary)
    {
      this.boundary = boundary;
      this.disposables.Add(this.boundary.Requests.OfType<TRequest>().Subscribe(this.ProcessRequest));
    }

    public void Dispose()
    {
      this.disposables.Dispose();
    }

    public abstract Task<TResponse> ProcessRequestAsync(TRequest request);

    private async void ProcessRequest(TRequest request)
    {
      var response = await this.ProcessRequestAsync(request);
      this.boundary.PublishResponse(response);
    }
  }
}
