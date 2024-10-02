using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

namespace TimeBasedLeaderboard {
    public class TimeBasedLeaderboard : MonoBehaviour {
        [SerializeField] private string _leaderboardID;
        [SerializeField] private Toggle _allTimeToggle;
        [SerializeField] private Toggle _dailyToggle;
        [SerializeField] private Toggle _weeklyToggle;
        [SerializeField] private Toggle _monthlyToggle;
        [SerializeField] private LeaderboardEntryWidget _entryWidgetPrefab;
        [SerializeField] private Transform _entriesContainer;
        [SerializeField] private TMP_InputField _scoreInput;
        [SerializeField] private Button _submitScoreButton;
        [SerializeField] private Button _changeUserButton;
        [SerializeField] private MessageView _messageView;

        private List<LeaderboardEntryWidget> _entryWidgets;

        private async void Start() {
            _entryWidgets = new List<LeaderboardEntryWidget>();

            _allTimeToggle.onValueChanged.AddListener(isOn => {
                if (isOn) {
                    OnFilterSelected(LeaderboardFilter.AllTime);
                }
            });

            _dailyToggle.onValueChanged.AddListener(isOn => {
                if (isOn) {
                    OnFilterSelected(LeaderboardFilter.Daily);
                }
            });

            _weeklyToggle.onValueChanged.AddListener(isOn => {
                if (isOn) {
                    OnFilterSelected(LeaderboardFilter.Weekly);
                }
            });

            _monthlyToggle.onValueChanged.AddListener(isOn => {
                if (isOn) {
                    OnFilterSelected(LeaderboardFilter.Monthly);
                }
            });

            _submitScoreButton.onClick.AddListener(SubmitScoreAsync);
            _changeUserButton.onClick.AddListener(ChangeUserAsync);

            await Authenticate();

            var entries = await FetchEntries();

            UpdateEntries(entries);
        }

        private async Task Authenticate() {
            var authenticationResult = await AuthenticationFunctions.Authenticate();

            if (!authenticationResult.IsSuccess) {
                _messageView.ShowErrorMessage(authenticationResult.ErrorMessage);
                return;
            }

            _messageView.ShowSuccessMessage("Authenticated!");
        }

        private async void OnFilterSelected(LeaderboardFilter selectedFilter) {
            await ShowFilteredEntries(selectedFilter);
        }

        private async Task ShowFilteredEntries(LeaderboardFilter filter) {
            var allTimeEntries = await FetchEntries();

            if (allTimeEntries == null) {
                return;
            }

            var filteredEntries = filter switch {
                LeaderboardFilter.Daily => allTimeEntries.SelectDailyEntries(),
                LeaderboardFilter.Weekly => allTimeEntries.SelectWeeklyEntries(),
                LeaderboardFilter.Monthly => allTimeEntries.SelectMonthlyEntries(),
                _ => allTimeEntries
            };

            UpdateEntries(filteredEntries);
        }

        private async Task<IEnumerable<CustomLeaderboardEntry>> FetchEntries() {
            var allTimeEntriesResult = await LeaderboardFunctions.GetAllTimeEntries(_leaderboardID);

            if (allTimeEntriesResult.IsSuccess) {
                _messageView.ShowSuccessMessage("Scores Fetched Successfully");

                return allTimeEntriesResult.Value;
            }

            _messageView.ShowErrorMessage(allTimeEntriesResult.ErrorMessage);

            return null;
        }

        private void UpdateEntries(IEnumerable<CustomLeaderboardEntry> entries) {
            ClearEntries();

            if (entries == null) {
                return;
            }

            foreach (var leaderboardEntry in entries) {
                var entryWidget = Instantiate(_entryWidgetPrefab, _entriesContainer);

                entryWidget.Initialize(
                    leaderboardEntry.Rank.ToString(),
                    leaderboardEntry.Score.ToString("N"),
                    leaderboardEntry.PlayerID == AuthenticationService.Instance.PlayerId
                        ? "YOU"
                        : leaderboardEntry.PlayerName
                );

                _entryWidgets.Add(entryWidget);
            }
        }

        private void ClearEntries() {
            foreach (var entryWidget in _entryWidgets) {
                Destroy(entryWidget.gameObject);
            }

            _entryWidgets.Clear();
        }

        private async void SubmitScoreAsync() {
            if (double.TryParse(_scoreInput.text, out var score)) {
                var metadata = LeaderboardFunctions.GenerateMetadata();
                var submitResult = await LeaderboardFunctions.SubmitScoreAsync(_leaderboardID, score, metadata);

                if (submitResult.IsSuccess) {
                    _messageView.ShowSuccessMessage("Submitted");
                    _scoreInput.text = string.Empty;
                    var entries = await FetchEntries();

                    UpdateEntries(entries);
                } else {
                    _messageView.ShowErrorMessage(submitResult.ErrorMessage);
                }
            }
        }

        private async void ChangeUserAsync() {
            AuthenticationFunctions.SignOut();

            await Task.Yield();

            await Authenticate();
        }
    }
}