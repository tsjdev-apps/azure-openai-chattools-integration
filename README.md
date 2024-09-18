# Using ChatTools to Implement Custom Logic in Your Azure OpenAI Project

Explore the ChatTools functionality within the Azure.AI.OpenAI NuGet package to implement custom logic in your .NET AI project.

![Header](/docs/header.png)

In some cases, it’s necessary to call custom logic within your Azure OpenAI project. To address this need, OpenAI introduced a mechanism called Function Calling. Function Calling allows models like GPT-4 to connect with external tools and systems, enabling them to perform various tasks. This feature is valuable for enhancing AI assistants with specific capabilities or creating seamless integrations between your applications and AI models.

In this blog post, we will build a simple .NET console application that interacts with the OpenWeatherMap API to fetch current weather information for a specified city. If the user doesn’t want to retrieve weather data, the application will skip the API call and return the response directly.

## Usage

For this project, we’ll be using the free OpenWeatherMap API, but you’ll need to register and obtain an API key. To get started, visit [OpenWeatherMap](https://home.openweathermap.org/users/sign_in) and either log in or create a new account. Once logged in, navigate to the [API keys page](https://home.openweathermap.org/api_keys) to generate a new API key.

Additionally, you will need either a valid OpenAI API key or access to a running Azure OpenAI Service to test our application.

Finally run the app and follow the steps displayed on the screen.

## Screenshots

Let’s see the application in action. The first screenshot shows the input of the OpenWeatherMap API key.

![Screenshot01](/docs/chattool-screenshot-01.png)

The next screenshot shows the host selection process.

![Screenshot02](/docs/chattool-screenshot-02.png)

Depending on your selection, you will be prompted to enter the required parameters.

![Screenshot03](/docs/chattool-screenshot-03.png)

Finally, you can begin the chat.

![Screenshot04](/docs/chattool-screenshot-04.png)

## Blog Posts

If you are more interested into details, please see the following posts on [medium.com](https://medium.com/@tsjdevapps) or in my [personal blog](https://www.tsjdev-apps.de/):

- [Using ChatTools to Implement Custom Logic in Your Azure OpenAI Project](https://medium.com/medialesson/using-chattools-to-implement-custom-logic-in-your-azure-openai-project-26ee211529fe)
- [Einrichtung von OpenAI](https://www.tsjdev-apps.de/einrichtung-von-openai/)
- [Einrichtung von Azure OpenAI](https://www.tsjdev-apps.de/einrichtung-von-azure-openai/)