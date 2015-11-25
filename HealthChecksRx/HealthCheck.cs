namespace HealthChecksRx
{
  class HealthCheck
  {
    public HealthCheck(string externalSystemName, bool isAvailable)
    {
      this.ExternalSystemName = externalSystemName;
      this.IsAvailable = isAvailable;
    }

    public string ExternalSystemName { get; }

    public bool IsAvailable { get; }
  }
}
