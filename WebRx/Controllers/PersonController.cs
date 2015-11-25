using Microsoft.AspNet.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using WebRx.Boundary;
using WebRx.Models.Person;
using WebRx.Models;

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
      return Json(response.Persons.Select(model => new DTOs.Person.Person { ID = model.ID, FirstName = model.FirstName, LastName = model.LastName }));
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
      var response = await this.Boundary.ProcessRequest<GetByIdRequest, GetByIdResponse>(new GetByIdRequest(id));

      var notFound = response.Errors.FirstOrDefault(e => e.Kind == ErrorKind.NotFound);
      if (notFound != null)
      {
        return new HttpNotFoundObjectResult(notFound.Message);
      }

      return Json(new DTOs.Person.Person { ID = response.Person.ID, FirstName = response.Person.FirstName, LastName = response.Person.LastName });
    }
    
    [HttpPost]
    public void Post([FromBody]string value)
    {
      // TODO
    }
    
    [HttpPut("{id}")]
    public void Put(int id, [FromBody]string value)
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
