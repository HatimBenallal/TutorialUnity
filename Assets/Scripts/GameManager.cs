using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
using UnityEngine.UI;       //Allows us to use Lists. 

    public class GameManager : MonoBehaviour
    {
        public float levelStartDelay = 2f;
        public float turnDealy = .1f;
        public static GameManager instance = null;
       
        //Store a reference to our BoardManager which will set up the level.
        public BoardManager boardScript;
        
        public int playerFoodPoints = 100;
        [HideInInspector] public bool playersTurn = true;
        
        //Current level number, expressed in game as "Day  1".
        private Text levelText;
        private GameObject levelImage;
        private int level = 1;
        private List<Enemy> enemies;
        private bool enemiesMoving;
        private bool doingSetup;

        //Awake is always called before any Start functions
        void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);
            //Get a component reference to the attached BoardManager script
            DontDestroyOnLoad(gameObject);
            enemies = new List<Enemy>();
            boardScript = GetComponent<BoardManager>();

            //Call the InitGame function to initialize the first level 
            InitGame();
        }

        void OnLevelWasLoaded (int index)
        {
            level ++;
            InitGame();
        }

        //Initializes the game for each level.
        void InitGame()
        {
            doingSetup = true;

            levelImage = GameObject.Find("LevelImage");
            levelText = GameObject.Find("LevelText").GetComponent<Text>();
            levelText.text = "Level " + level;
            levelImage.SetActive(true);
            Invoke("HideLevelImage", levelStartDelay);

            enemies.Clear();
            //Call the SetupScene function of the BoardManager script, pass it current level number.
            boardScript.SetupScene(level);

        }

        private void HideLevelImage()
        {
            levelImage.SetActive(false);
            doingSetup = false;
        }

        public void GameOver()
        {
            levelText.text = "After " + level + " levels";
            levelImage.SetActive(true);
            enabled = false;
        }

        //Update is called every frame.
        void Update()
        {
            if (playersTurn || enemiesMoving || doingSetup)
                return;
            StartCoroutine(MoveEnemies());
        }

        public void AddEnemyToList(Enemy script)
        {
            enemies.Add (script);
        }

        IEnumerator MoveEnemies()
        {
            enemiesMoving = true;
            yield return new WaitForSeconds(turnDealy);
            if (enemies.Count == 0)
            {
                yield return new WaitForSeconds(turnDealy);
            }
            for (int i=0; i < enemies.Count; i++)
            {
                enemies[i].MoveEnemy();
                yield return new WaitForSeconds(enemies[i].moveTime);
            }

            playersTurn = true;
            enemiesMoving = false;
        }
    }
