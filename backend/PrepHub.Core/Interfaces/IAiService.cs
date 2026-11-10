namespace PrepHub.Core.Interfaces;

/// <summary>
/// Stub interface for Phase 2 AI-powered features.
/// </summary>
public interface IAiService
{
    Task<string> GenerateExplanationAsync(string question);
    Task<string> GenerateHintAsync(string problemStatement);
}
