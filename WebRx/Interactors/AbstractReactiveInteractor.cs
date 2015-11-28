using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using WebRx.Boundary;
using WebRx.Models;

namespace WebRx.Interactors
{
  public abstract class AbstractReactiveInteractor<TRequest, TResponse> : IInteractor
    where TRequest : class
    where TResponse : class
  {
    private readonly IReactiveBoundary boundary;

    protected AbstractReactiveInteractor(IReactiveBoundary boundary)
    {
      this.boundary = boundary;
    }

    public void Activate()
    {
      var requests = this.boundary.Requests.OfType<TRequest>();
      var transformed = this.TransformRequests(requests);

      this.boundary.PublishResponseStream(transformed);
    }

    public abstract IObservable<TResponse> TransformRequests(IObservable<TRequest> requests);

    protected IObservable<Choice<TResponse, IEnumerable<Error>>> Error(Exception ex) =>
      Observable.Return(new Choice<TResponse, IEnumerable<Error>>(new[] { new Error(ErrorKind.Unknown, ex.Message) }));

    protected IObservable<Choice<TResponse, IEnumerable<Error>>> Error(Error e) => 
      Observable.Return(new Choice<TResponse, IEnumerable<Error>>(new[] { e }));

    protected IObservable<Choice<TResponse, IEnumerable<Error>>> Error(IEnumerable<Error> e) =>
      Observable.Return(new Choice<TResponse, IEnumerable<Error>>(e));
  }
}