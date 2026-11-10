using PrepHub.Core.Interfaces;

namespace PrepHub.Infrastructure.Services;

public class StubCodeRunnerService : ICodeRunnerService
{
    public Task<object> RunCodeAsync(string language, string code, string input)
    {
        return Task.FromResult<object>(new
        {
            Status = "NotImplemented",
            Message = "Code execution sandbox will be available in Phase 2."
        });
    }
}

public class StubAiService : IAiService
{
    public Task<string> GenerateExplanationAsync(string question)
    {
        return Task.FromResult("AI explanation will be available in Phase 2.");
    }

    public Task<string> GenerateHintAsync(string problemStatement)
    {
        return Task.FromResult("AI progressive hints will be available in Phase 2.");
    }
}
