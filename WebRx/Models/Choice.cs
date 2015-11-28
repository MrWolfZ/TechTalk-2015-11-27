using System;

namespace WebRx.Models
{
  public sealed class Choice<U, V>
  {
    private readonly bool isU;
    private readonly U u;
    private readonly V v;

    public Choice(U u)
    {
      this.isU = true;
      this.u = u;
    }

    public Choice(V v)
    {
      this.isU = false;
      this.v = v;
    }

    public T Get<T>(Func<U, T> onU, Func<V, T> onV) => this.isU ? onU(this.u) : onV(this.v);
  }
}