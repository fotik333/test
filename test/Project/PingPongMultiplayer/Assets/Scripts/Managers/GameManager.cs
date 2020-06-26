using System;
using Interfaces;
using Listeners;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public static Vector2 BottomLeft, TopRight;

        public IGamePlayManager CurrentGamePlayManager { get; private set; }
        public SomeButtonClickedManager SomeButtonClickedManager { get; private set; }
        public SaveLoadManger SaveLoadManger { get; private set; }
        public OptionsManager OptionsManager { get; private set; }

        public int Record { get; set; }

        private bool _isOnlineSceneLoading;

        private bool IsOnlineMode => _isOnlineSceneLoading ||
                                     CurrentGamePlayManager != null && CurrentGamePlayManager.GetType() ==
                                     typeof(GamePlayNetworkManager);

        private void Awake()
        {
            if (Instance == null)
                Init();
            else
                Destroy(gameObject);
        }

        private void Init()
        {
            DontDestroyOnLoad(gameObject);

            Instance = this;

            OptionsManager = new OptionsManager();
            SaveLoadManger = new SaveLoadManger();
            
            InitButtonClickedManager();

            SceneManager.sceneLoaded += (scene, mode) =>
            {
                if (scene.name != "Game") return;

                if (IsOnlineMode)
                    Resources.FindObjectsOfTypeAll<GamePlayNetworkManager>()[0].gameObject.SetActive(true);
                else
                    Resources.FindObjectsOfTypeAll<GamePlayManager>()[0].gameObject.SetActive(true);

                _isOnlineSceneLoading = false;
            };
        }

        private void InitButtonClickedManager()
        {
            SomeButtonClickedManager = new SomeButtonClickedManager();

            SomeButtonClickedManager.PlayButtonClicked += OnPlayButtonClicked;
            SomeButtonClickedManager.OnlineButtonClicked += OnOnlineButtonClicked;
            SomeButtonClickedManager.QuitButtonClicked += OnQuitButtonClicked;
            SomeButtonClickedManager.MenuButtonClicked += OnMenuButtonClicked;
            SomeButtonClickedManager.StartButtonClicked += StartGame;
        }

        #region ButtonsEvents

        private void StartGame(object sender, EventArgs args)
        {
            CurrentGamePlayManager.StartGame();
        }

        private void OnPlayButtonClicked(object sender, EventArgs args)
        {
            _isOnlineSceneLoading = false;
            LoadScene("Game");
        }

        private void OnOnlineButtonClicked(object sender, EventArgs args)
        {
            _isOnlineSceneLoading = true;
            LoadScene("Game");
        }

        private static void OnMenuButtonClicked(object sender, EventArgs args)
        {
            LoadScene("Menu");
        }

        private static void OnQuitButtonClicked(object sender, EventArgs args)
        {
            Application.Quit();
        }

        #endregion

        private static void LoadScene(string name)
        {
            SceneManager.LoadScene(name, LoadSceneMode.Single);
        }

        public void OnGamePlayManagerCreated(IGamePlayManager manager)
        {
            InitScreenBounds();

            CurrentGamePlayManager = manager;
            CurrentGamePlayManager.Destroyed += OnGamePlayManagerDestroyed;

            if (IsOnlineMode)
                ((GamePlayNetworkManager) CurrentGamePlayManager).Disconnected += OnDisconnected;
            else
                ((GamePlayManager) CurrentGamePlayManager).RecordUpdated += OnRecordUpdated;
        }

        private static void InitScreenBounds()
        {
            var camera = Camera.main;
            var rect = camera.rect;
            
            BottomLeft = camera.ScreenToWorldPoint(new Vector2(0 + rect.x * Screen.width, 0));
            TopRight = camera.ScreenToWorldPoint(new Vector2((rect.x + rect.width) * Screen.width, Screen.height));
        }

        private void OnRecordUpdated(object sender, int newRecord)
        {
            Record = newRecord;
            SaveLoadManger.SaveRecord();
        }
        
        private static void OnDisconnected(object sender, EventArgs args)
        {
            if (SceneManager.GetActiveScene().name == "Game") LoadScene("Menu");
        }

        private void OnGamePlayManagerDestroyed(object sender, EventArgs args)
        {
            if (SceneManager.GetActiveScene().name == "Game") LoadScene("Menu");

            CurrentGamePlayManager.Destroyed -= OnGamePlayManagerDestroyed;

            if (IsOnlineMode)
                ((GamePlayNetworkManager) CurrentGamePlayManager).Disconnected -= OnDisconnected;
            else
                ((GamePlayManager) CurrentGamePlayManager).RecordUpdated -= OnRecordUpdated;

            CurrentGamePlayManager = null;
        }
    }
}