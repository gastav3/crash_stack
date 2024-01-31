using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour {
    private Block_Controller blockcontroller;
    private Main_Ui main_Ui;

    private float destroyBlocksTimer = 0f;
    private float spawnBlockTimer = 0f;

    void Start() {
        blockcontroller = SingleGamemanager.Instance.GetComponent<Block_Controller>();
        main_Ui = Main_Ui.Instance;
    }

    void Update() {

        if (Application.platform == RuntimePlatform.Android) {
            TouchInput();
        }

        if (Input.GetKeyDown(KeyCode.Space)) {

            DoDestroyBlocks();
        }

        if (Input.GetKeyDown(KeyCode.LeftControl)) {

                DoSpawnBlock();
        }
    }

    private void DoSpawnBlock() {
        if (spawnBlockTimer <= Time.time) {

            if (blockcontroller.counting) {

                main_Ui.SkipCountingScene(2.5f);

            } else {

                blockcontroller.SpawnBlock();

                // float spawnTime = blockcontroller.GetPenaltyBlocks();

                spawnBlockTimer = Time.time + 0.1f;
            }
        }
    }

    private void DoDestroyBlocks() {
        if (destroyBlocksTimer <= Time.time) {

            blockcontroller.DestroyBlocks();

            destroyBlocksTimer = Time.time + 1f;
        }
    }



    Rect MiddleScreen = new Rect(0, Screen.height - (Screen.height / 3.9f), Screen.width - Screen.width / 7f, (Screen.height / 4f) - Screen.height);

    //Bottom
    Rect LeftHalfScreenTest_2 = new Rect(0, Screen.height - (Screen.height / 4f), Screen.width / 1.4f, (Screen.height / 4f));
    Rect RightHalfScreenTest_2 = new Rect(Screen.width - Screen.width / 3.7f, Screen.height - (Screen.height / 4f), Screen.width / 3.7f, (Screen.height / 4f));


    private Vector2 startTouchPos;
    private float timeResetMoved = 0f;

    private void TouchInput() {

        bool inSideTouch = false;

        Rect MiddleScreen = new Rect(0, Screen.height - (Screen.height / 3.9f), Screen.width - Screen.width / 6.5f, (Screen.height / 2.5f) - Screen.height);

        //Bottom
        Rect LeftHalfScreenTest_2 = new Rect(0, Screen.height - (Screen.height / 4f), Screen.width / 1.17f, (Screen.height / 4f));
        Rect RightHalfScreenTest_2 = new Rect(Screen.width - Screen.width / 7f, Screen.height - (Screen.height / 1.15f), Screen.width / 3.7f, (Screen.height / 1.1f));


        // Fix diffrent pos for touch and rect
        Touch touch = Input.GetTouch(0); // first touch
        Vector2 touchPos = touch.position;

        // Fix diffrent pos for touch and rect
        Vector3 TouchPositionFix = new Vector3(touch.position.x, touch.position.y, 0f);
        TouchPositionFix.y = Screen.height - TouchPositionFix.y;


        if (MiddleScreen.Contains(TouchPositionFix, true)) {
            inSideTouch = true;
        } else if (LeftHalfScreenTest_2.Contains(TouchPositionFix, true)) {
            inSideTouch = true;
        } else if (RightHalfScreenTest_2.Contains(TouchPositionFix, true)) {
            inSideTouch = true;
        }


        if (inSideTouch && !main_Ui.pausedGame) {

            float dragAmount = 0f;

            if (Screen.width > Screen.height) {
                dragAmount = Screen.width / 4f;
            } else {
                dragAmount = Screen.height / 4f;
            }

            if (touch.phase == TouchPhase.Began) {

                    startTouchPos = touchPos;
                    DoSpawnBlock();
                    timeResetMoved = Time.unscaledTime + 1f;
            }

            if (touch.phase == TouchPhase.Ended) {
                float distance = Vector2.Distance(touchPos, startTouchPos);

                if (distance > dragAmount && timeResetMoved >= Time.unscaledTime) {
                    DoDestroyBlocks();
                    timeResetMoved = 0;
                }
            }
        }


         }


    }
/*
    Vector3 TouchPositionFix = new Vector3(touch.position.x, touch.position.y, 0f);
    TouchPositionFix.y = Screen.height - TouchPositionFix.y;

    Vector2 touchPos = touch.position;
    float distance = Vector2.Distance(touchPos, startTouchPos);

    if (timeResetMoved < Time.time && moved) {
        startTouchPos = Vector2.zero;
        moved = false;
    }

    if (touch.phase == TouchPhase.Moved) {
        moved = true;
        timeResetMoved = Time.time + 0.3f;
        startTouchPos = touchPos;

        if (distance >= 10f) {
            DoDestroyBlocks();
            startTouchPos = Vector2.zero;
            moved = false;
        }
    }


    if (touch.phase == TouchPhase.Began) {

        if (!moved) {

             touchPos = Vector2.zero;

            if (MiddleScreen.Contains(TouchPositionFix, true)) {
                DoSpawnBlock();
            } else if (LeftHalfScreenTest_2.Contains(TouchPositionFix, true)) { // since they do the same thing
                DoSpawnBlock();
            }else if (RightHalfScreenTest_2.Contains(TouchPositionFix, true)) {
                DoSpawnBlock();
        }
    }
}
}
}
*/




// too look where touch is
/*
private void OnGUI() {


    GUIStyle style = new GUIStyle();
    //Rect bounds = new Rect(0, 0, Screen.width/2, Screen.height/0.75f)

    //Rect SuspensionButtonTest = new Rect(Screen.width/4f, Screen.height- (Screen.height/4f), Screen.width/2f, (Screen.height/4f));
    //	Rect LeftHalfScreenTest = new Rect(0, Screen.height - (Screen.height/4f), Screen.width/2f, (Screen.height/4f) - Screen.height);

    Rect MiddleScreen = new Rect(0, Screen.height - (Screen.height / 3.9f), Screen.width - Screen.width / 6.5f, (Screen.height / 2.5f) - Screen.height);

    //Bottom
    Rect LeftHalfScreenTest_2 = new Rect(0, Screen.height - (Screen.height / 4f), Screen.width / 1.17f, (Screen.height / 4f));
    Rect RightHalfScreenTest_2 = new Rect(Screen.width - Screen.width / 7f, Screen.height - (Screen.height / 1.15f), Screen.width / 3.7f, (Screen.height / 1.1f));

    style.alignment = TextAnchor.UpperLeft;
    style.fontSize = Screen.height * 2 / 100;
    style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);

    //GUI.Box(SuspensionButtonTest,"1");
    //GUI.Box(LeftHalfScreenTest,"2");

    GUI.Box(MiddleScreen, "3");
    GUI.Box(RightHalfScreenTest_2, "4");

    GUI.Box(LeftHalfScreenTest_2, "5");

}*/


