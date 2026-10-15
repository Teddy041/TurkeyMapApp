using Microsoft.EntityFrameworkCore;
using TurkeyMapApp.Data;

namespace TurkeyMapApp.Services;

/// <summary>
/// Veritabanı migration işlemlerini güvenli ve hızlı şekilde uygular.
/// </summary>
public static class DatabaseInitializer
{
    public static async Task ApplyMigrationsAsync(
        ApplicationDbContext db,
        IWebHostEnvironment env,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        await ConfigureSqliteAsync(db, cancellationToken);

        if (env.IsDevelopment())
            await ClearMigrationLockAsync(db, logger, cancellationToken);

        var pending = (await db.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();

        if (pending.Count == 0)
        {
            logger.LogInformation("Veritabanı güncel, migration gerekmiyor.");
            return;
        }

        logger.LogInformation("{Count} migration uygulanıyor...", pending.Count);

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(TimeSpan.FromSeconds(30));

        try
        {
            await db.Database.MigrateAsync(timeoutCts.Token);
        }
        catch (OperationCanceledException)
        {
            throw new InvalidOperationException(
                "Migration zaman aşımına uğradı. Visual Studio veya başka bir dotnet run örneği " +
                "açık olabilir. Tüm süreçleri kapatıp tekrar deneyin.");
        }
    }

    private static async Task ConfigureSqliteAsync(
        ApplicationDbContext db,
        CancellationToken cancellationToken)
    {
        var connection = db.Database.GetDbConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = "PRAGMA busy_timeout = 10000;";
        await command.ExecuteNonQueryAsync(cancellationToken);

        command.CommandText = "PRAGMA journal_mode = WAL;";
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    /// <summary>
    /// Önceki çökme sonrası kalmış migration kilidini temizler.
    /// </summary>
    private static async Task ClearMigrationLockAsync(
        ApplicationDbContext db,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await db.Database.ExecuteSqlRawAsync(
                "DELETE FROM __EFMigrationsLock;", cancellationToken);

            if (deleted > 0)
                logger.LogWarning("Takılı migration kilidi temizlendi.");
        }
        catch
        {
            // __EFMigrationsLock tablosu henüz oluşturulmamış olabilir
        }
    }
}
