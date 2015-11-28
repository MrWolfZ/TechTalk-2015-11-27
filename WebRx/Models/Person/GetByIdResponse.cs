namespace WebRx.Models.Person
{
  public class GetByIdResponse
  {
    public GetByIdResponse(Person p)
    {
      this.Person = p;
    }

    public Person Person { get; }
  }
}