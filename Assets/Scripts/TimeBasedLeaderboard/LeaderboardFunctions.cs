using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Leaderboards;

namespace TimeBasedLeaderboard {
    public static class LeaderboardFunctions {
        public static object GenerateMetadata() => new Dictionary<string, string> {
            { CustomLeaderboardEntry.DATE_KEY, DateTime.UtcNow.ToString("o") }
        };

        public static async Task<Result<CustomLeaderboardEntry>> SubmitScoreAsync(string leaderboardID, double score,
            object metadata) {
            var submitTask = LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardID, score,
                new AddPlayerScoreOptions { Metadata = metadata });

            try {
                var result = await submitTask;

                return Result<CustomLeaderboardEntry>.Success(CustomLeaderboardEntry.FromUnityLeaderboardEntry(result));
            }
            catch (Exception e) {
                return Result<CustomLeaderboardEntry>.Failure(e.Message ?? "Failed to submit score.");
            }
        }

        public static async Task<Result<IEnumerable<CustomLeaderboardEntry>>> GetAllTimeEntries(string leaderboardID) {
            var getEntriesTask =
                LeaderboardsService.Instance.GetScoresAsync(leaderboardID,
                    new GetScoresOptions() { IncludeMetadata = true });

            try {
                var result = await getEntriesTask;
                return Result<IEnumerable<CustomLeaderboardEntry>>.Success(
                    result.Results.Select(CustomLeaderboardEntry.FromUnityLeaderboardEntry)
                );
            }
            catch (Exception exception) {
                return Result<IEnumerable<CustomLeaderboardEntry>>.Failure(exception.Message ?? "Failed to fetch entries.");
            }
        }

        public static IEnumerable<CustomLeaderboardEntry> SelectDailyEntries(
            this IEnumerable<CustomLeaderboardEntry> allEntries) {
            if (allEntries == null) {
                return null;
            }

            try {
                return allEntries.Where(entry => entry.SubmitDate.Date == DateTime.UtcNow.Date);
            }
            catch {
                return Enumerable.Empty<CustomLeaderboardEntry>();
            }
        }

        public static IEnumerable<CustomLeaderboardEntry> SelectWeeklyEntries(
            this IEnumerable<CustomLeaderboardEntry> allEntries
        ) {
            if (allEntries == null) {
                return null;
            }

            try {
                return allEntries.Where(entry => entry.SubmitDate >= DateTime.UtcNow.AddDays(-7));
            }
            catch {
                return Enumerable.Empty<CustomLeaderboardEntry>();
            }
        }

        public static IEnumerable<CustomLeaderboardEntry> SelectMonthlyEntries(
            this IEnumerable<CustomLeaderboardEntry> allEntries
        ) {
            if (allEntries == null) {
                return null;
            }

            try {
                return allEntries.Where(entry => entry.SubmitDate >= DateTime.UtcNow.AddMonths(-1));
            }
            catch {
                return Enumerable.Empty<CustomLeaderboardEntry>();
            }
        }
    }
}