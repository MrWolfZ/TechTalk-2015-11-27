using System;

namespace HealthChecksRx
{
  interface IExternalSystem
  {
    string Name { get; }

    IObservable<HealthCheck> ObserveHealth();
  }
}
