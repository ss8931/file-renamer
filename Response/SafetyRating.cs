using System.Runtime.Serialization;

namespace Response;

[DataContract]
public class SafetyRating
{
	[DataMember(Name = "category")]
	public string Category { get; set; } = "";

	[DataMember(Name = "probability")]
	public string Probability { get; set; } = "";
}
