namespace ConditionalVotingRx
{
  public class Event
  {
    public Event(string id, MessageType type)
    {
      this.ID = id;
      this.Type = type;
    }

    public string ID { get; } 

    public MessageType Type { get; }
  }
}