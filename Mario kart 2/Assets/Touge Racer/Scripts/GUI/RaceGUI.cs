using Sain.Utils;
using TMPro;
using UnityEngine;

namespace Sain.TougeRacer
{
    public class RaceGUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text countdownText;
        [SerializeField] private TMP_Text raceProgressText;
        [SerializeField] private TMP_Text positionText;
        [SerializeField] private TMP_Text raceTimerText;
        [Header("Result")]
        [SerializeField] private GameObject raceResult;
        [SerializeField] private TMP_Text resultPosText;
        [SerializeField] private TMP_Text resultTimeText;

        void OnEnable()
        {
            RaceEvents.CountdownEvent.AddListener(UpdateCountdownText);
            RaceEvents.UpdateRaceProgressEvent.AddListener(UpdateRaceProgressText);
            RaceEvents.UpdatePlayerPositionEvent.AddListener(UpdatePositionText);
            RaceEvents.RaceTimerEvent.AddListener(UpdateRaceTimerText);
            RaceEvents.RaceResultEvent.AddListener(UpdateRaceResult);
        }

        void OnDisable()
        {
            RaceEvents.CountdownEvent.RemoveListener(UpdateCountdownText);
            RaceEvents.UpdateRaceProgressEvent.RemoveListener(UpdateRaceProgressText);
            RaceEvents.UpdatePlayerPositionEvent.RemoveListener(UpdatePositionText);
            RaceEvents.RaceTimerEvent.RemoveListener(UpdateRaceTimerText);
            RaceEvents.RaceResultEvent.RemoveListener(UpdateRaceResult);
        }

        private void UpdateCountdownText(int countdown)
        {
            if (countdownText == null) return;
            string countText = countdown.ToString();
            if (countdown == 0) countText = "GO!";
            if (countdown < 0) countText = string.Empty;

            countdownText.text = countText;
        }

        private void UpdateRaceProgressText(string progress)
        {
            if (raceProgressText)
                raceProgressText.text = progress;
        }

        private void UpdatePositionText(int position)
        {
            if (positionText)
                positionText.text = $"{position}{CardinalPos(position)}";
        }

        private void UpdateRaceTimerText(float timeInSeconds)
        {
            if (raceTimerText)
                raceTimerText.text = timeInSeconds.FormatTime();
        }

        private void UpdateRaceResult(int position, float timeInSeconds)
        {
            if (raceResult) raceResult.SetActive(true);
            if (resultPosText) resultPosText.text = $"{position}{CardinalPos(position)}";
            if (resultTimeText) resultTimeText.text = timeInSeconds.FormatTime();
        }

        public static string CardinalPos(int i)
        {
            if (i % 100 >= 11 && i % 100 <= 13)
            {
                return "th";
            }

            switch (i % 10)
            {
                case 1: return "st"; // 1st, 21st, 31st...
                case 2: return "nd"; // 2nd, 22nd, 32nd...
                case 3: return "rd"; // 3rd, 23rd, 33rd...
                default: return "th"; // 4th, 5th, ..., 11th, 12th...
            }
        }
    }
}
