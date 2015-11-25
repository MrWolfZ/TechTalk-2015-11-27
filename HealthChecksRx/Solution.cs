using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace HealthChecksRx
{
  class Solution
  {
    private static Random r = new Random();

    public static void Run()
    {
      var systems = new[] { "A", "B", "C" }.Select(CreateExternalSystem);
      var observables = systems.Select(s => s.ObserveHealth()).Select(obs => obs.DistinctUntilChanged(c => c.IsAvailable)).ToList();
      var disposable = new CompositeDisposable();

      // observe independently
      disposable.Add(new CompositeDisposable(observables.Select(c => c.Subscribe(PrintHealthCheck))));

      // merge
      var merged = observables.Aggregate((l, r) => l.Merge(r));
      disposable.Add(merged.Subscribe(PrintHealthCheck));

      // combine
      var combined = observables
        .Aggregate(Observable.Return(Enumerable.Empty<HealthCheck>()), (agg, obs) => agg.CombineLatest(obs, (checks, check) => checks.Concat(new[] { check })));

      var scan = merged.Scan(ImmutableDictionary<string, bool>.Empty, (d, check) => d.SetItem(check.ExternalSystemName, check.IsAvailable));
      
      disposable.Add(combined.Subscribe(e => Console.WriteLine("Combined: " + string.Join(", ", e.Select(c => $"{c.ExternalSystemName}={c.IsAvailable}")))));
      disposable.Add(scan.Subscribe(d => Console.WriteLine("Scanned: " + string.Join(", ", d.Select(p => $"{p.Key}={p.Value}")))));

      Console.ReadKey();
      disposable.Dispose();
    }

    private static void PrintHealthCheck(HealthCheck check)
    {
      Console.WriteLine($"System \"{check.ExternalSystemName}\" reported health status: {check.IsAvailable}");
    }

    private static IExternalSystem CreateExternalSystem(string name) => new ExternalSystemSolution(name, r);
  }
}
