namespace WebRx.Models.Person
{
  public sealed class Person
  {
    public Person(string id, string firstName, string lastName)
    {
      this.ID = id;
      this.FirstName = firstName;
      this.LastName = lastName;
    }

    public string ID { get; }

    public string FirstName { get; }

    public string LastName { get; }
  }
}
