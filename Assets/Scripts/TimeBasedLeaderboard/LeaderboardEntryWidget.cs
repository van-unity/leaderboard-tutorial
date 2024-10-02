using TMPro;
using UnityEngine;

namespace TimeBasedLeaderboard {
    public class LeaderboardEntryWidget : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI _rankText;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _nameText;

        public void Initialize(string rank, string score, string playerName) {
            _rankText.text = rank;
            _scoreText.text = score;
            _nameText.text = playerName;
        }
    }
}