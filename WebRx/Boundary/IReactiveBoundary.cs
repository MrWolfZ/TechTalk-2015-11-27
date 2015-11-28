using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using WebRx.Models;

namespace WebRx.Boundary
{
  public interface IReactiveBoundary
  {
    IObservable<object> Requests { get; }

    Task<TResponse> ProcessRequest<TRequest, TResponse>(TRequest request, Func<TResponse, bool> predicate = null);

    void PublishResponse<TResponse>(Choice<TResponse, IImmutableList<Error>> response);
    void PublishResponseStream<TResponse>(IObservable<Choice<TResponse, IImmutableList<Error>>> rs) where TResponse : class;
  }
}
