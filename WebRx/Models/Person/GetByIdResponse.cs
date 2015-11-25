using System.Collections.Generic;
using System.Collections.Immutable;

namespace WebRx.Models.Person
{
  public class GetByIdResponse
  {
    public GetByIdResponse(Person p)
    {
      this.Person = p;
      this.Errors = ImmutableList<Error>.Empty;
    }

    public GetByIdResponse(IEnumerable<Error> errors)
    {
      this.Errors = errors.ToImmutableList();
    }

    public Person Person { get; }

    public IImmutableList<Error> Errors { get; }
  }
}
