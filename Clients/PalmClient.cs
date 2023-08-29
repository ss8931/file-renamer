
using System.Net.Http.Headers;
using Request;
using Flurl;
using Newtonsoft.Json;
using Response;
using System.Collections.Concurrent;
using System.Diagnostics;
using Log;
using Models;

public class PalmClient
{
	private HttpClient client = new HttpClient();

	private readonly FileInfo FileInfo;
	private readonly Logger Logger;
	private readonly Options Options;

	public PalmClient(FileInfo fileInfo, Logger logger, Options options)
	{
		FileInfo = fileInfo;
		Logger = logger;
		Options = options;


		Logger.LogVerbose($"Using prompt file {FileInfo.FullName}").Wait();
	}

	public async Task<IList<(string oldName, string newName)>> SummarizeFileNames(ICollection<string> filenames)
	{
		// since we don't guarentee order, we need to keep track of the old name
		var results = new ConcurrentBag<(string oldName, string newName)>();
		await Logger.LogVerbose($"Summarizing {filenames.Count} files");
		await Parallel.ForEachAsync(
				filenames,
				new ParallelOptions() { MaxDegreeOfParallelism = 5 },
		async (string filename, CancellationToken cts) =>
		{
			try
			{
				var result = await SummarizedFileName(filename, cts);
				results.Add((filename, result));
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		});

		return results.ToList();
	}

	public async Task<string> SummarizedFileName(string fileName, CancellationToken cts = default)
	{
		// get base name with extension
		var baseName = Path.GetFileName(fileName);
		var prefixPath = fileName.Substring(0, fileName.Length - baseName.Length);

		var uri = Options.BaseUrl
			.SetQueryParam("key", Environment.GetEnvironmentVariable("API_KEY"));

		// add query param

		await Logger.LogVerbose($"Summarizing {baseName} -- API Call Initiated");

		var request = new HttpRequestMessage(HttpMethod.Post, uri);
		using var file = File.OpenRead(FileInfo.FullName);
		using var reader = new StreamReader(file);
		var req = new PalmDataRequest()
		{
			Prompt = new Prompt()
			{

				Text = reader.ReadToEnd()
					+ $"\ninput: {baseName}\noutput:"
			},
		};

		var json = JsonConvert.SerializeObject(req);
		request.Content = new StringContent(json);
		request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
		var sw = Stopwatch.StartNew();
		var resp = await client.SendAsync(request, cts);
		try
		{
			resp.EnsureSuccessStatusCode();
		}
		catch
		{
			await Logger.LogVerbose($"Failed to summarize {baseName}");
			await Logger.LogVerbose(await resp.Content.ReadAsStringAsync(cts));
			throw;
		}

		var body = await resp.Content.ReadAsStringAsync(cts);
		var data = JsonConvert.DeserializeObject<PalmDataResponse>(body) ?? throw new Exception("No data");


		await Logger.LogVerbose($"Summarizing {baseName} -- API Call Completed in {sw.ElapsedMilliseconds}ms");

		return Path.Join(prefixPath, data.Candidates[0].Output);
	}
}


