﻿
namespace Quackmageddon
{
    /// <summary>
    /// Contains gameplay event types strings
    /// </summary>
    public class GameplayEventType
    {
        public const string EnemyHit = "Enemy hit";
        public const string EnemyDestroyed = "Enemy destroyed";
        public const string EnemyBeakshot = "Enemy beakshot";

        public const string PlayerHit = "Player hit";

        public const string ScoreUpdate = "Score update";
        public const string HealthUpdate = "Health update";

        public const string PauseSpawning = "Pause spawning";
        public const string ResumeSpawning = "Resume spawning";
    }
}