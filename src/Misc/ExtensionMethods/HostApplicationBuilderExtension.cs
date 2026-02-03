namespace Quizzical.Misc.ExtensionMethods;

public static class HostApplicationBuilderExtension
{
    extension(HostApplicationBuilder builder)
    {
        public HostApplicationBuilder ConfigureApp()
        {
            builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly());

            return builder;
        }

        public HostApplicationBuilder ConfigureServices()
        {
            //// auto-mapper
            //builder.Services.AddAutoMapper(typeof(MapperProfile));

            // mediatr
            builder.Services.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()); });

            // openai client
            var quizzicalOpenAiApiKey = builder.Configuration[ConfigKeys.OpenAiApiKey];
            var quizzicalOpenAiModel = builder.Configuration[ConfigKeys.OpenAiModel];
            var openAiChatClient = new ChatClient(quizzicalOpenAiModel, quizzicalOpenAiApiKey);
            builder.Services.AddSingleton(openAiChatClient);

            // strategies
            builder.Services
                .AddTransient<IQuizPlayStrategy, TrueFalseQuizPlayStrategy>()
                .AddTransient<IQuizPlayStrategy, MultipleChoiceQuizPlayStrategy>()
                .AddTransient<IQuizPlayStrategy, GroupableItemsQuizPlayStrategy>();

            // services
            builder.Services
                .AddTransient<IQuizFactory, QuizFactory>()
                .AddTransient<IQuestionFactory, QuestionFactory>();

            // repositories

            // misc
            builder.Services
                .AddTransient<SinglePlayerConsoleQuizEngine>();

            return builder;
        }
    }
}