using UnityEngine;

namespace CBS.Scriptable
{
    [CreateAssetMenu(fileName = "CommonPrefabs", menuName = "CBS/Add new Common Prefabs")]
    public class CommonPrefabs : CBSScriptable
    {
        public override string ResourcePath => "Scriptable/CommonPrefabs";

        public GameObject Canvas;
        public GameObject Lessons;
        public GameObject QuizUI;
        public GameObject FlashCardUI;
        public GameObject Setting;
        public GameObject LessonDetail;
        public GameObject GameContext;
    }
}
