namespace WebRx.Models.Person
{
  public class GetByIdRequest
  {
    public GetByIdRequest(string id)
    {
      this.ID = id;
    }

    public string ID { get; }
  }
}
