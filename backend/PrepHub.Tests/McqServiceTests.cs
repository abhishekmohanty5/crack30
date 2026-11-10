using Microsoft.EntityFrameworkCore;
using PrepHub.Core.DTOs;
using PrepHub.Core.Entities;
using PrepHub.Core.Enums;
using PrepHub.Infrastructure.Data;
using PrepHub.Infrastructure.Services;
using Xunit;

namespace PrepHub.Tests;

public class McqServiceTests
{
    private PrepHubDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<PrepHubDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new PrepHubDbContext(options);
    }

    [Fact]
    public async Task SubmitAttemptAsync_WithCorrectAnswer_ReturnsIsCorrectTrue()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var mcq = new McqQuestion
        {
            Id = 1,
            ChapterId = 1,
            Question = "What is the size of int in C#?",
            OptionA = "2 bytes",
            OptionB = "4 bytes",
            OptionC = "8 bytes",
            OptionD = "Depends on OS",
            CorrectOption = 'B',
            Explanation = "In C#, int is an alias for System.Int32 which is 4 bytes.",
            Status = ContentStatus.Approved,
            Difficulty = Difficulty.Easy
        };
        context.McqQuestions.Add(mcq);
        await context.SaveChangesAsync();

        var service = new McqService(context);

        // Act
        var result = await service.SubmitAttemptAsync(1, "user-123", new McqAttemptRequest('B', 15));

        // Assert
        Assert.True(result.IsCorrect);
        Assert.Equal('B', result.CorrectOption);
        Assert.Contains("4 bytes", result.Explanation);

        var attempts = await service.GetUserAttemptsAsync(1, "user-123");
        Assert.Single(attempts);
        Assert.True(attempts[0].IsCorrect);
    }

    [Fact]
    public async Task SubmitAttemptAsync_WithWrongAnswer_ReturnsIsCorrectFalse()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var mcq = new McqQuestion
        {
            Id = 2,
            ChapterId = 1,
            Question = "Which keyword is used for inheritance in C#?",
            OptionA = "extends",
            OptionB = "implements",
            OptionC = ":",
            OptionD = "inherits",
            CorrectOption = 'C',
            Explanation = "C# uses colon ':' for both class inheritance and interface implementation.",
            Status = ContentStatus.Approved,
            Difficulty = Difficulty.Easy
        };
        context.McqQuestions.Add(mcq);
        await context.SaveChangesAsync();

        var service = new McqService(context);

        // Act
        var result = await service.SubmitAttemptAsync(2, "user-123", new McqAttemptRequest('A', 10));

        // Assert
        Assert.False(result.IsCorrect);
        Assert.Equal('C', result.CorrectOption);

        var attempts = await service.GetUserAttemptsAsync(2, "user-123");
        Assert.Single(attempts);
        Assert.False(attempts[0].IsCorrect);
    }
}
