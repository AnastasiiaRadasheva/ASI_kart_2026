using UnityEngine;
using UnityEngine.Events;

namespace Sain.TougeRacer
{
    public class RaceEvents
    {
        public static UnityEvent SpawnRacersEvent = new();
        public static UnityEvent<int> CountdownEvent = new();
        public static UnityEvent<int> UpdatePlayerPositionEvent = new();
        public static UnityEvent<string> UpdateRaceProgressEvent = new();
        public static UnityEvent PauseEvent = new();
        public static UnityEvent ResumeEvent = new();

        public static UnityEvent RacePreviewEvent = new();
        public static UnityEvent RaceCountdownEvent = new();
        public static UnityEvent RaceStartEvent = new();
        public static UnityEvent RaceFinishEvent = new();
        public static UnityEvent RaceEndEvent = new();
        public static UnityEvent<float> RaceTimerEvent = new();
        public static UnityEvent<int, float> RaceResultEvent = new();

        public static UnityEvent<FinishLine, CarController> ThroughFinishLineEvent = new();
        public static UnityEvent<Sprite, float> ShowGuideEvent = new();
    }
}
