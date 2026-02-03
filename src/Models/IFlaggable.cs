namespace Quizzical.Cli.Models;

/// <summary>
///     Represents an item that can be flagged.
/// </summary>
public interface IFlaggable
{
    /// <summary>
    ///     Whether the item has been flagged.
    /// </summary>
    bool IsFlagged { get; set; }

    /// <summary>
    ///     The reason the item was flagged.
    /// </summary>
    string? FlagReason { get; set; }
}