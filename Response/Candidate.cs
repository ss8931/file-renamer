using System.Runtime.Serialization;

namespace Response;

[DataContract]
public class Candidate
{
	[DataMember(Name = "output")]
	public string Output { get; set; } = "";

	[DataMember(Name = "safetyRatings")]
	public List<SafetyRating> SafetyRatings { get; set; } = new();
}
