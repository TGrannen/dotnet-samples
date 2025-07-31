using Microsoft.EntityFrameworkCore;

namespace TickerQExample.WebAPI.Data;

public class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
{
}