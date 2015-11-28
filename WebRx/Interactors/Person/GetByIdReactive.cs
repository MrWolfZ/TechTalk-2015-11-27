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
              .Select(c => c.Get(p => new GetByIdResponse(p), e => new GetByIdResponse(new[] { e })));

    private static Choice<Models.Person.Person, Error> NotFound(string id) =>
      new Choice<Models.Person.Person, Error>(new Error(ErrorKind.NotFound, $"The person with ID \"{id}\" does not exist!"));

    private static Choice<Models.Person.Person, Error> UnknownError(Exception ex, string id) =>
      new Choice<Models.Person.Person, Error>(new Error(ErrorKind.Unknown, $"Error while fetching the person with ID \"{id}\": {ex}"));

    private IObservable<Choice<Models.Person.Person, Error>> GetPersonById(string id) =>
      Observable.FromAsync(() => this.personRepository.Get(id))
                .Select(p => new Choice<Models.Person.Person, Error>(p))
                .Catch((KeyNotFoundException ex) => Observable.Return(NotFound(id)))
                .Catch((Exception ex) => Observable.Return(UnknownError(ex, id)));
  }
}