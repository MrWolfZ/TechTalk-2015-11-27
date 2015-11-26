using System.Collections.Generic;
using System.Threading.Tasks;
using WebRx.Boundary;
using WebRx.Data.Person;
using WebRx.Models;
using WebRx.Models.Person;

namespace WebRx.Interactors.Person
{
  public sealed class GetById : AbstractInteractor<GetByIdRequest, GetByIdResponse>
  {
    private readonly IPersonRepository personRepository;

    public GetById(IReactiveBoundary boundary, IPersonRepository personRepository) : base(boundary)
    {
      this.personRepository = personRepository;
    }

    public override async Task<GetByIdResponse> ProcessRequestAsync(GetByIdRequest request)
    {
      try
      {
        var person = await this.personRepository.Get(request.ID);
        return new GetByIdResponse(person);
      }
      catch (KeyNotFoundException)
      {
        return new GetByIdResponse(new[] { new Error(ErrorKind.NotFound, $"The person with ID \"{request.ID}\" does not exist!") });
      }
    }
  }
}
