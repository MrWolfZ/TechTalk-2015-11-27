using System;
using System.Collections.Immutable;

namespace WebRx.Models
{
  public sealed class BusinessException : Exception
  {
    public BusinessException(IImmutableList<Error> errors)
    {
      this.Errors = errors;
    }

    public IImmutableList<Error> Errors { get; }
  }
}