using System.Runtime.Serialization;

namespace Response;

[DataContract]
public class PalmDataResponse
{
	[DataMember(Name = "candidates")]
	public List<Candidate> Candidates { get; set; } = new();
}
