using System;

namespace ConditionalVotingRx
{
  public class Subscription
  {
    public Subscription(Func<Event, bool> predicate)
    {
      this.Predicate = predicate;
    }

    public Func<Event, bool> Predicate { get; }
  }
}