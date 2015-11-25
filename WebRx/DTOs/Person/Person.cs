using System.Runtime.Serialization;

namespace WebRx.DTOs.Person
{
  [DataContract]
  public class Person
  {
    [DataMember]
    public string ID { get; set; }

    [DataMember]
    public string FirstName { get; set; }

    [DataMember]
    public string LastName { get; set; }
  }
}
