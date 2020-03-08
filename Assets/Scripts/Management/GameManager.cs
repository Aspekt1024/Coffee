using System;
using UnityEngine;

namespace Coffee
{
    public class GameManager : MonoBehaviour
    {
        
        public static HouseManager House { get; private set; }
        public static DataManager Data { get; private set; }
        public static UIManager UI { get; private set; }
        public static GameplayManager Gameplay { get; private set; }
        public static CharacterManager Character { get; private set; }

        private bool isGameInitialisedAndStarted = false;

        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    var gm = FindObjectOfType<GameManager>();
                    if (gm == null)
                    {
                        Debug.LogWarning("No Game Manager found in scene, although something is trying to access it. If this is a test scene, there's probably nothing to worry about.");
                        return null;
                    }
                    
                    Debug.LogError("Calling GameManager.Instance too early. This is set in Awake(). Use Start()");
                }
                return instance;
            }
        }
        
        private static GameManager instance;

        private void Awake()
        {
            if (instance != null)
            {
                Debug.LogError("Too many GameManagers found in scene. Destroying duplicate");
                Destroy(gameObject);
                return;
            }
            instance = this;
            Init();
        }

        private void Update()
        {
            if (!isGameInitialisedAndStarted)
            {
                isGameInitialisedAndStarted = true;
                Gameplay.StartGame();
            }
        }

        private void Init()
        {
            Data = new DataManager();
            House = FindObjectOfType<HouseManager>();
            UI = FindObjectOfType<UIManager>();
            Gameplay = new GameplayManager();
            Character = new CharacterManager();
            
            Data.Init();
            House.Init();
            Gameplay.Init();
            UI.Init();
            Character.Init();
        }
    }
}