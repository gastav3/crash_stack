using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBlock : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
          Debug.Log("1: " + this.transform.eulerAngles.x + " 2: "+ this.transform.localEulerAngles.x + " 3: " + this.transform.localRotation.eulerAngles.x + " 4: " + this.transform.rotation.x);


       // float x = CorrectedAngle(this.transform.rotation.x, this.transform.eulerAngles.x);
       // Debug.Log(x);


    }


    private float CorrectedAngle(float normalRot, float eularRot) {

        if (normalRot < 0f) {
            float calcAngle = Mathf.Abs(eularRot - 360f);

            return Mathf.Round(calcAngle * 1000f) / 1000f;
        } else {
            return Mathf.Round(eularRot * 1000f) / 1000f;
        }
    }

}
