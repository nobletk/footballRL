namespace Assets.Scripts
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class GoalLine : MonoBehaviour
    {
        public ScoreManager localManager;
        [Tooltip("Team Blue ID = 0, Team Red ID = 1")]
        public int scoringTeam;

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Ball"))
            {
                localManager.AddScore(scoringTeam);
            }
        }
    }
}
