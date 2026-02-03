namespace Quizzical.Models;

/// <summary>
///     Represents the response model for the breakdown of a recipe.
/// </summary>
/// <remarks>
///     Using a struct and instead of a class, since the latter's generated JSON schema encounters issues with
///     OpenAI's structured response format.
/// </remarks>
public struct ResponseModel;