using System;
using System.Threading.Tasks;

namespace WebRx.Boundary
{
  public interface IReactiveBoundary
  {
    IObservable<object> Requests { get; }

    Task<TResponse> ProcessRequest<TRequest, TResponse>(TRequest request, Func<TResponse, bool> predicate = null);

    void PublishResponse<TResponse>(TResponse response);
    void PublishResponseStream<TResponse>(IObservable<TResponse> rs) where TResponse : class;
  }
}
