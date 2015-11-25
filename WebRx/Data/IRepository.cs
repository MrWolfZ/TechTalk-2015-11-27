using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebRx.Data
{
  public interface IRepository<TEntity>
  {
    Task<IEnumerable<TEntity>> GetAll();

    Task<TEntity> Get(string id);
  }
}
