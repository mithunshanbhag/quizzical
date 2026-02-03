namespace Quizzical.Misc.ExtensionMethods;

public static class ChatCompletionExtension
{
    public static void Dump(this ChatCompletion chatCompletion, ILogger logger)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Chat completion details:");
        sb.AppendLine($"- Chat completion ID: {chatCompletion.Id}");
        sb.AppendLine($"- Created: {chatCompletion.CreatedAt}");
        sb.AppendLine($"- Finish reason: {chatCompletion.FinishReason.ToString()}");
        sb.AppendLine($"- Model: {chatCompletion.Model}");
        sb.AppendLine($"- Refusal: {chatCompletion.Refusal}");
        sb.AppendLine($"- Role: {chatCompletion.Role.ToString()}");
        sb.AppendLine($"- System fingerprint: {chatCompletion.SystemFingerprint}");

        var usage = chatCompletion.Usage;
        var iDetails = usage.InputTokenDetails;
        var oDetails = usage.OutputTokenDetails;
        sb.AppendLine("- Usage:");
        sb.AppendLine($"  - Input tokens: {usage.InputTokenCount} (cached = {iDetails.CachedTokenCount}, audio = {iDetails.AudioTokenCount})");
        sb.AppendLine($"  - Output tokens: {usage.OutputTokenCount} (reasoning = {oDetails.ReasoningTokenCount}, audio = {oDetails.ReasoningTokenCount})");
        sb.AppendLine($"  - Total tokens: {usage.TotalTokenCount}");

        var toolCalls = chatCompletion.ToolCalls;
        sb.AppendLine($"- Tool calls (count = {toolCalls.Count}):");
        foreach (var toolCall in toolCalls)
            sb.AppendLine($"  - ID: {toolCall.Id} | Kind: {toolCall.Kind.ToString()} | Function: {toolCall.FunctionName}");

        var content = chatCompletion.Content;
        sb.AppendLine($"- Content (count = {content.Count}):");
        foreach (var contentItem in content)
        {
            sb.AppendLine($"  - Kind: {contentItem.Kind}");
            sb.AppendLine($"  - Refusal: {contentItem.Refusal}");
            sb.AppendLine($"  - Text: {contentItem.Text}");
        }

        sb.AppendLine();

        logger.LogDebug(sb.ToString());
    }
}