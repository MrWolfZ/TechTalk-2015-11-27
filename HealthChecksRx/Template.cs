﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace HealthChecksRx
{
  class Template
  {
    private static readonly Random R = new Random();

    public static void Run()
    {
      var systems = new[] { "A", "B", "C" }.Select(CreateExternalSystem);
      var observables = systems.Select(s => s.ObserveHealth()).Select(obs => obs.DistinctUntilChanged(c => c.IsAvailable)).ToList();
      var disposable = new CompositeDisposable();

      // observe independently
      disposable.Add(new CompositeDisposable(observables.Select(c => c.Subscribe(PrintHealthCheck))));

      // merge
      var merged = Observable.Empty<HealthCheck>();
      disposable.Add(merged.Subscribe(PrintHealthCheck));

      // combine
      var combined = Observable.Empty<IEnumerable<HealthCheck>>();

      disposable.Add(combined.Subscribe(e => Console.WriteLine("Combined: " + string.Join(", ", e.Select(c => $"{c.ExternalSystemName}={c.IsAvailable}")))));

      Console.ReadKey();
      disposable.Dispose();
    }

    private static void PrintHealthCheck(HealthCheck check)
    {
      Console.WriteLine($"System \"{check.ExternalSystemName}\" reported health status: {check.IsAvailable}");
    }

    private static IExternalSystem CreateExternalSystem(string name) => new ExternalSystem(name, R);
  }
}
