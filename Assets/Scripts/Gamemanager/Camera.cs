using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public GameObject cam;

    private Block_Controller blockController;

    private float cameraStartZ = 0f;
    private float cameraStartY = 0f;
    private float cameraStartX = 0f;

    private float x = 0f;
    private float y = 0f;
    private float z = 0f;


    void Start()
    {


        x = cam.transform.position.x;

        blockController = this.gameObject.GetComponent<Block_Controller>();
        cameraStartZ = cam.transform.position.z;
        cameraStartY = cam.transform.position.y;
        cameraStartX = cam.transform.position.x;
    }


    private float GetCameraXPos() {

        if (Mathf.Abs(x) >= Mathf.Abs(cameraStartX) + 1f) {
            cameraStartX = cam.transform.position.x;
            return x;
        }

        return this.cam.transform.position.x;
    }


    void LateUpdate()
    {

        if (blockController.GetBlockList().Count > 0) {
            try {
                y = blockController.GetLastActiveBlock().transform.position.y;
                z = blockController.GetLastActiveBlock().transform.localScale.x;
                x = blockController.GetLastActiveBlock().transform.position.x;
            } catch (Exception e) {
                //   Debug.Log("Deleted Block - Camera");
            }
            }else {
            x = this.cam.transform.position.x;
            z = 0;
        }


        if (!blockController.GetLastActiveBlock()) {
            y = cameraStartY;
        }

            cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(x, y + 2f, (cameraStartZ + 0.25f) - (z * 2f)), 0.5f * Time.deltaTime);
    }
}
