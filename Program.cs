using GPT_API_Client;
using Newtonsoft.Json;
using System.Text;

//Generate your own Api Key at OpenAI
var secretAppsettingReader = new SecretAppsettingReader();
var secretValues = secretAppsettingReader.ReadSection<SecretValues>("MySecretValues");

if (args.Length > 0)
{
    HttpClient client = new();

    client.DefaultRequestHeaders.Add("authorization", $"Bearer {secretValues.Token}");

    var content = new StringContent("{\"model\": \"text-davinci-001\", \"prompt\": \"" + args[0] + "\",\"temperature\": 1,\"max_tokens\": 100}",
          Encoding.UTF8, "application/json");

    HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/completions", content);

    string responseString = await response.Content.ReadAsStringAsync();

    //Deserializing the response
    try
    {
        //Using NewtoonSoft for dynamic desirialization
        var dynamicData = JsonConvert.DeserializeObject<dynamic>(responseString);


        string guess = GuesCommand(dynamicData!.choises[0].text);
        Console.ForegroundColor = ConsoleColor.Green;
        //Not recomanded
        Console.WriteLine($"--->My guess to the command prompt is : {guess}");
        Console.ResetColor();
    }
    catch (Exception ex)
    {

        Console.WriteLine($"---> Couldnt deserialize the JSON : {ex.Message}");
    }

    // Console.WriteLine(responseString);
}
else
{
    Console.WriteLine("---> You need to provide some input");
}

//Isolate the Command Prompt

static string GuesCommand(string raw)
{
    Console.WriteLine("---> GPT API Returned Text:");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine(raw);

    var lastIndex = raw.LastIndexOf("\n");

    string guess = raw.Substring(lastIndex + 1);

    Console.ResetColor();

    //Copy to clipboard
    TextCopy.ClipboardService.SetText(guess);

    return guess;
}