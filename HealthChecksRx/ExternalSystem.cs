using System;
using System.Reactive.Linq;

namespace HealthChecksRx
{
  class ExternalSystem : IExternalSystem
  {
    private readonly Random r;

    public ExternalSystem(string name, Random r)
    {
      this.Name = name;
      this.r = r;
    }

    public string Name { get; }

    public IObservable<HealthCheck> ObserveHealth()
    {
      // 10% probability to go down every 1-3 seconds
      return Observable.Return(new HealthCheck(this.Name, true));
    }
  }
}
