using Microsoft.EntityFrameworkCore;
using Lab_10.Structures;
using System.Collections.Generic;
using System.Diagnostics;

namespace Lab_10.Classes;

public class TickersContext : DbContext
{
    public DbSet<Tickers> Tickers { get; set; }
    public DbSet<Prices> Prices { get; set; }
    public DbSet<TodayCondition> TodayCondition { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql(
            "server=localhost;user=root;password=password;database=Tickers;",
            new MySqlServerVersion(new Version(8, 0, 35))
        );
    }
}