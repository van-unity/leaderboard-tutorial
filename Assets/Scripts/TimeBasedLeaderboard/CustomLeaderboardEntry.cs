using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Unity.Services.Leaderboards.Models;

namespace TimeBasedLeaderboard {
    public class CustomLeaderboardEntry {
        public string PlayerID { get; }
        public string PlayerName { get; }
        public int Rank { get; }
        public double Score { get; }
        public DateTime SubmitDate { get; }

        private CustomLeaderboardEntry(string playerID, string playerName, int rank, double score, DateTime submitDate) {
            PlayerID = playerID;
            PlayerName = playerName;
            Rank = rank + 1; //rank is 0 based
            Score = score;
            SubmitDate = submitDate;
        }

        public static CustomLeaderboardEntry FromUnityLeaderboardEntry(LeaderboardEntry leaderboardEntry) {
            DateTime submitTime = default;

            if (!string.IsNullOrEmpty(leaderboardEntry.Metadata)) {
                try {
                    var metadata = JsonConvert.DeserializeObject<Dictionary<string, string>>(leaderboardEntry.Metadata);

                    if (metadata.TryGetValue("date", out var dateString)) {
                        submitTime = DateTime.Parse(dateString, null, DateTimeStyles.RoundtripKind);
                    }
                }
                catch {
                    //ignore
                }
            }

            return new CustomLeaderboardEntry(leaderboardEntry.PlayerId, leaderboardEntry.PlayerName,
                leaderboardEntry.Rank, leaderboardEntry.Score, submitTime);
        }
    }
}