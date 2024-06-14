
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

[Serializable]
public class ProfileData 
{
    [JsonProperty("ProfileName")]
    public string ProfileName;

    [JsonProperty("UserLessons")]
    public List<UserLesson> UserLessons;
}

[Serializable]
public class UserLesson
{
    [JsonProperty("hasClearedLesson")]
    public bool hasClearedLesson;

    [JsonProperty("hasClearedQuiz")]
    public bool hasClearedQuiz;

    [JsonProperty("hasWatchVideo")]
    public bool hasWatchVideo;
}

