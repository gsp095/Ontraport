using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HanumanInstitute.CommonWeb.Tests
{
    /// <summary>
    /// Facilitates the creation of an in-memory database for unit testing.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class InMemoryDbContext
    {
        /// <summary>
        /// Creates a new in-memory database context and returns a DbContextOptions pointing to it.
        /// </summary>
        /// <returns>A DbContextOptions pointing to the in-memory database.</returns>
        public static DbContextOptions<T> GetTestDbOptions<T>()
            where T : DbContext
        {
            // Create a new service provider to create a new in-memory database.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Create a new options instance using an in-memory database and 
            // IServiceProvider that the context should resolve all of its 
            // services from.
            var builder = new DbContextOptionsBuilder<T>()
                .UseInMemoryDatabase("InMemoryDb")
                .UseInternalServiceProvider(serviceProvider);

            return builder.Options;
        }
    }
}
