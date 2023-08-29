namespace Models;

public class Options
{
	public string BaseUrl => "https://generativelanguage.googleapis.com/v1beta2/models/text-bison-001:generateText";

	private bool dryRun = false;

	public bool DryRun
	{
		get => !Train && dryRun; // if we're training, we don't want to dry run
		set => dryRun = value;
	}
	public bool Train { get; set; } = false;
	public bool Verbose { get; set; } = false;

	public bool HideNoChanges { get; set; } = false;

	public string PromptFile { get; set; } = Environment.GetEnvironmentVariable("PROMPT_FILE") ?? throw new Exception("PROMPT_FILE environment variable not set.");
}