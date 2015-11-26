namespace ConditionalVotingRx
{
  public class Vote
  {
    public Vote(string id, bool result)
    {
      this.ID = id;
      this.Result = result;
    }

    public string ID { get; } 

    public bool Result { get; }
  }
}