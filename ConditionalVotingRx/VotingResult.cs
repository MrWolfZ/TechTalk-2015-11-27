using System.Collections.Generic;
using System.Collections.Immutable;

namespace ConditionalVotingRx
{
  public class VotingResult
  {
    public VotingResult(string id, IEnumerable<bool> votes, int nrOfExpectedVotes)
    {
      this.ID = id;
      this.NrOfExpectedVotes = nrOfExpectedVotes;
      this.Votes = votes.ToImmutableList();
    }

    public string ID { get; }

    public IImmutableList<bool> Votes { get; }

    public int NrOfExpectedVotes { get; }

    public override string ToString()
    {
      return $"ID: {this.ID}, Votes: [{string.Join(",", this.Votes)}], NrOfExpectedVotes: {this.NrOfExpectedVotes}";
    }
  }
}