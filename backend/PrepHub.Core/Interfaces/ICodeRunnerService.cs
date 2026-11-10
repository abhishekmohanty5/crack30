namespace PrepHub.Core.Interfaces;

/// <summary>
/// Stub interface for Phase 2 code runner integration.
/// </summary>
public interface ICodeRunnerService
{
    Task<object> RunCodeAsync(string language, string code, string input);
}
