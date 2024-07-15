using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace SlimUI.ModernMenu{
	public class EndMenuManager : MonoBehaviour {
		private Animator CameraObject;

		// campaign button sub menu
        [Header("MENUS")]
        [Tooltip("The Menu for when the MAIN menu buttons")]
        public GameObject mainMenu;
        [Tooltip("THe first list of buttons")]
        public GameObject firstMenu;

        public enum Theme {custom1, custom2, custom3};
        [Header("THEME SETTINGS")]
        public Theme theme;
        private int themeIndex;
        public ThemedUIData themeController;

        [Header("PANELS")]
        [Tooltip("The UI Panel parenting all sub menus")]
        public GameObject mainCanvas;

        [Header("LOADING SCREEN")]
		[Tooltip("If this is true, the loaded scene won't load until receiving user input")]
		public bool waitForInput = true;
        public GameObject loadingMenu;
		[Tooltip("The loading bar Slider UI element in the Loading Screen")]
        public Slider loadingBar;
        public TMP_Text loadPromptText;
		public KeyCode userPromptKey;

		[Header("SFX")]
        [Tooltip("The GameObject holding the Audio Source component for the HOVER SOUND")]
        public AudioSource hoverSound;
        [Tooltip("The GameObject holding the Audio Source component for the AUDIO SLIDER")]
        public AudioSource sliderSound;
        [Tooltip("The GameObject holding the Audio Source component for the SWOOSH SOUND when switching to the Settings Screen")]
        public AudioSource swooshSound;

		public AudioClip win_sound;
		private AudioSource win;

        void Start(){
			CameraObject = transform.GetComponent<Animator>();

			firstMenu.SetActive(true);
			mainMenu.SetActive(true);

			SetThemeColors();

            win = gameObject.AddComponent<AudioSource>();
            win.clip = win_sound;
            // quiet
            win.volume = 0.4f;
            win.loop = true;
            win.Play();

        }

        void SetThemeColors()
		{
			switch (theme)
			{
				case Theme.custom1:
					themeController.currentColor = themeController.custom1.graphic1;
					themeController.textColor = themeController.custom1.text1;
					themeIndex = 0;
					break;
				case Theme.custom2:
					themeController.currentColor = themeController.custom2.graphic2;
					themeController.textColor = themeController.custom2.text2;
					themeIndex = 1;
					break;
				case Theme.custom3:
					themeController.currentColor = themeController.custom3.graphic3;
					themeController.textColor = themeController.custom3.text3;
					themeIndex = 2;
					break;
				default:
					Debug.Log("Invalid theme selected.");
					break;
			}
		}

        public void GoToMainMenu()
        {
            // Unlock cursor
            Cursor.lockState = CursorLockMode.None;
            // switch scene to win scene
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameMenu");
        }

        public void LoadScene(string scene){
			if(scene != ""){
				StartCoroutine(LoadAsynchronously(scene));
			}
		}

		public void Position1(){
			CameraObject.SetFloat("Animate",0);
		}

		public void PlayHover(){
			hoverSound.Play();
		}

		public void PlaySFXHover(){
			sliderSound.Play();
		}

		public void PlaySwoosh(){
			swooshSound.Play();
		}

		public void QuitGame(){
			#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
			#else
				Application.Quit();
			#endif
		}

		// Load Bar synching animation
		IEnumerator LoadAsynchronously(string sceneName){ // scene name is just the name of the current scene being loaded
			AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
			operation.allowSceneActivation = false;
			mainCanvas.SetActive(false);
			loadingMenu.SetActive(true);

			while (!operation.isDone){
				float progress = Mathf.Clamp01(operation.progress / .95f);
				loadingBar.value = progress;

				if (operation.progress >= 0.9f && waitForInput){
					loadPromptText.text = "Press " + userPromptKey.ToString().ToUpper() + " to continue";
					loadingBar.value = 1;

					if (Input.GetKeyDown(userPromptKey)){
						operation.allowSceneActivation = true;
					}
                }else if(operation.progress >= 0.9f && !waitForInput){
					operation.allowSceneActivation = true;
				}

				yield return null;
			}
		}
	}
}