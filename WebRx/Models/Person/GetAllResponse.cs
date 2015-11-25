using System.Collections.Generic;
using System.Collections.Immutable;

namespace WebRx.Models.Person
{
  public sealed class GetAllResponse
  {
    public GetAllResponse(IEnumerable<Person> persons)
    {
      this.Persons = persons.ToImmutableList();
    }

    public IImmutableList<Person> Persons { get; }
  }
}
