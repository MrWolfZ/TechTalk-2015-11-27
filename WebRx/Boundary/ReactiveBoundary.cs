using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace WebRx.Boundary
{
  public class ReactiveBoundary : IReactiveBoundary
  {
    private readonly Subject<object> requests = new Subject<object>();
    private readonly Subject<object> responses = new Subject<object>();

    public IObservable<object> Requests => this.requests;

    public async Task<TResponse> ProcessRequest<TRequest, TResponse>(TRequest request, Func<TResponse, bool> predicate = null)
    {
      predicate = predicate ?? (_ => true);
      var response = this.responses.OfType<TResponse>().Where(predicate).FirstAsync().Replay(1);
      response.Connect();
      this.requests.OnNext(request);
      return await response;
    }

    public void PublishResponse<TResponse>(TResponse response) => this.responses.OnNext(response);
  }
}
