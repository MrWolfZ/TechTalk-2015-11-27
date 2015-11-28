using System;
using System.Collections.Generic;
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

    public override IObservable<GetByIdResponse> TransformRequests(IObservable<GetByIdRequest> requests) =>
      requests.Select(r => r.ID)
              .SelectMany(this.GetPersonById)
              .Select(c => c.Get(r => r, es => new GetByIdResponse(es)));

    private IObservable<Choice<GetByIdResponse, IEnumerable<Error>>> GetPersonById(string id) =>
      Observable.FromAsync(() => this.personRepository.Get(id))
                .Select(p => new Choice<GetByIdResponse, IEnumerable<Error>>(new GetByIdResponse(p)))
                .Catch((KeyNotFoundException ex) => this.Error(new Error(ErrorKind.NotFound, $"The person with ID \"{id}\" does not exist!")))
                .Catch((Exception ex) => this.Error(ex));
  }
}