using System;
using System.Reactive.Linq;

namespace HealthChecksRx
{
  class ExternalSystemSolution : IExternalSystem
  {
    private readonly Random r;

    public ExternalSystemSolution(string name, Random r)
    {
      this.Name = name;
      this.r = r;
    }

    public string Name { get; }

    public IObservable<HealthCheck> ObserveHealth()
    {
      // 10% probability to go down every 1-3 seconds
      return Observable.Generate(
        true,
        _ => true,
        _ => this.r.Next(9) > 0,
        b => new HealthCheck(this.Name, b),
        _ => TimeSpan.FromSeconds(this.r.NextDouble() * 2 + 1)
      ).Publish().RefCount().StartWith(new HealthCheck(this.Name, true));
    }
  }
}
