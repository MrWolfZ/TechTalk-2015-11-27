using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;

namespace ConditionalVotingRx
{
  public class VotingEngine
  {
    private readonly IObservable<Subscription> subscriptionSeq;
    private readonly IObservable<Event> eventSeq;
    private readonly IObservable<Vote> voteSeq;

    public VotingEngine(IObservable<Subscription> subscriptionSeq, IObservable<Event> eventSeq, IObservable<Vote> voteSeq)
    {
      this.subscriptionSeq = subscriptionSeq;
      this.eventSeq = eventSeq;
      this.voteSeq = voteSeq;
    }

    public IObservable<VotingResult> VotingResults =>
      this.eventSeq
          .CombineLatest(
            this.Subscriptions,
            (e, subscribers) => new { e.ID, NrOfVoters = subscribers.Count(s => s.Predicate(e)) })
          .DistinctUntilChanged(a => a.ID)
          .SelectMany(a => this.CreateVoteCollector(a.ID, a.NrOfVoters));

    private IObservable<ICollection<Subscription>> Subscriptions =>
      this.subscriptionSeq
          .Scan(ImmutableList<Subscription>.Empty, (list, subscription) => list.Add(subscription))
          .StartWith(ImmutableList<Subscription>.Empty);

    private IObservable<VotingResult> CreateVoteCollector(string id, int nrOfVotes)
    {
      return Observable.Return(new VotingResult(id, Enumerable.Empty<bool>(), nrOfVotes));
    }
  }
}