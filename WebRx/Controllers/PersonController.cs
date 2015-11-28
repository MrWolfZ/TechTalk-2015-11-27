using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using WebRx.Boundary;
using WebRx.Models;
using WebRx.Models.Person;
using Person = WebRx.DTOs.Person.Person;

namespace WebRx.Controllers
{
  [Route("api/[controller]")]
  public class PersonController : Controller
  {
    public PersonController(IReactiveBoundary boundary)
    {
      this.Boundary = boundary;
    }

    private IReactiveBoundary Boundary { get; }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
      var response = await this.Boundary.ProcessRequest<GetAllRequest, GetAllResponse>(new GetAllRequest());
      return this.Json(response.Persons.Select(model => new Person { ID = model.ID, FirstName = model.FirstName, LastName = model.LastName }));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
      try
      {
        var response = await this.Boundary.ProcessRequest<GetByIdRequest, GetByIdResponse>(new GetByIdRequest(id));
        return this.Json(new Person { ID = response.Person.ID, FirstName = response.Person.FirstName, LastName = response.Person.LastName });
      }
      catch (BusinessException ex)
      {
        var notFound = ex.Errors.FirstOrDefault(e => e.Kind == ErrorKind.NotFound);
        if (notFound != null)
        {
          return this.HttpNotFound(notFound.Message);
        }

        return new HttpStatusCodeResult(500);
      }
    }

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