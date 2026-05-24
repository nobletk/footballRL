namespace Assets.Scripts
{
    using UnityEngine;
    using Unity.MLAgents;
    using Unity.MLAgents.Actuators;
    using Unity.MLAgents.Sensors;

    public class Player : Agent
    {
        public ScoreManager localManager;
        [Tooltip("Set to 1 for Blue and -1 for Red")]
        public float invertX = 1f;
        public float moveSpeed = 5f;
        public float acceleration = 10f;
        public float deceleration = 5f;
        public float jumpForce = 7f;
        public float rotationSpeed = 10f;
        public float kickForce = 10f;
        public float kickUpwardForce = 2f;

        private Rigidbody rb;
        private bool isGrounded;
        private Vector3 moveDirection;

        private Vector3 startPos;
        private Quaternion startRot;

        private float moveX;
        private float moveZ;
        private bool wantsToJump;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            startPos = transform.position;
            startRot = transform.rotation;
        }

        public override void OnEpisodeBegin()
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.position = startPos;
            transform.rotation = startRot;

            localManager.ResetEnvironment();
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(new Vector3(transform.localPosition.x * invertX, transform.localPosition.y, transform.localPosition.z));
            sensor.AddObservation(new Vector3(rb.velocity.x * invertX, rb.velocity.y, rb.velocity.z));

            if (localManager != null && localManager.ball != null)
            {
                Vector3 ballPos = localManager.ball.localPosition;
                Vector3 ballVel = localManager.ball.GetComponent<Rigidbody>().velocity;
                sensor.AddObservation(new Vector3(ballPos.x * invertX, ballPos.y, ballPos.z));
                sensor.AddObservation(new Vector3(ballVel.x * invertX, ballVel.y, ballVel.z));

                Vector3 directionToBall = (ballPos - transform.localPosition).normalized;
                sensor.AddObservation(new Vector3(directionToBall.x * invertX, directionToBall.y, directionToBall.z));
            }
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            moveX = actions.ContinuousActions[0] * invertX;
            moveZ = actions.ContinuousActions[1];

            wantsToJump = actions.DiscreteActions[0] == 1;

            moveDirection = new Vector3(moveX, 0f, moveZ).normalized;
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var continousActionsOut = actionsOut.ContinuousActions;
            var discreteActionsOut = actionsOut.DiscreteActions;

            continousActionsOut[0] = Input.GetAxisRaw("Horizontal");
            continousActionsOut[1] = Input.GetAxisRaw("Vertical");

            discreteActionsOut[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;
        }

        void Update()
        {
            Jump();
            RotateCharacter();
        }

        void FixedUpdate()
        {
            Move();

            // May be add a tiny negative reward to encourage scoring quickly
            AddReward(-0.0001f);

            if (localManager != null && localManager.ball != null)
            {
                float ballVelocityX = localManager.ball.GetComponent<Rigidbody>().velocity.x;
                float forwardProgress = ballVelocityX * invertX;
                if (forwardProgress > 0.5f)
                {
                    AddReward(0.001f);
                }

                float distanceToBall = Vector3.Distance(transform.localPosition, localManager.ball.localPosition);
                if (distanceToBall < 0.75f)
                {
                    AddReward(0.0005f);
                }
            }
        }

        void Move()
        {
            Vector3 targetVelocity = moveDirection * moveSpeed;
            if (!isGrounded)
            {
                targetVelocity.y = rb.velocity.y;
            }

            rb.velocity = Vector3.Lerp(rb.velocity, targetVelocity, (isGrounded ? acceleration : deceleration) * Time.fixedDeltaTime);
        }

        void RotateCharacter()
        {
            if (moveDirection.magnitude > 0)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        void Jump()
        {
            if (wantsToJump && isGrounded)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                isGrounded = false;
                wantsToJump = false;
            }
        }

        void OnCollisionStay(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out Ground ground))
            {
                isGrounded = true;
            }
        }

        void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out Ground ground))
            {
                isGrounded = false;
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out Ball ball))
            {
                AddReward(0.1f);

                Rigidbody ballRb = collision.gameObject.GetComponent<Rigidbody>();
                if (ballRb != null)
                {
                    Vector3 kickDirection = (collision.transform.position - transform.position).normalized;
                    kickDirection.y += kickUpwardForce;

                    ballRb.AddForce(kickDirection * kickForce, ForceMode.Impulse);
                    ballRb.AddTorque(Vector3.up * kickForce * 0.2f, ForceMode.Impulse);
                }
            }
        }
    }

}

