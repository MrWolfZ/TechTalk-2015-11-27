using System;
using System.Reactive.Subjects;

namespace ConditionalVotingRx
{
  class Program
  {
    static void Main()
    {
      var subscriptionSeq = new Subject<Subscription>();
      var eventSeq = new Subject<Event>();
      var voteSeq = new Subject<Vote>();

      var engine = new VotingEngine(subscriptionSeq, eventSeq, voteSeq);

      var disposable = engine.VotingResults.Subscribe(Console.WriteLine);

      subscriptionSeq.OnNext(new Subscription(e => true));
      eventSeq.OnNext(new Event("1", MessageType.A));
      voteSeq.OnNext(new Vote("1", true));

      Console.ReadKey();

      subscriptionSeq.OnNext(new Subscription(e => e.Type == MessageType.A));
      eventSeq.OnNext(new Event("2", MessageType.A));
      voteSeq.OnNext(new Vote("2", true));
      voteSeq.OnNext(new Vote("2", true));

      Console.ReadKey();

      eventSeq.OnNext(new Event("3", MessageType.A));
      voteSeq.OnNext(new Vote("3", true));

      Console.ReadKey();

      eventSeq.OnNext(new Event("4", MessageType.B));
      voteSeq.OnNext(new Vote("4", true));

      Console.ReadKey();
      disposable.Dispose();
    }
  }
}
