using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main_Menu_Ui : MonoBehaviour{

    public GameObject blockObj;
    public Transform blockSpawn;

    public AudioClip buttonClickSoundEffect;

    public GameObject mainMenu_Panel;
    public GameObject highScoreMenu_Panel;

    public Text highScoreText;
    public Text highScoreText_Background;

    public Text blockStackText;
    public Text blockStackText_Background;

    private List<AudioSource> stoppedSources = new List<AudioSource>();

    private GameObject newBlock;

    public void StartGameButton() {

        Time.timeScale = 1f;
        StopAllAudio();

        Loading_Screen.Instance.LoadSceneWithLoadingScreenString("GameScene");
    }

    public void HighScoreButton() {
        mainMenu_Panel.SetActive(false);
        highScoreMenu_Panel.SetActive(true);

        int score = Data_Script.Instance.GetPlayerHighScore();
        int stackScore = Data_Script.Instance.GetPlayerStackScore();

        highScoreText.text = score.ToString();
        highScoreText_Background.text = score.ToString();

        blockStackText.text = "STACKED\n" + stackScore.ToString();
        blockStackText_Background.text = "STACKED\n" + stackScore.ToString();

        SoundManager.Instance.PlayPicthedSoundEffect(buttonClickSoundEffect);
    }

    public void MainMenuButton() {
        mainMenu_Panel.SetActive(true);
        highScoreMenu_Panel.SetActive(false);

        SoundManager.Instance.PlayPicthedSoundEffect(buttonClickSoundEffect);
    }


    private void Update() {
        SpawnBackgroundBlocks();
    }

    private void Start() {
        newBlock = Instantiate(blockObj, new Vector3(blockSpawn.position.x, blockSpawn.position.y, blockSpawn.position.z), Quaternion.identity);
    }

    private float TimeBackgroundBlock = 0f;
    private void SpawnBackgroundBlocks() {
        if (TimeBackgroundBlock < Time.time) {
            newBlock.GetComponent<Rigidbody>().velocity = Vector3.zero;
            newBlock.transform.position = new Vector3(blockSpawn.transform.position.x + Random.Range(-3, 3), blockSpawn.transform.position.y, blockSpawn.transform.position.z);


            TimeBackgroundBlock = Time.time + 2.5f;
        }
    }

    private void StopAllAudio() {
        stoppedSources.Clear();
        var allAudioSources = Object.FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
        foreach (AudioSource audioS in allAudioSources) {
            if (audioS.isPlaying) {
                stoppedSources.Add(audioS);
                audioS.Stop();
            }
        }
    }

    private void PlayStoppedAudioSources() {
        foreach (AudioSource audioS in stoppedSources) {
            if (audioS != null) {
                audioS.Play();
            }
        }
    }

}
