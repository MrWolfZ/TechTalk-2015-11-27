using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using WebRx.Models;

namespace WebRx.Controllers
{
  public abstract class AbstractController : Controller
  {
    protected async Task<IActionResult> Try(Func<Task<IActionResult>> f)
    {
      try
      {
        return await f();
      }
      catch (BusinessException ex)
      {
        return this.HandleErrors(ex.Errors);
      }
    }

    private IActionResult HandleErrors(IImmutableList<Error> errors)
    {
      var notFound = errors.FirstOrDefault(e => e.Kind == ErrorKind.NotFound);
      if (notFound != null)
      {
        return this.HttpNotFound(notFound.Message);
      }

      return new HttpStatusCodeResult(500);
    }
  }
}