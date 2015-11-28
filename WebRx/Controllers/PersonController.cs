using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using WebRx.Boundary;
using WebRx.Models.Person;
using Person = WebRx.DTOs.Person.Person;

namespace WebRx.Controllers
{
  [Route("api/[controller]")]
  public class PersonController : AbstractController
  {
    public PersonController(IReactiveBoundary boundary)
    {
      this.Boundary = boundary;
    }

    private IReactiveBoundary Boundary { get; }

    [HttpGet]
    public Task<IActionResult> Get() => this.Try(
      async () =>
      {
        var response = await this.Boundary.ProcessRequest<GetAllRequest, GetAllResponse>(new GetAllRequest());
        return this.Json(response.Persons.Select(model => new Person { ID = model.ID, FirstName = model.FirstName, LastName = model.LastName }));
      });

    [HttpGet("{id}")]
    public Task<IActionResult> Get(string id) => this.Try(
      async () =>
      {
        var response = await this.Boundary.ProcessRequest<GetByIdRequest, GetByIdResponse>(new GetByIdRequest(id));
        return this.Json(new Person { ID = response.Person.ID, FirstName = response.Person.FirstName, LastName = response.Person.LastName });
      });

    [HttpPost]
    public void Post([FromBody] string value)
    {
      // TODO
    }

    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
      // TODO
    }

    [HttpDelete("{id}")]
    public void Delete(int id)
    {
      // TODO
    }
  }
}