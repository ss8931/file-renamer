using Log;
using Models;

internal class Program
{
	private Options Options;
	private Logger Logger;

	private Program(Options options, Logger logger)
	{
		Options = options;
		Logger = logger;
	}

	private static async Task Main(string[] args)
	{
		EnvironmentLoader.Load(); // Load .env file

		var options = new Options();
		var logger = new Logger(options);

		var program = new Program(options, logger);

		if (args.Length < 1) return;

		if (args[0] == "rename")
		{
			await program.RenameFiles(args[1..]);
		}
		else if (args[0] == "add")
		{
			await program.AddExample(args[1..]);
		}
		else
		{
			program.PrintUsage();
		}
	}

	private async Task AddExample(string[] args)
	{
		string oldName = args[0];
		string newName = args[1];
		await AddExample(oldName, newName);
	}

	private async Task AddExample(string oldName, string newName)
	{
		var file = new FileInfo(Options.PromptFile);
		using var stream = file.AppendText();
		oldName = Path.GetFileName(oldName);
		newName = Path.GetFileName(newName);
		await Logger.Log($"Training {oldName} -> {newName}");
		await stream.WriteAsync($"\ninput: {oldName}\noutput: {newName}");
	}

	private void PrintUsage()
	{
		Console.WriteLine("Usage: file-renamer rename [-v | --verbose] [-n | --dry-run] [file] [file] ...");
		Console.WriteLine("Usage: file-renamer add [old file] [new file]");
		Console.WriteLine("Rename file using the palm client.");
	}

	private async Task RenameFiles(string[] args)
	{
		var client = new PalmClient(new FileInfo(Options.PromptFile), Logger, Options);

		var filenames = new HashSet<string>();

		var ignoreArgs = false;
		foreach (var arg in args)
		{
			if (arg.StartsWith("-") && !ignoreArgs)
			{
				if (arg == "--" && !ignoreArgs)
				{
					ignoreArgs = true;
					continue;
				}
				switch (arg.Trim('-'))
				{
					case "dry-run":
					case "n":
						Options.DryRun = true;
						continue;
					case "verbose":
					case "v":
						Options.Verbose = true;
						continue;
					case "t":
					case "train":
						Options.Train = true;
						continue;
					case "h":
					case "hide-no-changes":
						Options.HideNoChanges = true;
						continue;
					default:
						break;
				}
			}

			// must be file
			if (Options.Train || File.Exists(arg))
			{
				filenames.Add(arg);
				continue;
			};
			await Logger.LogError($"File {arg} is not a file and training mode off.");
		}


		var results = (await client.SummarizeFileNames(filenames))
			.OrderBy(result => result.newName);
		// check no newName is dupe
		// group by
		var grouped = results.GroupBy(result => result.newName).Where(group => group.Count() > 1);
		foreach (var group in grouped)
		{
			await Logger.LogError($"Duplicate file name {group.Key}");
			foreach (var (oldName, newName) in group)
			{
				await Logger.LogError($"  {oldName}");
			}
		}
		if (grouped.Any())
		{
			await Logger.LogError("Exiting due to duplicate file names.");
			return;
		}

		foreach (var (oldName, newName) in results)
		{
			if (oldName == newName)
			{
				if (!Options.HideNoChanges)
					await Logger.Log($"{oldName} -> {newName} (no change)");
				continue;
			};

			await Logger.Log($"{oldName} -> {newName}");

			if (Options.Train)
			{
				await Logger.Log("Train? [[y]es/[n]o/[N]ever/[m]anual]");
				var response = Console.ReadLine();
				if (response == "y")
				{
					await AddExample(oldName, newName);
				}

				if (response == "N")
				{
					Options.Train = false;
					continue;
				}

				if (response == "m")
				{
					// ask user to manualy put in the new name
					await Logger.Log("Enter the actual basename (directories will be ignored):");
					var manualName = Console.ReadLine()?.Trim();
					if (string.IsNullOrWhiteSpace(manualName))
					{
						await Logger.LogError("Invalid name. Skipping.");
						continue;
					}
					await AddExample(oldName, manualName);
				}

			}

			if (File.Exists(newName) && oldName == newName)
			{
				await Logger.LogError($"File {newName} already exists. Skipping.");
				continue;
			}

			if (!Options.DryRun && File.Exists(oldName))
				File.Move(oldName, newName);
		}
	}
}