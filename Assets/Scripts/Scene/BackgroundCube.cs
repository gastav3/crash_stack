using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundCube : MonoBehaviour
{

   private void Start()
    {
        Color newColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        this.gameObject.GetComponent<Renderer>().material.color = newColor;

        float startScaleY = this.transform.localScale.y;
        float rnd = Random.Range(0.25f, 2f);

        this.transform.localScale = new Vector3(this.transform.localScale.x * rnd, this.transform.localScale.y * rnd, this.transform.localScale.z * rnd);
        this.transform.position = new Vector3(this.transform.position.x, ((this.transform.localScale.y/2f)), this.transform.position.z);

        this.transform.rotation = Quaternion.Euler(0f, Random.Range(-30f, 30f), 0f);
    }
}
