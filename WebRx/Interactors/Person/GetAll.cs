using System.Collections.Immutable;
using System.Threading.Tasks;
using WebRx.Boundary;
using WebRx.Data.Person;
using WebRx.Models;
using WebRx.Models.Person;

namespace WebRx.Interactors.Person
{
  public class GetAll : AbstractInteractor<GetAllRequest, GetAllResponse>
  {
    private readonly IPersonRepository personRepository;

    public GetAll(IReactiveBoundary boundary, IPersonRepository personRepository) : base(boundary)
    {
      this.personRepository = personRepository;
    }

    public override async Task<Choice<GetAllResponse, IImmutableList<Error>>> ProcessRequestAsync(GetAllRequest request)
    {
      var persons = await this.personRepository.GetAll();
      return this.Success(new GetAllResponse(persons));
    }
  }
}