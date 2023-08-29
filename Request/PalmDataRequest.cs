using System.Runtime.Serialization;

namespace Request;

[DataContract]
public class PalmDataRequest
{
	[DataMember(Name = "prompt")]
	public Prompt Prompt { get; set; } = new();

	[DataMember(Name = "temperature")]
	public double Temperature { get; set; } = 0.8;

	[DataMember(Name = "top_k")]
	public int TopK { get; set; } = 40;

	[DataMember(Name = "top_p")]
	public double TopP { get; set; } = 0.95;

	[DataMember(Name = "candidate_count")]
	public int CandidateCount { get; set; } = 1;

	[DataMember(Name = "max_output_tokens")]
	public int MaxOutputTokens { get; set; } = 1024;

	[DataMember(Name = "stop_sequences")]
	public List<string> StopSequences { get; set; } = new();

	[DataMember(Name = "safety_settings")]
	public List<SafetySetting> SafetySettings { get; set; } = new() {
		new() { Category = "HARM_CATEGORY_DEROGATORY", Threshold = 1 },
		new() { Category = "HARM_CATEGORY_TOXICITY", Threshold = 1 },
		new() { Category = "HARM_CATEGORY_VIOLENCE", Threshold = 2 },
		new() { Category = "HARM_CATEGORY_SEXUAL", Threshold = 2 },
		new() { Category = "HARM_CATEGORY_MEDICAL", Threshold = 2 },
		new() { Category = "HARM_CATEGORY_DANGEROUS", Threshold = 2 }
	};
}
