
using UnityEngine;

namespace CBS.Scriptable
{
    [CreateAssetMenu(fileName = "AuthData", menuName = "CBS/Add new Auth Data")]
    public class AuthData : CBSScriptable
    {
        public override string ResourcePath => "Scriptable/Core/AuthData";

        [Header("Preload data after login")]
        public bool PreloadAccountInfo;
    }
}
