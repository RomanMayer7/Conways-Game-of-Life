using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GameOfLife.Main;
using System;

namespace GameOfLife.UI
{
    public class UIManager : MonoBehaviour
    {

        [SerializeField]
        private GameOfLifeGrid gameScreen;//Main Game Screen

        //----------BUTTONS ON UI SIDEBAR----------------------
        [SerializeField]
        private Button togglePlayPauseButton;

        [SerializeField]
        private Button nextStepButton;

        private Button restartButton;

        [SerializeField]
        private Button exitButton;

        [SerializeField]
        private Dropdown gridSizeDropdown;
        
        [SerializeField]
        private Slider intervalSlider;


        // Start is called before the first frame update
        void Start()
        {

            if (gameScreen == null)
            {
                gameScreen = GetComponent<GameOfLifeGrid>();
            }

            if (exitButton == null)
            {
                exitButton = GameObject.Find("ButtonExit").GetComponent<Button>();
            }
            if (restartButton == null)
            {
                restartButton = GameObject.Find("ButtonRestart").GetComponent<Button>();
            }
            if (nextStepButton == null)
            {
                nextStepButton = GameObject.Find("ButtonNextStep").GetComponent<Button>();
            }
            if (togglePlayPauseButton == null)
            {
                togglePlayPauseButton = GameObject.Find("ButtonTogglePause").GetComponent<Button>();
            }

            if (gridSizeDropdown==null)
            {
                gridSizeDropdown = GetComponent<Dropdown>();
            }

            if (intervalSlider == null)
            {
                intervalSlider = GetComponent<Slider>();
            }

            //Add listeners for the Buttons andwhen the value of the Dropdown changes, to take action
            exitButton.onClick.AddListener(Exit);
            togglePlayPauseButton.onClick.AddListener(gameScreen.TogglePause);
            nextStepButton.onClick.AddListener(gameScreen.Tick);
            restartButton.onClick.AddListener(gameScreen.RestartGame);
            
            gridSizeDropdown.onValueChanged.AddListener(delegate {
                DropdownValueChanged(gridSizeDropdown);
            });

            intervalSlider.onValueChanged.AddListener(delegate { IntervalValueChangeCheck(); });

        }

        // Update is called once per frame
        void Update()
        {

        }

        void DropdownValueChanged(Dropdown change)
        {

            switch (change.value)
            {
                case 0:
                    gameScreen.ChangeGridSize(15);
                    break;
                case 1:
                    gameScreen.ChangeGridSize(10);
                    break;
                case 2:
                    gameScreen.ChangeGridSize(7);
                    break;
            }

        }

        private void IntervalValueChangeCheck()
        {
            gameScreen.ChangeInterpolationPeriod(intervalSlider.value);
        }


        public void Exit()
        {
            Debug.Log("Quiting");
            #if UNITY_EDITOR
              UnityEditor.EditorApplication.isPlaying = false;
            #else
              Application.Quit();
            #endif

        }
    }
}
