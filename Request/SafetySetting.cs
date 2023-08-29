using System.Runtime.Serialization;

namespace Request;

[DataContract]
public class SafetySetting
{
	[DataMember(Name = "category")]
	public string Category { get; set; } = "";

	[DataMember(Name = "threshold")]
	public int Threshold { get; set; } = 0;
}