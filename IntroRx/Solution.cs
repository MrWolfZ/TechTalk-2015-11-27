using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace IntroRx
{
  class Solution
  {
    private static event EventHandler Event;

    public static void Run()
    {
      // creating observables
      var created = Observable.Create<int>(obs =>
      {
        obs.OnNext(2);
        obs.OnCompleted();
        return Disposable.Empty;
      });

      // lazy, infinite
      var generated = Observable.Generate(
        0,
        _ => true,
        i => i + 1,
        i => i
      );

      var subject = new Subject<int>();
      subject.OnNext(2);

      // transforming into observables
      var fromAsync = Observable.FromAsync(() => Task.Factory.StartNew(() => 2));
      var fromEvent = Observable.FromEventPattern(h => Event += h, h => Event -= h);

      // subscribing to observables
      var subscription = created.Subscribe(Console.WriteLine);
      subscription.Dispose(); // redundant, since source sequence terminates immediately

      // reducing sequences
      var filtered = generated.Where(i => i % 2 == 0);
      filtered.Take(10).Subscribe(Console.WriteLine);

      generated.Take(10).Subscribe(Console.WriteLine);

      // transforming sequences
      var transformed = generated.Select(i => (double)i / 2);
      transformed.Take(10).Subscribe(Console.WriteLine);

      // aggregating sequences
      var aggregated = generated.Take(10).Aggregate((sum, i) => sum + i);
      aggregated.Subscribe(Console.WriteLine);

      Console.ReadKey();
    }
  }
}
