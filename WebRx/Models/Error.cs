using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebRx.Models
{
  public sealed class Error
  {
    public Error(ErrorKind kind, string message = "")
    {
      this.Kind = kind;
      this.Message = message;
    }

    public ErrorKind Kind { get; }

    public string Message { get; }
  }
}
