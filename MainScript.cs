using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class MainScript : MonoBehaviour
    {
        [SerializeField]
        private RobotControl robot = null;
        [SerializeField]
        private GameObject gear = null, plane = null, playAgainButton;
        [SerializeField]
        private Text scoreLabel, dialogLabel;
        [SerializeField]
        private float timeBetweenObstacle, dialogDuration, superPowerDuration;

        private const int SPAWN_GEARS_ONLY = 2;
        private const int RANDOM_CHOICE_ITEMS_MIN = 0;
        private const int RANDOM_CHOICE_ITEMS_MAX = 15;
        private const float RANDOM_X_SINGLE_MIN = -3.75f;
        private const float RANDOM_X_SINGLE_MAX = 3.7f;
        private const float RANDOM_X_DOUBLE_GEARS_MAX = -0.5f;
        private const float RANDOM_X_DOUBLE_GEARS_MIN = 0.5f;
        private const float RANDOM_X_DOUBLE_PLANES_MAX = -2.3f;
        private const float RANDOM_X_DOUBLE_PLANES_MIN = 2.3f;
        private const float OBSTACLES_Y = -9.58f;
        private const float OBSTACLES_Z = 0.0f;
        private const float ADD_SPEED = 0.4f;
        private const float MIN_TIME_BETWEEN_OBSTACLES = 0.8f;
        private const float REDUCING_DISTANS_BETWEEN_OBSTACLES_CONST = 0.01f;
        private const float ZERO = 0.0f;
        private const float ONE_HUNDREDTH = 0.01f;
        private const string TEXT_TIME_TO_PLAY = "Time to play!";
        private const string TEXT_PRESS_SPACE = "Press SPACE to start your fall";
        private const string TEXT_GAME_OVER_SHAME = "GAME OVER! \nSo shame...";
        private const string TEXT_GAME_OVER_FRIENDS = "GAME OVER! \nShhh... don't tell friends!";
        private const string TEXT_DOUBLE_GEARS = "Catch them all!";
        private const string TEXT_DOUBLE_PLANES = "That planes really shouldn't be here...";
        private const string TEXT_WATCH_OUT = "Watch out!";
        private const string TEXT_SUPER_POWER_ENABLED = "Super Power enabled!\nGears instead planes!\nPress SPACE to use it!\nTime limited!";
        private const string TEXT_STAY_INSIDE = "Stay inside!";

        private Vector3 robotInitPosision = new Vector3(-0.12f, 8.4f, 0.00f);
        private Vector3 robotInitRotation = new Vector3(0.00f, 0.00f, 180.00f);
        private Vector3 robotInitLocalScale = new Vector3(0.41f, 0.30f, 1.00f);
        private enum GameState
        {
            GAME_INIT,
            GAME_PLAYING,
            GAME_OVER,
        }
        private GameState gameState = GameState.GAME_INIT;
        private bool superPower;
        private bool superPowerEnable;
        private int score;
        private float speedChange;
        private float timer;
        private float timerDialog;
        private float timerSuperPower;
        private float gearCounter;
    
        private void Start ()
        {
            InitGame();
        }

        private void InitGame()
        {
            robot.transform.position = robotInitPosision; 
            robot.transform.rotation = new Quaternion(robotInitRotation.x, robotInitRotation.y, robotInitRotation.z, robot.transform.rotation.w);
            robot.transform.localScale = robotInitLocalScale;
            timer = 0.00f;
            timerDialog = 0.0f;
            timerSuperPower = 0.0f;
            score = 0;
            Time.timeScale = 1f;
            speedChange = 5;
            playAgainButton.SetActive(false);
            gearCounter = 0;
            superPower = false;
            superPowerEnable = false;
        }
	
        private void WaitForStart()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                gameState = GameState.GAME_PLAYING;
                dialogLabel.text = TEXT_TIME_TO_PLAY;
                timerDialog = dialogDuration;
            }
            else
            {
                dialogLabel.text = TEXT_PRESS_SPACE;
                timerDialog = dialogDuration;
            }
        }

        private void Spawn(int i, Vector3 vec)
        {
            if (i == 1)
            {
                GameObject obstacle = Instantiate(gear) as GameObject;
                obstacle.transform.localPosition = vec;
                obstacle.GetComponent<MovingObstacle>().setSpeed(speedChange);
            }
            else if (i == 2)
            {
                GameObject obstacle = Instantiate(plane) as GameObject;
                obstacle.transform.localPosition = vec;
                obstacle.GetComponent<MovingObstacle>().setSpeed(speedChange);
            }
        }

        private void Gameplay()
        {
            timer += Time.deltaTime;

            if (timer > timeBetweenObstacle)
            {
                int choice;
                if (superPower == false)
                    choice = Random.Range(RANDOM_CHOICE_ITEMS_MIN, RANDOM_CHOICE_ITEMS_MAX);
                else
                    choice = SPAWN_GEARS_ONLY;

                if (choice < 6) // gears (collectible items) 
                {
                    if (choice == 1) // double 
                    {
                        dialogLabel.text = TEXT_DOUBLE_GEARS;
                        timerDialog = dialogDuration;
                        Spawn(1, new Vector3(Random.Range(RANDOM_X_SINGLE_MIN, RANDOM_X_DOUBLE_GEARS_MAX), OBSTACLES_Y, OBSTACLES_Z));
                        Spawn(1, new Vector3(Random.Range(RANDOM_X_DOUBLE_GEARS_MIN, RANDOM_X_SINGLE_MAX), OBSTACLES_Y, OBSTACLES_Z));
                    }
                    else // single
                    {
                        Spawn(1, new Vector3(Random.Range(RANDOM_X_SINGLE_MIN, RANDOM_X_SINGLE_MAX), OBSTACLES_Y, OBSTACLES_Z));
                    }
                }
                else // planes (obstacles)
                {
                    if (choice == 8) // double
                    {
                        dialogLabel.text = TEXT_DOUBLE_PLANES;
                        timerDialog = dialogDuration;
                        Spawn(2, new Vector3(Random.Range(RANDOM_X_SINGLE_MIN, RANDOM_X_DOUBLE_PLANES_MAX), OBSTACLES_Y, OBSTACLES_Z));
                        Spawn(2, new Vector3(Random.Range(RANDOM_X_DOUBLE_PLANES_MIN, RANDOM_X_SINGLE_MAX), OBSTACLES_Y, OBSTACLES_Z));
                    }
                    else // single
                    {
                        Spawn(2, new Vector3(Random.Range(RANDOM_X_SINGLE_MIN, RANDOM_X_SINGLE_MAX), OBSTACLES_Y, OBSTACLES_Z));

                        if (choice == 7)
                        {
                            dialogLabel.text = TEXT_WATCH_OUT;
                            timerDialog = dialogDuration;
                        }
                    }
                }

                speedChange += ADD_SPEED;

                if (timeBetweenObstacle > MIN_TIME_BETWEEN_OBSTACLES)
                    timeBetweenObstacle -= ONE_HUNDREDTH * speedChange;

                timer -= timeBetweenObstacle;
            }

            // text visibility
            if (timerDialog < ZERO)
            {
                dialogLabel.text = "";
            }
            else if (timerDialog > ZERO)
            {
                timerDialog -= ONE_HUNDREDTH;
            }

            // Super Power
            if ((gearCounter == 5) && (superPowerEnable == false))
            {
                dialogLabel.text = TEXT_SUPER_POWER_ENABLED;
                timerDialog = dialogDuration;
                superPowerEnable = true;
            }

            // Super Power activation
            if (superPowerEnable == true)
            {
                if (Input.GetKey(KeyCode.Space))
                {
                    superPower = true;
                    superPowerEnable = false;
                    gearCounter++; 
                    timerSuperPower = superPowerDuration;
                }
            }

            // Super Power duration
            if (superPower == true)
            {
                if (timerSuperPower < ZERO)
                {
                    superPower = false;
                    gearCounter = 0;
                }
                else if (timerSuperPower > ZERO)
                {
                    timerSuperPower -= ONE_HUNDREDTH;
                }
            }
        }

        private void Update ()
        {
            switch (gameState)
            {
                case GameState.GAME_INIT:
                    WaitForStart();
                    break;

                case GameState.GAME_PLAYING:
                    Gameplay();
                    break;

                case GameState.GAME_OVER:
                    TheEnd();
                    break;
            }
        }

        private void TheEnd()
        {
            playAgainButton.SetActive(true);

            if (score > 20)
                dialogLabel.text = TEXT_GAME_OVER_SHAME;
            else
            {
                dialogLabel.text = TEXT_GAME_OVER_FRIENDS;
            }
            robot.playCrushAnimation();
        }

        public void RobotCollision()
        {
            Time.timeScale = 0f;
            robot.playCrushAnimation();
            gameState = GameState.GAME_OVER;
        }

        public void AddPoint()
        {
            score++;
            scoreLabel.text = score.ToString();
        }

        public void ButtonPlayAgainClick()
        {
            SceneManager.LoadScene("game");
        }

        public void WallCollision()
        {
            dialogLabel.text = TEXT_STAY_INSIDE;
            timerDialog = dialogDuration;
        }

        public void IncreaseGearCounter()
        {
            gearCounter++;
        }
    }
}
