using Microsoft.EntityFrameworkCore;
using PrepHub.Core.Entities;
using PrepHub.Core.Enums;
using PrepHub.Infrastructure.Data;
using PrepHub.Infrastructure.Services;
using Xunit;

namespace PrepHub.Tests;

public class AdminServiceTests
{
    private PrepHubDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<PrepHubDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new PrepHubDbContext(options);
    }

    [Fact]
    public async Task ApproveAsync_McqPending_ChangesStatusToApproved()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var mcq = new McqQuestion
        {
            Id = 10,
            ChapterId = 1,
            Question = "What is LINQ?",
            OptionA = "Language Integrated Query",
            OptionB = "Local Integrated Network Query",
            OptionC = "Linked Index Query",
            OptionD = "None",
            CorrectOption = 'A',
            Status = ContentStatus.Pending
        };
        context.McqQuestions.Add(mcq);
        await context.SaveChangesAsync();

        var service = new AdminService(context);

        // Act
        await service.ApproveAsync("mcq", 10, "admin-1", "Looks good");

        // Assert
        var updated = await context.McqQuestions.FindAsync(10);
        Assert.NotNull(updated);
        Assert.Equal(ContentStatus.Approved, updated.Status);
        Assert.Equal("admin-1", updated.ReviewedById);
    }

    [Fact]
    public async Task RejectAsync_InterviewQuestionPending_ChangesStatusToRejected()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var question = new InterviewQuestion
        {
            Id = 20,
            ChapterId = 1,
            Question = "Explain boxing and unboxing.",
            ModelAnswer = "Boxing converts value type to object; unboxing extracts value type.",
            Status = ContentStatus.Pending
        };
        context.InterviewQuestions.Add(question);
        await context.SaveChangesAsync();

        var service = new AdminService(context);

        // Act
        await service.RejectAsync("interview", 20, "admin-1", "Needs more detail");

        // Assert
        var updated = await context.InterviewQuestions.FindAsync(20);
        Assert.NotNull(updated);
        Assert.Equal(ContentStatus.Rejected, updated.Status);
    }
}
