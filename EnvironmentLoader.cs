public class EnvironmentLoader
{

	private static FileInfo filePath = new FileInfo("/Users/srikarsundaram/bin/.file-renamer/.env");

	public static void Load()
	{
		if (!filePath.Exists)
			return;
		using var stream = filePath.OpenText();

		var lines = stream.ReadToEnd().Split(
			'\n',
			StringSplitOptions.RemoveEmptyEntries);

		foreach (var line in lines)
		{
			var parts = line.Split(
				'=',
				StringSplitOptions.RemoveEmptyEntries);

			if (parts.Length != 2)
				continue;

			Environment.SetEnvironmentVariable(parts[0], parts[1]);
		}
	}
}
