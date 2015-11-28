using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

    public abstract IObservable<Choice<TResponse, IImmutableList<Error>>> TransformRequests(IObservable<TRequest> requests);

    protected IObservable<Choice<TResponse, IImmutableList<Error>>> Success(TResponse response) =>
      Observable.Return(new Choice<TResponse, IImmutableList<Error>>(response));

    protected IObservable<Choice<TResponse, IImmutableList<Error>>> Error(Exception ex) =>
      Observable.Return(new Choice<TResponse, IImmutableList<Error>>(ImmutableList.Create(new Error(ErrorKind.Unknown, ex.Message))));

    protected IObservable<Choice<TResponse, IImmutableList<Error>>> Error(Error e) => 
      Observable.Return(new Choice<TResponse, IImmutableList<Error>>(ImmutableList.Create(e)));

    protected IObservable<Choice<TResponse, IImmutableList<Error>>> Error(IEnumerable<Error> e) =>
      Observable.Return(new Choice<TResponse, IImmutableList<Error>>(e.ToImmutableList()));
  }
}