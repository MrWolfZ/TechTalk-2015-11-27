﻿using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using WebRx.Boundary;

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

    public abstract Task<TResponse> ProcessRequestAsync(TRequest request);

    private async void ProcessRequest(TRequest request)
    {
      var response = await this.ProcessRequestAsync(request);
      this.boundary.PublishResponse(response);
    }
  }
}