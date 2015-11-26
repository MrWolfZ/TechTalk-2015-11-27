using System.Threading.Tasks;
using WebRx.Boundary;
using WebRx.Data.Person;
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

    public override async Task<GetAllResponse> ProcessRequestAsync(GetAllRequest request)
    {
      var persons = await this.personRepository.GetAll();
      return new GetAllResponse(persons);
    }
  }
}
