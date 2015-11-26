using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebRx.Data.Person
{
  public class PersonRepository : IPersonRepository
  {
    private static readonly IDictionary<string, Models.Person.Person> Persons = new Dictionary<string, Models.Person.Person> {
      { "1", new Models.Person.Person("1", "Jon", "Doe") },
      { "2", new Models.Person.Person("2", "Jane", "Doe") },
      { "3", new Models.Person.Person("3", "Max", "Mustermann") },
      { "4", new Models.Person.Person("4", "Maria", "Mustermann") }
    };

    public async Task<Models.Person.Person> Get(string id)
    {
      await Task.Yield();
      if (!Persons.ContainsKey(id))
      {
        throw new KeyNotFoundException();
      }

      return Persons[id];
    }

    public async Task<IEnumerable<Models.Person.Person>> GetAll()
    {
      await Task.Yield();
      return Persons.Values;
    }
  }
}
