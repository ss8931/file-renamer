using System.Runtime.Serialization;

namespace Request;

[DataContract]
public class Prompt
{
	[DataMember(Name = "text")]
	public string Text { get; set; } = "";
}
