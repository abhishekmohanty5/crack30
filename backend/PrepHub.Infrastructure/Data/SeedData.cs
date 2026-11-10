using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PrepHub.Core.Entities;
using PrepHub.Core.Enums;

namespace PrepHub.Infrastructure.Data;

/// <summary>
/// Seeds the database with courses, topics, chapters, sample content, and admin user.
/// </summary>
public static class SeedData
{
    /// <summary>
    /// Applies pending migrations and seeds all data.
    /// </summary>
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PrepHubDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await context.Database.MigrateAsync();

        await SeedRolesAsync(roleManager);
        var adminId = await SeedAdminUserAsync(userManager);
        await SeedCoursesAsync(context, adminId);
        await SeedHiringStagesAsync(context);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = ["Student", "Contributor", "Admin"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    private static async Task<string> SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
    {
        const string email = "admin@prephub.com";
        var existing = await userManager.FindByEmailAsync(email);
        if (existing != null)
            return existing.Id;

        var admin = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            Name = "PrepHub Admin",
            Batch = "Admin",
            CreatedAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(admin, "Admin@123");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }

        return admin.Id;
    }

    private static async Task SeedCoursesAsync(PrepHubDbContext context, string adminId)
    {
        if (await context.Courses.AnyAsync())
            return;

        // ═══════════════════════════════════════════════════════════
        //  COURSE 1: C# + .NET
        // ═══════════════════════════════════════════════════════════
        var course1 = new Course { Title = "C# + .NET", Description = "Comprehensive C# and .NET fundamentals including OOP, LINQ, async programming, and framework essentials.", Hours = 40, SortOrder = 1 };

        var t1_1 = new Topic { Title = "C# Basics", SortOrder = 1, Course = course1 };
        t1_1.Chapters = [
            new Chapter { Title = "Variables & Data Types", SortOrder = 1, Topic = t1_1 },
            new Chapter { Title = "Control Flow", SortOrder = 2, Topic = t1_1 },
            new Chapter { Title = "Methods & Parameters", SortOrder = 3, Topic = t1_1 },
            new Chapter { Title = "Exception Handling", SortOrder = 4, Topic = t1_1 }
        ];

        var t1_2 = new Topic { Title = "OOP in C#", SortOrder = 2, Course = course1 };
        var chClassesObjects = new Chapter { Title = "Classes & Objects", SortOrder = 1, Topic = t1_2 };
        t1_2.Chapters = [
            chClassesObjects,
            new Chapter { Title = "Inheritance & Polymorphism", SortOrder = 2, Topic = t1_2 },
            new Chapter { Title = "Abstraction & Interfaces", SortOrder = 3, Topic = t1_2 },
            new Chapter { Title = "Encapsulation", SortOrder = 4, Topic = t1_2 }
        ];

        var t1_3 = new Topic { Title = "Collections & LINQ", SortOrder = 3, Course = course1 };
        var chLinqBasics = new Chapter { Title = "LINQ Basics", SortOrder = 3, Topic = t1_3 };
        t1_3.Chapters = [
            new Chapter { Title = "Arrays & Lists", SortOrder = 1, Topic = t1_3 },
            new Chapter { Title = "Dictionaries & Sets", SortOrder = 2, Topic = t1_3 },
            chLinqBasics,
            new Chapter { Title = "LINQ Advanced Queries", SortOrder = 4, Topic = t1_3 }
        ];

        var t1_4 = new Topic { Title = "Async Programming", SortOrder = 4, Course = course1 };
        t1_4.Chapters = [
            new Chapter { Title = "Tasks & async/await", SortOrder = 1, Topic = t1_4 },
            new Chapter { Title = "Parallel Programming", SortOrder = 2, Topic = t1_4 },
            new Chapter { Title = "Threading Basics", SortOrder = 3, Topic = t1_4 }
        ];

        var t1_5 = new Topic { Title = ".NET Essentials", SortOrder = 5, Course = course1 };
        t1_5.Chapters = [
            new Chapter { Title = "CLR & Memory Management", SortOrder = 1, Topic = t1_5 },
            new Chapter { Title = "Delegates & Events", SortOrder = 2, Topic = t1_5 },
            new Chapter { Title = "Generics", SortOrder = 3, Topic = t1_5 },
            new Chapter { Title = "File I/O", SortOrder = 4, Topic = t1_5 }
        ];

        course1.Topics = [t1_1, t1_2, t1_3, t1_4, t1_5];

        // ═══════════════════════════════════════════════════════════
        //  COURSE 2: .NET (ASP.NET Core, EF Core, Web API)
        // ═══════════════════════════════════════════════════════════
        var course2 = new Course { Title = ".NET (ASP.NET Core, EF Core, Web API)", Description = "Building modern web applications with ASP.NET Core, Entity Framework Core, and Web API development.", Hours = 45, SortOrder = 2 };

        var t2_1 = new Topic { Title = "ASP.NET Core Fundamentals", SortOrder = 1, Course = course2 };
        t2_1.Chapters = [
            new Chapter { Title = "MVC Pattern", SortOrder = 1, Topic = t2_1 },
            new Chapter { Title = "Middleware & Pipeline", SortOrder = 2, Topic = t2_1 },
            new Chapter { Title = "Dependency Injection", SortOrder = 3, Topic = t2_1 },
            new Chapter { Title = "Configuration & Logging", SortOrder = 4, Topic = t2_1 }
        ];
        var chDI = t2_1.Chapters.First(c => c.Title == "Dependency Injection");

        var t2_2 = new Topic { Title = "Web API Development", SortOrder = 2, Course = course2 };
        t2_2.Chapters = [
            new Chapter { Title = "RESTful API Design", SortOrder = 1, Topic = t2_2 },
            new Chapter { Title = "Controllers & Routing", SortOrder = 2, Topic = t2_2 },
            new Chapter { Title = "Model Binding & Validation", SortOrder = 3, Topic = t2_2 },
            new Chapter { Title = "API Versioning & Documentation", SortOrder = 4, Topic = t2_2 }
        ];

        var t2_3 = new Topic { Title = "Entity Framework Core", SortOrder = 3, Course = course2 };
        t2_3.Chapters = [
            new Chapter { Title = "DbContext & Migrations", SortOrder = 1, Topic = t2_3 },
            new Chapter { Title = "Querying Data", SortOrder = 2, Topic = t2_3 },
            new Chapter { Title = "Relationships", SortOrder = 3, Topic = t2_3 },
            new Chapter { Title = "Performance & Optimization", SortOrder = 4, Topic = t2_3 }
        ];

        var t2_4 = new Topic { Title = "Security & Auth", SortOrder = 4, Course = course2 };
        t2_4.Chapters = [
            new Chapter { Title = "Authentication", SortOrder = 1, Topic = t2_4 },
            new Chapter { Title = "Authorization", SortOrder = 2, Topic = t2_4 },
            new Chapter { Title = "JWT & Identity", SortOrder = 3, Topic = t2_4 },
            new Chapter { Title = "Data Protection", SortOrder = 4, Topic = t2_4 }
        ];

        course2.Topics = [t2_1, t2_2, t2_3, t2_4];

        // ═══════════════════════════════════════════════════════════
        //  COURSE 3: Java
        // ═══════════════════════════════════════════════════════════
        var course3 = new Course { Title = "Java", Description = "Core Java programming covering fundamentals, OOP, collections, and advanced topics.", Hours = 40, SortOrder = 3 };

        var t3_1 = new Topic { Title = "Java Fundamentals", SortOrder = 1, Course = course3 };
        t3_1.Chapters = [
            new Chapter { Title = "Syntax & Data Types", SortOrder = 1, Topic = t3_1 },
            new Chapter { Title = "Control Structures", SortOrder = 2, Topic = t3_1 },
            new Chapter { Title = "Methods", SortOrder = 3, Topic = t3_1 },
            new Chapter { Title = "Exception Handling", SortOrder = 4, Topic = t3_1 }
        ];

        var t3_2 = new Topic { Title = "OOP in Java", SortOrder = 2, Course = course3 };
        t3_2.Chapters = [
            new Chapter { Title = "Classes & Interfaces", SortOrder = 1, Topic = t3_2 },
            new Chapter { Title = "Inheritance", SortOrder = 2, Topic = t3_2 },
            new Chapter { Title = "Polymorphism", SortOrder = 3, Topic = t3_2 },
            new Chapter { Title = "Abstract Classes", SortOrder = 4, Topic = t3_2 }
        ];

        var t3_3 = new Topic { Title = "Collections Framework", SortOrder = 3, Course = course3 };
        t3_3.Chapters = [
            new Chapter { Title = "Lists & Sets", SortOrder = 1, Topic = t3_3 },
            new Chapter { Title = "Maps", SortOrder = 2, Topic = t3_3 },
            new Chapter { Title = "Iterators & Streams", SortOrder = 3, Topic = t3_3 },
            new Chapter { Title = "Comparable & Comparator", SortOrder = 4, Topic = t3_3 }
        ];

        var t3_4 = new Topic { Title = "Java Advanced", SortOrder = 4, Course = course3 };
        t3_4.Chapters = [
            new Chapter { Title = "Multithreading", SortOrder = 1, Topic = t3_4 },
            new Chapter { Title = "File I/O", SortOrder = 2, Topic = t3_4 },
            new Chapter { Title = "JDBC", SortOrder = 3, Topic = t3_4 },
            new Chapter { Title = "Generics", SortOrder = 4, Topic = t3_4 }
        ];

        course3.Topics = [t3_1, t3_2, t3_3, t3_4];

        // ═══════════════════════════════════════════════════════════
        //  COURSE 4: Python
        // ═══════════════════════════════════════════════════════════
        var course4 = new Course { Title = "Python", Description = "Python programming from basics to advanced topics including OOP, data structures, and web development.", Hours = 35, SortOrder = 4 };

        var t4_1 = new Topic { Title = "Python Basics", SortOrder = 1, Course = course4 };
        t4_1.Chapters = [
            new Chapter { Title = "Syntax & Types", SortOrder = 1, Topic = t4_1 },
            new Chapter { Title = "Control Flow", SortOrder = 2, Topic = t4_1 },
            new Chapter { Title = "Functions", SortOrder = 3, Topic = t4_1 },
            new Chapter { Title = "Modules & Packages", SortOrder = 4, Topic = t4_1 }
        ];

        var t4_2 = new Topic { Title = "Data Structures", SortOrder = 2, Course = course4 };
        t4_2.Chapters = [
            new Chapter { Title = "Lists & Tuples", SortOrder = 1, Topic = t4_2 },
            new Chapter { Title = "Dictionaries & Sets", SortOrder = 2, Topic = t4_2 },
            new Chapter { Title = "Comprehensions", SortOrder = 3, Topic = t4_2 },
            new Chapter { Title = "String Operations", SortOrder = 4, Topic = t4_2 }
        ];

        var t4_3 = new Topic { Title = "Python OOP", SortOrder = 3, Course = course4 };
        t4_3.Chapters = [
            new Chapter { Title = "Classes & Objects", SortOrder = 1, Topic = t4_3 },
            new Chapter { Title = "Inheritance", SortOrder = 2, Topic = t4_3 },
            new Chapter { Title = "Magic Methods", SortOrder = 3, Topic = t4_3 },
            new Chapter { Title = "Decorators", SortOrder = 4, Topic = t4_3 }
        ];

        var t4_4 = new Topic { Title = "Python for Dev", SortOrder = 4, Course = course4 };
        t4_4.Chapters = [
            new Chapter { Title = "File Handling", SortOrder = 1, Topic = t4_4 },
            new Chapter { Title = "Error Handling", SortOrder = 2, Topic = t4_4 },
            new Chapter { Title = "Unit Testing", SortOrder = 3, Topic = t4_4 },
            new Chapter { Title = "REST with Flask/FastAPI", SortOrder = 4, Topic = t4_4 }
        ];

        course4.Topics = [t4_1, t4_2, t4_3, t4_4];

        // ═══════════════════════════════════════════════════════════
        //  COURSE 5: JavaScript & React (Intro)
        // ═══════════════════════════════════════════════════════════
        var course5 = new Course { Title = "JavaScript & React (Intro)", Description = "JavaScript fundamentals, DOM manipulation, and introduction to React development.", Hours = 35, SortOrder = 5 };

        var t5_1 = new Topic { Title = "JavaScript Core", SortOrder = 1, Course = course5 };
        t5_1.Chapters = [
            new Chapter { Title = "Variables & Types", SortOrder = 1, Topic = t5_1 },
            new Chapter { Title = "Functions & Scope", SortOrder = 2, Topic = t5_1 },
            new Chapter { Title = "Arrays & Objects", SortOrder = 3, Topic = t5_1 },
            new Chapter { Title = "Promises & Async", SortOrder = 4, Topic = t5_1 }
        ];

        var t5_2 = new Topic { Title = "DOM & Events", SortOrder = 2, Course = course5 };
        t5_2.Chapters = [
            new Chapter { Title = "DOM Manipulation", SortOrder = 1, Topic = t5_2 },
            new Chapter { Title = "Event Handling", SortOrder = 2, Topic = t5_2 },
            new Chapter { Title = "Fetch API", SortOrder = 3, Topic = t5_2 },
            new Chapter { Title = "Browser Storage", SortOrder = 4, Topic = t5_2 }
        ];

        var t5_3 = new Topic { Title = "Intro to React", SortOrder = 3, Course = course5 };
        t5_3.Chapters = [
            new Chapter { Title = "Components & JSX", SortOrder = 1, Topic = t5_3 },
            new Chapter { Title = "Props & State", SortOrder = 2, Topic = t5_3 },
            new Chapter { Title = "Hooks Basics", SortOrder = 3, Topic = t5_3 },
            new Chapter { Title = "Routing Basics", SortOrder = 4, Topic = t5_3 }
        ];

        course5.Topics = [t5_1, t5_2, t5_3];

        // ═══════════════════════════════════════════════════════════
        //  COURSE 6: ReactJS + Databases
        // ═══════════════════════════════════════════════════════════
        var course6 = new Course { Title = "ReactJS + Databases", Description = "Advanced React patterns, SQL fundamentals, and NoSQL basics.", Hours = 40, SortOrder = 6 };

        var t6_1 = new Topic { Title = "Advanced React", SortOrder = 1, Course = course6 };
        t6_1.Chapters = [
            new Chapter { Title = "State Management", SortOrder = 1, Topic = t6_1 },
            new Chapter { Title = "Context & Reducers", SortOrder = 2, Topic = t6_1 },
            new Chapter { Title = "Custom Hooks", SortOrder = 3, Topic = t6_1 },
            new Chapter { Title = "Performance Optimization", SortOrder = 4, Topic = t6_1 }
        ];

        var t6_2 = new Topic { Title = "SQL Fundamentals", SortOrder = 2, Course = course6 };
        t6_2.Chapters = [
            new Chapter { Title = "SELECT & Joins", SortOrder = 1, Topic = t6_2 },
            new Chapter { Title = "Subqueries", SortOrder = 2, Topic = t6_2 },
            new Chapter { Title = "Aggregation & Grouping", SortOrder = 3, Topic = t6_2 },
            new Chapter { Title = "Indexes & Optimization", SortOrder = 4, Topic = t6_2 }
        ];

        var t6_3 = new Topic { Title = "NoSQL Basics", SortOrder = 3, Course = course6 };
        t6_3.Chapters = [
            new Chapter { Title = "MongoDB Basics", SortOrder = 1, Topic = t6_3 },
            new Chapter { Title = "Document Design", SortOrder = 2, Topic = t6_3 },
            new Chapter { Title = "CRUD Operations", SortOrder = 3, Topic = t6_3 }
        ];

        course6.Topics = [t6_1, t6_2, t6_3];

        // ═══════════════════════════════════════════════════════════
        //  COURSE 7: Databases + Testing + Production Support
        // ═══════════════════════════════════════════════════════════
        var course7 = new Course { Title = "Databases + Testing + Production Support", Description = "Advanced SQL, testing methodologies, and production support practices.", Hours = 35, SortOrder = 7 };

        var t7_1 = new Topic { Title = "Advanced SQL", SortOrder = 1, Course = course7 };
        t7_1.Chapters = [
            new Chapter { Title = "Stored Procedures", SortOrder = 1, Topic = t7_1 },
            new Chapter { Title = "Triggers", SortOrder = 2, Topic = t7_1 },
            new Chapter { Title = "Transactions", SortOrder = 3, Topic = t7_1 },
            new Chapter { Title = "Database Design & Normalization", SortOrder = 4, Topic = t7_1 }
        ];

        var t7_2 = new Topic { Title = "Testing", SortOrder = 2, Course = course7 };
        t7_2.Chapters = [
            new Chapter { Title = "Unit Testing", SortOrder = 1, Topic = t7_2 },
            new Chapter { Title = "Integration Testing", SortOrder = 2, Topic = t7_2 },
            new Chapter { Title = "Test-Driven Development", SortOrder = 3, Topic = t7_2 },
            new Chapter { Title = "Mocking", SortOrder = 4, Topic = t7_2 }
        ];

        var t7_3 = new Topic { Title = "Prod Support", SortOrder = 3, Course = course7 };
        t7_3.Chapters = [
            new Chapter { Title = "Monitoring & Logging", SortOrder = 1, Topic = t7_3 },
            new Chapter { Title = "Incident Management", SortOrder = 2, Topic = t7_3 },
            new Chapter { Title = "Debugging Production Issues", SortOrder = 3, Topic = t7_3 }
        ];

        course7.Topics = [t7_1, t7_2, t7_3];

        // ═══════════════════════════════════════════════════════════
        //  COURSE 8: Enterprise Orientation & SDLC
        // ═══════════════════════════════════════════════════════════
        var course8 = new Course { Title = "Enterprise Orientation & SDLC", Description = "Software development lifecycle, Agile methodologies, and professional skills.", Hours = 20, SortOrder = 8 };

        var t8_1 = new Topic { Title = "SDLC", SortOrder = 1, Course = course8 };
        t8_1.Chapters = [
            new Chapter { Title = "Waterfall", SortOrder = 1, Topic = t8_1 },
            new Chapter { Title = "Agile & Scrum", SortOrder = 2, Topic = t8_1 },
            new Chapter { Title = "Kanban", SortOrder = 3, Topic = t8_1 },
            new Chapter { Title = "DevOps Overview", SortOrder = 4, Topic = t8_1 }
        ];

        var t8_2 = new Topic { Title = "Professional Skills", SortOrder = 2, Course = course8 };
        t8_2.Chapters = [
            new Chapter { Title = "Communication", SortOrder = 1, Topic = t8_2 },
            new Chapter { Title = "Team Collaboration", SortOrder = 2, Topic = t8_2 },
            new Chapter { Title = "Documentation", SortOrder = 3, Topic = t8_2 },
            new Chapter { Title = "Code Reviews", SortOrder = 4, Topic = t8_2 }
        ];

        course8.Topics = [t8_1, t8_2];

        // ═══════════════════════════════════════════════════════════
        //  COURSE 9: Hybrid Cloud and Data Management
        // ═══════════════════════════════════════════════════════════
        var course9 = new Course { Title = "Hybrid Cloud and Data Management", Description = "Cloud computing fundamentals, services, and data management strategies.", Hours = 30, SortOrder = 9 };

        var t9_1 = new Topic { Title = "Cloud Fundamentals", SortOrder = 1, Course = course9 };
        t9_1.Chapters = [
            new Chapter { Title = "IaaS/PaaS/SaaS", SortOrder = 1, Topic = t9_1 },
            new Chapter { Title = "AWS/Azure/GCP Overview", SortOrder = 2, Topic = t9_1 },
            new Chapter { Title = "Virtualization", SortOrder = 3, Topic = t9_1 },
            new Chapter { Title = "Containers Intro", SortOrder = 4, Topic = t9_1 }
        ];

        var t9_2 = new Topic { Title = "Data Management", SortOrder = 2, Course = course9 };
        t9_2.Chapters = [
            new Chapter { Title = "Data Lifecycle", SortOrder = 1, Topic = t9_2 },
            new Chapter { Title = "Backup & Recovery", SortOrder = 2, Topic = t9_2 },
            new Chapter { Title = "Data Security", SortOrder = 3, Topic = t9_2 },
            new Chapter { Title = "Compliance", SortOrder = 4, Topic = t9_2 }
        ];

        course9.Topics = [t9_1, t9_2];

        // ═══════════════════════════════════════════════════════════
        //  COURSE 10: DevOps & Deployment
        // ═══════════════════════════════════════════════════════════
        var course10 = new Course { Title = "DevOps & Deployment", Description = "CI/CD pipelines, containerization with Docker, and application monitoring.", Hours = 30, SortOrder = 10 };

        var t10_1 = new Topic { Title = "CI/CD", SortOrder = 1, Course = course10 };
        t10_1.Chapters = [
            new Chapter { Title = "Git & Version Control", SortOrder = 1, Topic = t10_1 },
            new Chapter { Title = "Build Pipelines", SortOrder = 2, Topic = t10_1 },
            new Chapter { Title = "Deployment Strategies", SortOrder = 3, Topic = t10_1 },
            new Chapter { Title = "GitHub Actions", SortOrder = 4, Topic = t10_1 }
        ];

        var t10_2 = new Topic { Title = "Containerization", SortOrder = 2, Course = course10 };
        t10_2.Chapters = [
            new Chapter { Title = "Docker Basics", SortOrder = 1, Topic = t10_2 },
            new Chapter { Title = "Docker Compose", SortOrder = 2, Topic = t10_2 },
            new Chapter { Title = "Kubernetes Intro", SortOrder = 3, Topic = t10_2 }
        ];

        var t10_3 = new Topic { Title = "Monitoring", SortOrder = 3, Course = course10 };
        t10_3.Chapters = [
            new Chapter { Title = "Application Monitoring", SortOrder = 1, Topic = t10_3 },
            new Chapter { Title = "Log Aggregation", SortOrder = 2, Topic = t10_3 },
            new Chapter { Title = "Alerting", SortOrder = 3, Topic = t10_3 }
        ];

        course10.Topics = [t10_1, t10_2, t10_3];

        // ═══════════════════════════════════════════════════════════
        //  COURSE 11: Programming Foundations
        // ═══════════════════════════════════════════════════════════
        var course11 = new Course { Title = "Programming Foundations", Description = "Foundational programming concepts, C language, and UniBasic.", Hours = 30, SortOrder = 11 };

        var t11_1 = new Topic { Title = "Programming Concepts", SortOrder = 1, Course = course11 };
        t11_1.Chapters = [
            new Chapter { Title = "Algorithms & Flowcharts", SortOrder = 1, Topic = t11_1 },
            new Chapter { Title = "Data Types", SortOrder = 2, Topic = t11_1 },
            new Chapter { Title = "Control Structures", SortOrder = 3, Topic = t11_1 },
            new Chapter { Title = "Functions", SortOrder = 4, Topic = t11_1 }
        ];

        var t11_2 = new Topic { Title = "C Language", SortOrder = 2, Course = course11 };
        t11_2.Chapters = [
            new Chapter { Title = "Pointers", SortOrder = 1, Topic = t11_2 },
            new Chapter { Title = "Memory Management", SortOrder = 2, Topic = t11_2 },
            new Chapter { Title = "Structures", SortOrder = 3, Topic = t11_2 },
            new Chapter { Title = "File I/O", SortOrder = 4, Topic = t11_2 }
        ];

        var t11_3 = new Topic { Title = "UniBasic", SortOrder = 3, Course = course11 };
        t11_3.Chapters = [
            new Chapter { Title = "Syntax", SortOrder = 1, Topic = t11_3 },
            new Chapter { Title = "Data Handling", SortOrder = 2, Topic = t11_3 },
            new Chapter { Title = "Subroutines", SortOrder = 3, Topic = t11_3 }
        ];

        course11.Topics = [t11_1, t11_2, t11_3];

        // ═══════════════════════════════════════════════════════════
        //  COURSE 12: Aptitude
        // ═══════════════════════════════════════════════════════════
        var course12 = new Course { Title = "Aptitude", Description = "Quantitative aptitude, logical reasoning, and psychometric assessment preparation.", Hours = 25, SortOrder = 12 };

        var t12_1 = new Topic { Title = "Quantitative Aptitude", SortOrder = 1, Course = course12 };
        t12_1.Chapters = [
            new Chapter { Title = "Number Systems", SortOrder = 1, Topic = t12_1 },
            new Chapter { Title = "Percentages & Ratios", SortOrder = 2, Topic = t12_1 },
            new Chapter { Title = "Time & Work", SortOrder = 3, Topic = t12_1 },
            new Chapter { Title = "Probability", SortOrder = 4, Topic = t12_1 }
        ];

        var t12_2 = new Topic { Title = "Logical Reasoning", SortOrder = 2, Course = course12 };
        t12_2.Chapters = [
            new Chapter { Title = "Pattern Recognition", SortOrder = 1, Topic = t12_2 },
            new Chapter { Title = "Syllogisms", SortOrder = 2, Topic = t12_2 },
            new Chapter { Title = "Blood Relations", SortOrder = 3, Topic = t12_2 },
            new Chapter { Title = "Coding-Decoding", SortOrder = 4, Topic = t12_2 }
        ];

        var t12_3 = new Topic { Title = "Psychometric", SortOrder = 3, Course = course12 };
        t12_3.Chapters = [
            new Chapter { Title = "Personality Assessment Tips", SortOrder = 1, Topic = t12_3 },
            new Chapter { Title = "Situational Judgment", SortOrder = 2, Topic = t12_3 },
            new Chapter { Title = "Behavioral Questions", SortOrder = 3, Topic = t12_3 }
        ];

        course12.Topics = [t12_1, t12_2, t12_3];

        // ── Add all courses ────────────────────────────────────
        context.Courses.AddRange(course1, course2, course3, course4, course5, course6, course7, course8, course9, course10, course11, course12);
        await context.SaveChangesAsync();

        // ═══════════════════════════════════════════════════════════
        //  SAMPLE CONTENT for "C# + .NET > OOP > Classes & Objects"
        // ═══════════════════════════════════════════════════════════

        // 2 Notes
        context.Notes.AddRange(
            new Note
            {
                ChapterId = chClassesObjects.Id,
                Title = "Classes & Objects — Quick Revision Sheet",
                MarkdownBody = @"# Classes & Objects in C#

## What is a Class?
A **class** is a blueprint or template that defines the properties (fields) and behaviors (methods) that objects of that type will have.

```csharp
public class Car
{
    // Fields
    public string Make;
    public string Model;
    public int Year;

    // Method
    public void Start()
    {
        Console.WriteLine($""{Make} {Model} is starting..."");
    }
}
```

## What is an Object?
An **object** is an instance of a class. It is created using the `new` keyword.

```csharp
Car myCar = new Car();
myCar.Make = ""Toyota"";
myCar.Model = ""Camry"";
myCar.Year = 2024;
myCar.Start(); // Output: Toyota Camry is starting...
```

## Key Concepts
| Concept | Description |
|---------|-------------|
| **Constructor** | Special method called when an object is created |
| **Properties** | Getters/setters that encapsulate fields |
| **this keyword** | Refers to the current instance |
| **static** | Belongs to the class, not instances |
| **Access modifiers** | public, private, protected, internal |

## Constructors
```csharp
public class Student
{
    public string Name { get; set; }
    public int Age { get; set; }

    // Default constructor
    public Student() { }

    // Parameterized constructor
    public Student(string name, int age)
    {
        Name = name;
        Age = age;
    }
}
```

## Memory: Stack vs Heap
- **Value types** (int, bool, struct) → stored on **Stack**
- **Reference types** (class, string, array) → stored on **Heap**, reference on Stack",
                Status = ContentStatus.Approved,
                CreatedById = adminId,
                CreatedAt = DateTime.UtcNow
            },
            new Note
            {
                ChapterId = chClassesObjects.Id,
                Title = "OOP Pillars — Classes Deep Dive",
                MarkdownBody = @"# Deep Dive: Classes in C#

## Properties with Backing Fields
```csharp
private string _name;
public string Name
{
    get => _name;
    set => _name = value ?? throw new ArgumentNullException(nameof(value));
}
```

## Auto-Implemented Properties
```csharp
public string Name { get; set; }       // read-write
public int Id { get; init; }           // init-only (C# 9+)
public string Type { get; } = ""Default""; // read-only with default
```

## Object Initializer Syntax
```csharp
var student = new Student
{
    Name = ""Alice"",
    Age = 22
};
```

## Records (C# 9+)
For immutable data objects:
```csharp
public record Person(string Name, int Age);
var p = new Person(""Bob"", 30);
```

## Destructors / Finalizers
```csharp
~MyClass()
{
    // Cleanup unmanaged resources
}
```

## Partial Classes
Split a class across multiple files:
```csharp
// File1.cs
public partial class MyClass { public void Method1() { } }

// File2.cs
public partial class MyClass { public void Method2() { } }
```

## Common Interview Tips
- Know the difference between **class** and **struct**
- Understand **reference vs value** semantics
- Be able to explain **constructor chaining** with `: this()` and `: base()`
- Understand when to use **sealed** classes",
                Status = ContentStatus.Approved,
                CreatedById = adminId,
                CreatedAt = DateTime.UtcNow
            }
        );

        // 10 MCQs for "Classes & Objects"
        context.McqQuestions.AddRange(
            new McqQuestion
            {
                ChapterId = chClassesObjects.Id, Question = "What keyword is used to create an instance of a class in C#?",
                OptionA = "class", OptionB = "new", OptionC = "create", OptionD = "instance",
                CorrectOption = 'B', Explanation = "The 'new' keyword allocates memory on the heap and calls the constructor to create an instance of a class.",
                Difficulty = Difficulty.Easy, Tags = "basics,object-creation", Status = ContentStatus.Approved, CreatedById = adminId
            },
            new McqQuestion
            {
                ChapterId = chClassesObjects.Id, Question = "Which of the following is NOT an access modifier in C#?",
                OptionA = "public", OptionB = "private", OptionC = "friend", OptionD = "protected",
                CorrectOption = 'C', Explanation = "'friend' is not a C# access modifier. C# uses 'internal' instead. The valid access modifiers are: public, private, protected, internal, and protected internal.",
                Difficulty = Difficulty.Easy, Tags = "access-modifiers", Status = ContentStatus.Approved, CreatedById = adminId
            },
            new McqQuestion
            {
                ChapterId = chClassesObjects.Id, Question = "What is the default access modifier for class members in C#?",
                OptionA = "public", OptionB = "private", OptionC = "protected", OptionD = "internal",
                CorrectOption = 'B', Explanation = "Class members are 'private' by default in C#. This follows the principle of least privilege.",
                Difficulty = Difficulty.Easy, Tags = "access-modifiers,defaults", Status = ContentStatus.Approved, CreatedById = adminId
            },
            new McqQuestion
            {
                ChapterId = chClassesObjects.Id, Question = "What is a constructor in C#?",
                OptionA = "A method that destroys objects", OptionB = "A special method called when an object is created", OptionC = "A static method of a class", OptionD = "A method that returns a value",
                CorrectOption = 'B', Explanation = "A constructor is a special method with the same name as the class that is automatically called when an instance is created using the 'new' keyword.",
                Difficulty = Difficulty.Easy, Tags = "constructors", Status = ContentStatus.Approved, CreatedById = adminId
            },
            new McqQuestion
            {
                ChapterId = chClassesObjects.Id, Question = "Where are reference type objects stored in memory?",
                OptionA = "Stack only", OptionB = "Heap only", OptionC = "Reference on stack, object on heap", OptionD = "CPU registers",
                CorrectOption = 'C', Explanation = "Reference type objects (class instances) are stored on the heap, while the reference variable pointing to them is stored on the stack.",
                Difficulty = Difficulty.Medium, Tags = "memory,stack-heap", Status = ContentStatus.Approved, CreatedById = adminId
            },
            new McqQuestion
            {
                ChapterId = chClassesObjects.Id, Question = "What is the purpose of the 'this' keyword in C#?",
                OptionA = "To create a new object", OptionB = "To refer to the current instance of the class", OptionC = "To call a static method", OptionD = "To inherit from a base class",
                CorrectOption = 'B', Explanation = "'this' refers to the current instance of the class. It's used to disambiguate between local variables and fields, and for constructor chaining.",
                Difficulty = Difficulty.Medium, Tags = "this-keyword", Status = ContentStatus.Approved, CreatedById = adminId
            },
            new McqQuestion
            {
                ChapterId = chClassesObjects.Id, Question = "What is the difference between a class and a struct in C#?",
                OptionA = "Classes are value types, structs are reference types", OptionB = "Classes support inheritance, structs do not", OptionC = "Structs can have destructors, classes cannot", OptionD = "There is no difference",
                CorrectOption = 'B', Explanation = "Classes are reference types that support inheritance. Structs are value types that do not support inheritance (they can implement interfaces). Classes are allocated on the heap; structs on the stack.",
                Difficulty = Difficulty.Medium, Tags = "class-vs-struct", Status = ContentStatus.Approved, CreatedById = adminId
            },
            new McqQuestion
            {
                ChapterId = chClassesObjects.Id, Question = "What does the 'sealed' keyword do when applied to a class?",
                OptionA = "Makes the class abstract", OptionB = "Prevents the class from being inherited", OptionC = "Makes all members private", OptionD = "Makes the class static",
                CorrectOption = 'B', Explanation = "The 'sealed' keyword prevents other classes from inheriting from the sealed class. This can improve performance and is used when inheritance is not intended.",
                Difficulty = Difficulty.Medium, Tags = "sealed,inheritance", Status = ContentStatus.Approved, CreatedById = adminId
            },
            new McqQuestion
            {
                ChapterId = chClassesObjects.Id, Question = "What is constructor chaining in C#?",
                OptionA = "Calling multiple constructors simultaneously", OptionB = "One constructor calling another constructor using ':this()' or ':base()'", OptionC = "Creating a chain of objects", OptionD = "Overloading the constructor",
                CorrectOption = 'B', Explanation = "Constructor chaining is when one constructor calls another using ':this()' (same class) or ':base()' (parent class) to avoid code duplication.",
                Difficulty = Difficulty.Hard, Tags = "constructors,chaining", Status = ContentStatus.Approved, CreatedById = adminId
            },
            new McqQuestion
            {
                ChapterId = chClassesObjects.Id, Question = "What is the init accessor introduced in C# 9?",
                OptionA = "A constructor replacement", OptionB = "An accessor that allows property setting only during object initialization", OptionC = "A new type of field", OptionD = "A destructor alternative",
                CorrectOption = 'B', Explanation = "The 'init' accessor allows a property to be set during object initialization (constructor or object initializer) but becomes read-only after that, supporting immutability patterns.",
                Difficulty = Difficulty.Hard, Tags = "init,properties,csharp9", Status = ContentStatus.Approved, CreatedById = adminId
            }
        );

        // 8 Interview Questions for "Classes & Objects"
        context.InterviewQuestions.AddRange(
            new InterviewQuestion
            {
                ChapterId = chClassesObjects.Id, Question = "What is the difference between a class and an object?",
                ModelAnswer = "A class is a blueprint/template that defines the structure (fields, properties) and behavior (methods) of objects. An object is a concrete instance of a class created at runtime using the 'new' keyword. A class exists in code; an object exists in memory. For example, 'Car' is a class, while 'myCar = new Car()' creates an object.",
                Difficulty = Difficulty.Easy, Frequency = Frequency.Common, Tags = "fundamentals", Status = ContentStatus.Approved, CreatedById = adminId
            },
            new InterviewQuestion
            {
                ChapterId = chClassesObjects.Id, Question = "Explain the difference between value types and reference types in C#.",
                ModelAnswer = "Value types (int, bool, struct, enum) store data directly on the stack and contain the actual value. When assigned or passed, a copy is made. Reference types (class, string, array, delegate) store a reference (pointer) on the stack that points to the actual data on the heap. When assigned, only the reference is copied, so both variables point to the same object. This affects equality comparisons, memory allocation, and garbage collection behavior.",
                Difficulty = Difficulty.Medium, Frequency = Frequency.Common, Tags = "memory,types", Status = ContentStatus.Approved, CreatedById = adminId
            },
            new InterviewQuestion
            {
                ChapterId = chClassesObjects.Id, Question = "What are constructors? Explain different types of constructors in C#.",
                ModelAnswer = "Constructors are special methods called when an object is created. Types: (1) Default Constructor - parameterless, provided by compiler if none defined. (2) Parameterized Constructor - accepts parameters to initialize fields. (3) Copy Constructor - creates object by copying another object. (4) Static Constructor - initializes static members, called once before any static member access, cannot have parameters or access modifiers. (5) Private Constructor - prevents direct instantiation, used in Singleton pattern.",
                Difficulty = Difficulty.Medium, Frequency = Frequency.Common, Tags = "constructors", Status = ContentStatus.Approved, CreatedById = adminId
            },
            new InterviewQuestion
            {
                ChapterId = chClassesObjects.Id, Question = "What is the difference between a class and a struct in C#?",
                ModelAnswer = "Key differences: (1) Class is a reference type (heap), struct is a value type (stack). (2) Class supports inheritance, struct does not (but can implement interfaces). (3) Class can have a destructor/finalizer, struct cannot. (4) Class members default to private, struct members also default to private. (5) Class can have a parameterless constructor explicitly, struct gets one implicitly. (6) Class instances are passed by reference, struct by value (copied). Use struct for small, immutable data; class for complex behavior and inheritance.",
                Difficulty = Difficulty.Medium, Frequency = Frequency.Common, Tags = "class-vs-struct", Status = ContentStatus.Approved, CreatedById = adminId
            },
            new InterviewQuestion
            {
                ChapterId = chClassesObjects.Id, Question = "What is the 'static' keyword and when would you use it?",
                ModelAnswer = "The 'static' keyword means the member belongs to the type itself, not to instances. Static members are shared across all instances. Use cases: (1) Utility/helper methods (Math.Max). (2) Constants and configuration. (3) Singleton pattern. (4) Extension methods must be in static classes. (5) Static constructors for one-time initialization. Static classes cannot be instantiated and can only contain static members. Overuse of static can make code harder to test and violate SOLID principles.",
                Difficulty = Difficulty.Medium, Frequency = Frequency.Occasional, Tags = "static", Status = ContentStatus.Approved, CreatedById = adminId
            },
            new InterviewQuestion
            {
                ChapterId = chClassesObjects.Id, Question = "Explain properties in C# and how they differ from fields.",
                ModelAnswer = "Fields are variables declared directly in a class. Properties are members that provide a flexible mechanism to read, write, or compute values using get/set accessors. Properties can: (1) Add validation logic in setters. (2) Be made read-only (only getter), write-only (only setter), or init-only (C# 9). (3) Be auto-implemented for simple cases. (4) Be used in data binding and serialization. Properties are preferred over public fields because they maintain encapsulation and allow adding logic without breaking the public API.",
                Difficulty = Difficulty.Easy, Frequency = Frequency.Common, Tags = "properties,encapsulation", Status = ContentStatus.Approved, CreatedById = adminId
            },
            new InterviewQuestion
            {
                ChapterId = chClassesObjects.Id, Question = "What are Records in C# and when would you use them?",
                ModelAnswer = "Records (C# 9+) are reference types designed for immutable data. Key features: (1) Value-based equality - two records with same data are considered equal. (2) Built-in ToString() with property values. (3) Positional syntax: 'record Person(string Name, int Age)'. (4) Non-destructive mutation with 'with' expressions. (5) record struct (C# 10) for value-type records. Use records for DTOs, events, API responses, and any scenario needing immutable data with value semantics. They reduce boilerplate compared to manually implementing Equals, GetHashCode, and ToString.",
                Difficulty = Difficulty.Hard, Frequency = Frequency.Occasional, Tags = "records,immutability", Status = ContentStatus.Approved, CreatedById = adminId
            },
            new InterviewQuestion
            {
                ChapterId = chClassesObjects.Id, Question = "Explain garbage collection in .NET and how it relates to object lifecycle.",
                ModelAnswer = "The .NET Garbage Collector (GC) automatically manages memory by freeing objects that are no longer referenced. It uses a generational approach: Gen 0 (short-lived), Gen 1 (medium), Gen 2 (long-lived). Process: (1) GC identifies unreachable objects (no references). (2) Compacts memory by moving surviving objects. (3) Updates references. Objects with finalizers go to a finalization queue, adding overhead. Best practices: Implement IDisposable for unmanaged resources, use 'using' statements, avoid finalizers when possible, don't force GC.Collect() in production. The Large Object Heap (LOH) handles objects > 85KB separately.",
                Difficulty = Difficulty.Hard, Frequency = Frequency.Occasional, Tags = "gc,memory,lifecycle", Status = ContentStatus.Approved, CreatedById = adminId
            }
        );

        // 5 MCQs for "Collections & LINQ > LINQ Basics"
        context.McqQuestions.AddRange(
            new McqQuestion
            {
                ChapterId = chLinqBasics.Id, Question = "What does LINQ stand for?",
                OptionA = "Language Integrated Native Query", OptionB = "Language Integrated Query", OptionC = "Linked Integrated Query", OptionD = "Language Internal Query",
                CorrectOption = 'B', Explanation = "LINQ stands for Language Integrated Query. It provides a consistent query syntax directly in C# to query various data sources (collections, databases, XML, etc.).",
                Difficulty = Difficulty.Easy, Tags = "linq,basics", Status = ContentStatus.Approved, CreatedById = adminId
            },
            new McqQuestion
            {
                ChapterId = chLinqBasics.Id, Question = "Which LINQ method is used to filter elements based on a condition?",
                OptionA = "Select", OptionB = "OrderBy", OptionC = "Where", OptionD = "GroupBy",
                CorrectOption = 'C', Explanation = "The 'Where' method filters a sequence based on a predicate. It returns only the elements that satisfy the given condition.",
                Difficulty = Difficulty.Easy, Tags = "linq,where,filtering", Status = ContentStatus.Approved, CreatedById = adminId
            },
            new McqQuestion
            {
                ChapterId = chLinqBasics.Id, Question = "What is deferred execution in LINQ?",
                OptionA = "The query executes immediately when defined", OptionB = "The query execution is postponed until the result is actually needed", OptionC = "The query runs in a separate thread", OptionD = "The query results are cached",
                CorrectOption = 'B', Explanation = "Deferred execution means the query is not executed when it's defined. It's executed when you iterate over the result (e.g., foreach, ToList(), Count()). This allows building up queries and only executing them when results are needed.",
                Difficulty = Difficulty.Medium, Tags = "linq,deferred-execution", Status = ContentStatus.Approved, CreatedById = adminId
            },
            new McqQuestion
            {
                ChapterId = chLinqBasics.Id, Question = "Which LINQ method forces immediate execution and returns a List<T>?",
                OptionA = "AsEnumerable()", OptionB = "Where()", OptionC = "Select()", OptionD = "ToList()",
                CorrectOption = 'D', Explanation = "ToList() forces immediate execution of the LINQ query and materializes the results into a List<T>. Other immediate execution methods include ToArray(), ToDictionary(), Count(), First(), etc.",
                Difficulty = Difficulty.Medium, Tags = "linq,execution", Status = ContentStatus.Approved, CreatedById = adminId
            },
            new McqQuestion
            {
                ChapterId = chLinqBasics.Id, Question = "What is the difference between Select and SelectMany in LINQ?",
                OptionA = "Select projects each element; SelectMany flattens nested collections", OptionB = "They are identical", OptionC = "SelectMany selects multiple columns", OptionD = "Select works only with arrays",
                CorrectOption = 'A', Explanation = "Select projects each element to a new form (1-to-1 mapping). SelectMany projects each element to a collection and flattens the resulting collections into one sequence. For example, if each customer has a list of orders, SelectMany gives you all orders from all customers in a flat list.",
                Difficulty = Difficulty.Hard, Tags = "linq,select,selectmany", Status = ContentStatus.Approved, CreatedById = adminId
            }
        );

        // 5 Interview Questions for "ASP.NET Core > Dependency Injection"
        context.InterviewQuestions.AddRange(
            new InterviewQuestion
            {
                ChapterId = chDI.Id, Question = "What is Dependency Injection and why is it important?",
                ModelAnswer = "Dependency Injection (DI) is a design pattern where an object receives its dependencies from an external source rather than creating them itself. It's a form of Inversion of Control (IoC). Benefits: (1) Loose coupling between classes. (2) Easier unit testing with mock dependencies. (3) Better separation of concerns. (4) More maintainable and flexible code. In ASP.NET Core, DI is built into the framework through the IServiceProvider and configured in Program.cs.",
                Difficulty = Difficulty.Easy, Frequency = Frequency.Common, Tags = "di,ioc,fundamentals", Status = ContentStatus.Approved, CreatedById = adminId
            },
            new InterviewQuestion
            {
                ChapterId = chDI.Id, Question = "Explain the three service lifetimes in ASP.NET Core DI.",
                ModelAnswer = "ASP.NET Core DI supports three lifetimes: (1) Transient (AddTransient) - A new instance is created every time the service is requested. Best for lightweight, stateless services. (2) Scoped (AddScoped) - One instance per HTTP request/scope. Shared within a request but not across requests. Best for DbContext and per-request services. (3) Singleton (AddSingleton) - One instance for the entire application lifetime. Shared across all requests and threads. Best for configuration, caches, and thread-safe services. Caution: Never inject a Scoped service into a Singleton — it causes a 'captive dependency' bug.",
                Difficulty = Difficulty.Medium, Frequency = Frequency.Common, Tags = "di,lifetime,scoped,transient,singleton", Status = ContentStatus.Approved, CreatedById = adminId
            },
            new InterviewQuestion
            {
                ChapterId = chDI.Id, Question = "What is the difference between AddTransient, AddScoped, and AddSingleton?",
                ModelAnswer = "AddTransient: Creates a new instance each time. Use for stateless services. AddScoped: Creates one instance per request scope. Ideal for EF Core DbContext. AddSingleton: Creates one instance for app lifetime. Use for thread-safe, shared state. Example: If a Transient service is injected in 3 places within one request, 3 instances are created. A Scoped service would be the same instance in all 3. A Singleton is the same instance across all requests.",
                Difficulty = Difficulty.Medium, Frequency = Frequency.Common, Tags = "di,service-registration", Status = ContentStatus.Approved, CreatedById = adminId
            },
            new InterviewQuestion
            {
                ChapterId = chDI.Id, Question = "How do you register and resolve services in ASP.NET Core?",
                ModelAnswer = "Registration in Program.cs: builder.Services.AddScoped<IMyService, MyService>(). Resolution: (1) Constructor Injection (preferred) - declare the interface in the constructor. (2) Method Injection via [FromServices] attribute in action methods. (3) Service Locator (anti-pattern) via IServiceProvider.GetService<T>(). (4) HttpContext.RequestServices for middleware. Best practice is constructor injection as it makes dependencies explicit, enables compile-time checking, and simplifies testing.",
                Difficulty = Difficulty.Medium, Frequency = Frequency.Common, Tags = "di,registration,resolution", Status = ContentStatus.Approved, CreatedById = adminId
            },
            new InterviewQuestion
            {
                ChapterId = chDI.Id, Question = "What is the Service Locator anti-pattern and why should you avoid it?",
                ModelAnswer = "Service Locator is when code explicitly asks a container for dependencies using IServiceProvider.GetService<T>() instead of receiving them through constructors. It's an anti-pattern because: (1) Hides dependencies — you can't see what a class needs from its constructor. (2) Makes testing harder — must set up a full container. (3) Runtime errors instead of compile-time — missing registration only fails at runtime. (4) Violates Explicit Dependencies Principle. Prefer constructor injection which makes dependencies visible, testable, and fails fast at startup if not registered.",
                Difficulty = Difficulty.Hard, Frequency = Frequency.Occasional, Tags = "di,anti-pattern,service-locator", Status = ContentStatus.Approved, CreatedById = adminId
            }
        );

        await context.SaveChangesAsync();
    }

    private static async Task SeedHiringStagesAsync(PrepHubDbContext context)
    {
        if (await context.HiringStages.AnyAsync())
            return;

        context.HiringStages.AddRange(
            new HiringStage { BatchOrCompany = "Mouser Electronics", Title = "Phase 1 training completed", StageDate = new DateOnly(2026, 11, 25), SortOrder = 1 },
            new HiringStage { BatchOrCompany = "Mouser Electronics", Title = "Psychometric test on SHL portal", StageDate = new DateOnly(2026, 11, 30), SortOrder = 2 },
            new HiringStage { BatchOrCompany = "Mouser Electronics", Title = "Technical assessment on SHL platform at Mouser premise", StageDate = new DateOnly(2026, 12, 1), SortOrder = 3 },
            new HiringStage { BatchOrCompany = "Mouser Electronics", Title = "Technical assessment results analysis by Mouser team", StageDate = new DateOnly(2026, 12, 4), SortOrder = 4 },
            new HiringStage { BatchOrCompany = "Mouser Electronics", Title = "In-person interviews at Mouser premise", StageDate = new DateOnly(2026, 12, 8), SortOrder = 5 }
        );

        await context.SaveChangesAsync();
    }
}
