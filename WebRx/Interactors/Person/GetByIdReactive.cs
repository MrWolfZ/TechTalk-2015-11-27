using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reactive.Linq;
using WebRx.Boundary;
using WebRx.Data.Person;
using WebRx.Models;
using WebRx.Models.Person;

namespace WebRx.Interactors.Person
{
  public sealed class GetByIdReactive : AbstractReactiveInteractor<GetByIdRequest, GetByIdResponse>
  {
    private readonly IPersonRepository personRepository;

    public GetByIdReactive(IReactiveBoundary boundary, IPersonRepository personRepository) : base(boundary)
    {
      this.personRepository = personRepository;
    }

    public override IObservable<Choice<GetByIdResponse, IImmutableList<Error>>> TransformRequests(IObservable<GetByIdRequest> requests) =>
      requests.Select(r => r.ID)
              .SelectMany(this.GetPersonById);

    private IObservable<Choice<GetByIdResponse, IImmutableList<Error>>> GetPersonById(string id) =>
      Observable.FromAsync(() => this.personRepository.Get(id))
                .SelectMany(p => this.Success(new GetByIdResponse(p)))
                .Catch((KeyNotFoundException ex) => this.Error(new Error(ErrorKind.NotFound, $"The person with ID \"{id}\" does not exist!")))
                .Catch((Exception ex) => this.Error(ex));
  }
}