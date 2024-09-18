using Azure.AI.OpenAI;
using OpenAI.Chat;
using OpenAIOpenWeatherMapClient.Utils;
using OpenWeatherMapSharp;
using System.ClientModel;
using System.Text.Json;

// Show header
ConsoleHelper.ShowHeader();

// Get API Key for OpenWeatherMap
string openWeatherMapApiKey =
    ConsoleHelper.GetString(
        "Please insert the [yellow]OpenWeatherMap[/] API key:", true);

// Create OpenWeatherMapService using the provided API Key
OpenWeatherMapService openWeatherMapService = new(openWeatherMapApiKey);

// Select whether to use Azure OpenAI or OpenAI
string host =
    ConsoleHelper.SelectFromOptions(
        [Statics.AzureOpenAIKey, Statics.OpenAIKey]);

// Create ChatClient
ChatClient? client = null;

// Create the appropriate ChatClient based on the user's selection
if (host == Statics.AzureOpenAIKey)
{
    string azureEndpoint =
        ConsoleHelper.GetUrl(
            "Please insert your [yellow]Azure OpenAI endpoint[/]:");

    string azureOpenAIKey =
        ConsoleHelper.GetString(
            $"Please insert your [yellow]{Statics.AzureOpenAIKey}[/] API key:", true);

    string azureDeploymentName =
        ConsoleHelper.GetString(
            "Please insert the [yellow]deployment name[/] of the model:", true);

    AzureOpenAIClient azureClient = new(
        new Uri(azureEndpoint),
        new ApiKeyCredential(azureOpenAIKey));

    client = azureClient.GetChatClient(azureDeploymentName);
}
else if (host == Statics.OpenAIKey)
{
    string openAIKey =
        ConsoleHelper.GetString(
            $"Please insert your [yellow]{Statics.OpenAIKey}[/] API key:", true);

    string openAIDeploymentName =
        ConsoleHelper.SelectFromOptions(
            [Statics.GPT4oKey, Statics.GPT4oMiniKey]);

    client = new ChatClient(openAIDeploymentName, new ApiKeyCredential(openAIKey));
}

// Exit if no client could be created
if (client == null)
{
    ConsoleHelper.WriteToConsole("Client creation failed.");
    return;
}

// Show the header
ConsoleHelper.ShowHeader();

// Initialize conversation message history
List<ChatMessage> conversationMessages = new();

// Define a tool for retrieving weather information
ChatTool getCurrentWeatherTool = ChatTool.CreateFunctionTool(
    nameof(OpenWeatherMapHelper.GetWeatherFromOpenWeatherAsync),
    "Get weather information for the provided city",
    BinaryData.FromString(
        @"
            {
              ""type"": ""object"",
              ""properties"": {
                ""city"": {
                  ""type"": ""string"",
                  ""description"": ""The name of the city, e.g. Boston""
                }
              },
              ""required"": [""city""]
            }
        ")
);

// Configure the chat completion options
ChatCompletionOptions options = new()
{
    Tools = { getCurrentWeatherTool }
};

while (true)
{
    // Get user input
    ConsoleHelper.WriteToConsole("[green]User:[/]");
    string? prompt = Console.ReadLine();
    while (prompt == null)
    {
        prompt = Console.ReadLine();
    }

    // Add the user prompt to the conversation
    conversationMessages.Add(new UserChatMessage(prompt));
    Console.WriteLine();

    // Send the prompt to the chat client and await a response
    ClientResult<ChatCompletion> completion =
        await client.CompleteChatAsync(conversationMessages, options);

    // Output the response to the console
    ConsoleHelper.WriteToConsole($"[green]Copilot:[/]");

    // Handle different completion reasons
    if (completion.Value?.FinishReason == ChatFinishReason.Stop)
    {
        ConsoleHelper.WriteToConsole(completion.Value.ToString());
    }
    else if (completion.Value?.FinishReason == ChatFinishReason.ToolCalls)
    {
        // Handle tool call (e.g., OpenWeatherMap function)
        ChatToolCall toolCall = completion.Value.ToolCalls[0];
        string chatToolOutput = await HandleToolCallAsync(toolCall);
        ConsoleHelper.WriteToConsole(chatToolOutput);
    }

    Console.WriteLine();
}

/// <summary>
///     Handles tool calls, specifically the OpenWeatherMap API call.
/// </summary>
/// <param name="toolCall">The tool call to handle.</param>
/// <returns>The result of the tool call.</returns>
async Task<string> HandleToolCallAsync(ChatToolCall toolCall)
{
    if (toolCall.FunctionName ==
        nameof(OpenWeatherMapHelper.GetWeatherFromOpenWeatherAsync))
    {
        // Validate and parse the JSON arguments for the city parameter
        try
        {
            using JsonDocument argumentsDocument =
                JsonDocument.Parse(toolCall.FunctionArguments);

            if (argumentsDocument.RootElement.TryGetProperty("city",
                out JsonElement cityElement) &&
                !string.IsNullOrEmpty(cityElement.GetString()))
            {
                return await OpenWeatherMapHelper.GetWeatherFromOpenWeatherAsync(
                    openWeatherMapService, cityElement.GetString()!);
            }
            return "Invalid or missing 'city' argument.";
        }
        catch (JsonException ex)
        {
            // Handle JSON parsing errors
            return $"Error parsing JSON: {ex.Message}";
        }
    }

    return "Unknown function call.";
}