using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthChecksRx
{
  interface IExternalSystem
  {
    string Name { get; }

    IObservable<HealthCheck> ObserveHealth();
  }
}
