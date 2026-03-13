namespace Quizzical.Misc.ExtensionMethods;

public static class HostApplicationBuilderExtension
{
    extension(HostApplicationBuilder builder)
    {
        public HostApplicationBuilder ConfigureApp()
        {
            builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly());

            var testConfigurationPath = builder.Configuration[ConfigKeys.TestConfigurationPath];

            if (!string.IsNullOrWhiteSpace(testConfigurationPath))
                builder.Configuration.AddJsonFile(testConfigurationPath, optional: false);

            return builder;
        }

        public HostApplicationBuilder ConfigureServices()
        {
            //// auto-mapper
            //builder.Services.AddAutoMapper(typeof(MapperProfile));

            // mediatr
            builder.Services.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()); });

            // strategies
            builder.Services
                .AddTransient<IQuizPlayStrategy, TrueFalseQuizPlayStrategy>()
                .AddTransient<IQuizPlayStrategy, MultipleChoiceQuizPlayStrategy>()
                .AddTransient<IQuizPlayStrategy, MultiSelectQuizPlayStrategy>()
                .AddTransient<IQuizPlayStrategy, GroupableItemsQuizPlayStrategy>();

            if (builder.Configuration.GetSection(ConfigKeys.TestAutomation).Exists())
                builder.Services.AddSingleton<IQuizPromptService, ConfiguredQuizPromptService>();
            else
                builder.Services.AddSingleton<IQuizPromptService, AnsiConsoleQuizPromptService>();

            builder.Services.AddTransient<IQuizFactory, QuizFactory>();

            if (builder.Configuration.GetSection(ConfigKeys.TestQuestionData).Exists())
            {
                builder.Services.AddTransient<IQuestionFactory, ConfiguredQuestionFactory>();
            }
            else
            {
                var quizzicalOpenAiApiKey = builder.Configuration[ConfigKeys.OpenAiApiKey];
                var quizzicalOpenAiModel = builder.Configuration[ConfigKeys.OpenAiModel];
                var openAiChatClient = new ChatClient(quizzicalOpenAiModel, quizzicalOpenAiApiKey);

                builder.Services.AddSingleton(openAiChatClient);
                builder.Services.AddTransient<IQuestionFactory, QuestionFactory>();
            }

            // repositories

            // misc
            builder.Services
                .AddTransient<SinglePlayerConsoleQuizEngine>();

            return builder;
        }
    }
}