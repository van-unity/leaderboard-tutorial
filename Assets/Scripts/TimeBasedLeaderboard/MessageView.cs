using System.Collections;
using TMPro;
using UnityEngine;

namespace TimeBasedLeaderboard {
    public class MessageView : MonoBehaviour {
        [SerializeField] private Color _errorColor = Color.red;
        [SerializeField] private Color _successColor = Color.green;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private float _hideDelay;

        private YieldInstruction _hideDelayYieldInstruction;

        private void Start() {
            _text.enabled = false;
            _hideDelayYieldInstruction = new WaitForSeconds(_hideDelay);
        }

        public void ShowSuccessMessage(string message) {
            StopAllCoroutines();

            StartCoroutine(ShowMessage(message, _successColor));
        }

        public void ShowErrorMessage(string message) {
            StopAllCoroutines();

            StartCoroutine(ShowMessage(message, _errorColor));
        }

        private IEnumerator ShowMessage(string message, Color color) {
            _text.enabled = true;
            _text.text = message;
            _text.color = color;

            yield return _hideDelayYieldInstruction;

            _text.enabled = false;
        }
    }
}