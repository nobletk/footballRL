namespace Assets.Scripts
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;

    public class ScoreManager : MonoBehaviour
    {
        [Header("Scores")]
        public int teamBlueScore = 0;
        public int teamRedScore = 0;

        [Header("UI Elements")]
        public TextMeshProUGUI blueScoreText;
        public TextMeshProUGUI redScoreText;
        
        [Header("RL Environment Variables")]
        public Transform ball;
        [Tooltip("Team Blue ID = 0")]
        public Player teamBlueAgent;
        [Tooltip("Team Red ID = 1")]
        public Player teamRedAgent;

        private Rigidbody ballRb;
        private Vector3 ballStartPos;

        void Awake()
        {
            if (ball != null)
            {
                ballRb = ball.GetComponent<Rigidbody>();
                ballStartPos = ball.position;
            }
        }

        void Start()
        {
            UpdateScoreUI();
        }

        public void AddScore(int teamID)
        {
            // Team Blue ID = 0
            if (teamID == 0)
            {
                teamBlueScore++;
                teamBlueAgent.AddReward(1.0f);
                teamRedAgent.AddReward(-1.0f);
            }
            // Team Red ID = 1
            else if (teamID == 1)
            {
                teamRedScore++;
                teamRedAgent.AddReward(1.0f);
                teamBlueAgent.AddReward(-1.0f);
            }

            UpdateScoreUI();
            Debug.Log($"Goal! Score is now Team Blue: {teamBlueScore} | Team Red: {teamRedScore}");

            ResetEnvironment();
            teamBlueAgent.EndEpisode();
            teamRedAgent.EndEpisode();
        }

        private void UpdateScoreUI()
        {
            if (blueScoreText != null)
            {
                blueScoreText.text = teamBlueScore.ToString();
            }

            if (redScoreText != null)
            {
                redScoreText.text = teamRedScore.ToString();
            }
        }

        public void ResetEnvironment()
        {
            if (ball != null)
            {
                ballRb.velocity = Vector3.zero;
                ballRb.angularVelocity = Vector3.zero;
                ball.position = ballStartPos;
            }
        }
    }
}
