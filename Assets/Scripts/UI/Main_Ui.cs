using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main_Ui : MonoBehaviour
{
    public AudioClip ButtonClickSound;

    public Sprite pausedButtonImage;
    public Sprite unpauseButtonImage;

    public Button BackButton;
    public Button PauseButton;

    public Text ScoreText;
    public Text ScoreText_Background;

    public Text ScoreTextBonus;
    public Text ScoreTextBonus_Background;

    public Text ScoreTextBonus_2;
    public Text ScoreTextBonus_2_Background;

    public Text BlockCounterText;
    public Text BlockCounterText_Background;

    public GameObject PauseMenu;
    public Text PauseText;
    public Text PauseText_Background;

    public Text WonGameScoreText;
    public Text WonGameScoreText_Background;

    public Text BlockScore_Text;
    public Text BlockScore_Text_Background;

    public Image CrashImage;

    public bool pausedGame = false;

    private List<AudioSource> stoppedSources = new List<AudioSource>();
    private Block_Controller blockController;

    private IEnumerator shakeCrashImage;
    private IEnumerator stackAmtCalc;
    private IEnumerator textColorChanger;
    private IEnumerator bonusPointCalc;
    private IEnumerator PointCalc;

    private IEnumerator SkipCounting;

    private Quaternion startCrashImageRotation;
    private Vector3 startCrashImageScale;

    private static Main_Ui _instance;

    public static Main_Ui Instance { get { return _instance; } }

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    private void Start() {

        startCrashImageRotation = CrashImage.transform.rotation;
        startCrashImageScale = CrashImage.transform.localScale;

        blockController = SingleGamemanager.Instance.gameObject.GetComponent<Block_Controller>();
    }

    public void PauseGame() {


        if (!pausedGame) {

            BlockScore_Text.gameObject.SetActive(false);
            BlockScore_Text_Background.gameObject.SetActive(false);

            PauseMenu.gameObject.SetActive(true);
            PauseText.text = "Paused";
            PauseText_Background.text = "Paused";
            pausedGame = true;

            PauseButton.gameObject.GetComponent<Image>().sprite = unpauseButtonImage;

            StopAllAudio();
            Time.timeScale = 0f;

            SoundManager.Instance.PlayPicthedSoundEffect(ButtonClickSound);

        } else {


            BlockScore_Text.gameObject.SetActive(true);
            BlockScore_Text_Background.gameObject.SetActive(true);

            PauseMenu.gameObject.SetActive(false);
            pausedGame = false;

            PauseButton.gameObject.GetComponent<Image>().sprite = pausedButtonImage;

            Time.timeScale = 1f;
            PlayStoppedAudioSources();

            SoundManager.Instance.PlayPicthedSoundEffect(ButtonClickSound);
        }
    }

    public void OpenMainMenu() {
        Time.timeScale = 1f;
        Loading_Screen.Instance.LoadSceneWithLoadingScreenString("Menu_Scene");
    }

    private bool didShowAd = false;
    public void RestartGameButton() {

        int rand = Random.Range(1, 5);

        if (rand == 1 && !didShowAd) {
            Ad_Handler.Instance.ShowNormalAd();
            didShowAd = true; // so we dont spam ads
        } else {
            Time.timeScale = 1f;
            Loading_Screen.Instance.LoadSceneWithLoadingScreenString("GameScene"); // maybe switch so scene dosent load
        }
    }

    public void FailGame() {
        PauseText.text = "Failed";
        PauseText_Background.text = "Failed";
        PauseMenu.gameObject.SetActive(true);
        pausedGame = true;
        Time.timeScale = 1f;

        WonGameScoreText.text = "" + blockController.scoreCounter.GetScore().ToString();
        WonGameScoreText_Background.text = "" + blockController.scoreCounter.GetScore().ToString();

        BackButton.gameObject.SetActive(false);

        ScoreText.text = blockController.scoreCounter.GetScore().ToString();
        ScoreText_Background.text = blockController.scoreCounter.GetScore().ToString();
    }

    public void AddScoreBonus(int score) {
        bonusPointCalc = AddBonusPoints(0.1f/score, score);
        StartCoroutine(bonusPointCalc);
    }

    public void CurrentBlocks(int amt, int min) {
        BlockCounterText.text = amt.ToString() + "/" + min.ToString();
        BlockCounterText_Background.text = amt.ToString() + "/" + min.ToString();


        float amtCompledted = 0f;
        if (amt > 0 && min > 0) amtCompledted = (float)amt / (float)min;


            Color col = new Color(1f - amtCompledted, amtCompledted, 0f);
            BlockCounterText.GetComponent<Text>().color = col;


        if (amt >= min && (amt - min) < 20){
            CrashImage.gameObject.SetActive(true);

            shakeCrashImage = ShakeCrashImage(amt, min);
            StopCoroutine(shakeCrashImage);
            StartCoroutine(shakeCrashImage);

            float scale = Mathf.Clamp(((float)min / (float)amt), 0.4f, 1f);
            CrashImage.gameObject.transform.localScale = new Vector3(startCrashImageScale.x * scale, startCrashImageScale.y * scale, startCrashImageScale.z * scale);
 
        } else {
            CrashImage.gameObject.SetActive(false);
        }
    }

    private bool hasSkippedCountingScene = false;

    public void SkipCountingScene(float showTime) {
        if (!hasSkippedCountingScene) {
            SkipCounting = SkipCountingIEnum(showTime);
            StartCoroutine(SkipCounting);
        }
    }

    private IEnumerator SkipCountingIEnum(float time) {

        hasSkippedCountingScene = true;

        yield return new WaitForSeconds(0.25f);

        StopCoroutine(bonusPointCalc);

        ScoreTextBonus.text = blockController.scoreCounter.GetScore().ToString();
        ScoreTextBonus_Background.text = blockController.scoreCounter.GetScore().ToString();

        ScoreText.text = blockController.scoreCounter.GetScore().ToString();
        ScoreText_Background.text = blockController.scoreCounter.GetScore().ToString();

        ScoreTextBonus_2.text = "";
        ScoreTextBonus_2_Background.text = "";

        BlockScore_Text.gameObject.SetActive(false);
        BlockScore_Text_Background.gameObject.SetActive(false);

        yield return new WaitForSeconds(time);

        blockController.counting = false;


        ScoreTextBonus.text = "";
        ScoreTextBonus_Background.text = "";

        BlockCounterText.gameObject.SetActive(true);
        BlockCounterText_Background.gameObject.SetActive(true);

        hasSkippedCountingScene = false;
    }

    private IEnumerator ShakeCrashImage(int amtBlocks, int minBlocks) {

        //  while (amtBlocks >= minBlocks && (amtBlocks - minBlocks) < 20) {
        while (amtBlocks >= minBlocks) {

            CrashImage.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 5f * Mathf.Sin(Time.time * 2f));

            yield return new WaitForSeconds(0.025f);
        }
    }

    private IEnumerator AddBonusPoints(float time, int points) {

         WaitForSeconds wait = new WaitForSeconds(time);
         bool skipWait = false;

        BlockCounterText.gameObject.SetActive(false);
        BlockCounterText_Background.gameObject.SetActive(false);

        int ExtraBonusPoints = blockController.GetGoodLandingPoints();

        for (int i = 0; i <= points; i++) {

            if (points / (i + 1) >= 10f) { // twice as faster
                i = i + 10;
            } else if (points / (i + 1) >= 5f) {
                i = i + 5;
            } else if (points / (i + 1) >= 3f) {
                i = i + 3;
            } else if (points / (i + 1) >= 2f) {
                i = i + 2;
            }

            ScoreTextBonus.text = i.ToString();
            ScoreTextBonus_Background.text = i.ToString();

            if (!skipWait) {
                skipWait = true;
                yield return wait;
            } else {
                skipWait = false;
            }
        }

        yield return new WaitForSeconds(1f);


        if (ExtraBonusPoints > 0) {
            for (int i = 0; i <= ExtraBonusPoints; i++) {

                ScoreTextBonus_2.text = "+" + i.ToString();
                ScoreTextBonus_2_Background.text = "+" + i.ToString();

                yield return wait;
            }

        yield return new WaitForSeconds(1.25f);

            for (int i = 0; i <= ExtraBonusPoints; i++) {

                int newPoints = points + i;
                int newMinusPoints = ExtraBonusPoints - i;

                ScoreTextBonus.text = newPoints.ToString();
                ScoreTextBonus_Background.text = newPoints.ToString();

                ScoreTextBonus_2.text = "+" + newMinusPoints.ToString();
                ScoreTextBonus_2_Background.text = "+" + newMinusPoints.ToString();

                if (newMinusPoints <= 0) {
                    ScoreTextBonus_2.text = "";
                    ScoreTextBonus_2_Background.text = "";
                }

                yield return wait;
            }
        }

            yield return new WaitForSeconds(1f);

        PointCalc = AddPointsToScore(0.25f / points, points);
        StartCoroutine(PointCalc);

        ScoreTextBonus.text = "";
        ScoreTextBonus_Background.text = "";

        ScoreTextBonus_2.text = "";
        ScoreTextBonus_2_Background.text = "";

        BlockScore_Text.gameObject.SetActive(false);
        BlockScore_Text_Background.gameObject.SetActive(false);

        blockController.counting = false;

        BlockCounterText.gameObject.SetActive(true);
        BlockCounterText_Background.gameObject.SetActive(true);

    }

    public void AddScoreViaFunction(int points) {
        PointCalc = AddPointsToScore(1f / points, points);
        StartCoroutine(PointCalc);
    }

    public IEnumerator AddPointsToScore(float time, int points) {

        WaitForSeconds wait = new WaitForSeconds(time);

        int TotalScore = blockController.scoreCounter.GetScore() - points;
        int NewScore = blockController.scoreCounter.GetScore();

        for (int i = TotalScore; i <= NewScore; i++) {
            ScoreText.text = i.ToString();
            ScoreText_Background.text = i.ToString();

            yield return wait;
        }
        yield return new WaitForSeconds(1f);
        // maybe change color
    }

    public void AddStackBonus(float time, int amt) {
        stackAmtCalc = AddStackAmt(time, amt);
        StartCoroutine(stackAmtCalc);
    }

    public IEnumerator AddStackAmt(float time, int amt) {

        BlockScore_Text.gameObject.SetActive(true);
        BlockScore_Text_Background.gameObject.SetActive(true);

        int StackHighScore = Data_Script.Instance.GetPlayerStackScore();

        WaitForSeconds wait = new WaitForSeconds(time/amt);

        for (int i = 0; i <= amt; i++) {
            BlockScore_Text.text = "STACKED\n" + i.ToString();
            BlockScore_Text_Background.text = "STACKED\n" + i.ToString();

            yield return wait;
        }

        if (amt > StackHighScore) {
            textColorChanger = TextColorChanger(BlockScore_Text);
            StartCoroutine(textColorChanger);
        }

        yield return new WaitForSeconds(1f);
    }


    public IEnumerator TextColorChanger(Text txt) {

        Color startColor = txt.color;
        Color newColor = new Color(1f, 0f, 0f);

        float time = Time.time + 10f;
        float duration = 0.1f;

        WaitForSeconds wait = new WaitForSeconds(0.01f);

        while (time >= Time.time) {
                float t = Mathf.PingPong(Time.time, duration) / duration;
                txt.color = Color.Lerp(startColor, newColor, t);

                yield return wait;
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
