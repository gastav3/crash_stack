using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    private ParticleSystem particleSys;

    public AudioClip breakBlockClip;

    public AudioClip goodLanding_Clip;
    public AudioClip normalLanding_Clip;

    private IEnumerator createCubeCour;
    private IEnumerator coroutine;

    public Block_Controller block_Controller;

    public int amtAttached = 0;
    private GameObject hasAttchedGameObj;
    public bool hasAttached = false;
    public ObjectPool pool;



    private float CalculateCubeSize(float size) {

        float SizePerCube = size / block_Controller.standardPieces;

        return SizePerCube;
    }

    public void CreateCubes(bool inst) {

        if (!inst) {
            if (this.transform.gameObject.activeSelf) {
                createCubeCour = CreateCubeWithTimer(0.025f); // block create time
                StartCoroutine(createCubeCour);
            }
        } else {
            this.transform.gameObject.SetActive(false);
        }
    }


    private IEnumerator CreateCubeWithTimer(float time) {

        float cubeSize = CalculateCubeSize(this.gameObject.transform.localScale.x);
        float cubeHeight = this.gameObject.transform.localScale.y;

        BoxColiderControl(false);

        WaitForSeconds wait = new WaitForSeconds(time);

        for (int x = 0; x < block_Controller.standardPieces; x++) {
            for (int y = 0; y < block_Controller.standardPieces; y++) {
                CreateSingleCube(x, y, cubeSize, cubeHeight);
            }
            yield return wait;
        }

        this.transform.gameObject.SetActive(false);
        Explode();
    }


    public void CreateSingleCube(int x, int y, float size, float height) {

        if (this.gameObject && this.transform.gameObject.activeSelf) {
            // GameObject piece = GameObject.CreatePrimitive(PrimitiveType.Cube);

            GameObject piece = pool.GetNextPooledItem();

            //set piece position and scale

            piece.gameObject.GetComponent<Renderer>().material.color = this.gameObject.GetComponent<Renderer>().material.color;

            piece.transform.position = transform.position + new Vector3(size * x, size, size * y) - new Vector3(transform.localScale.x / 2f, 0f, transform.localScale.z / 2f);
            piece.transform.rotation = this.transform.rotation;
            piece.transform.localScale = new Vector3(size, height, size);

            //add rigidbody and set mass
            //  piece.AddComponent<Rigidbody>();
            // piece.GetComponent<Rigidbody>().mass = size;
        }
    }

    private void Explode() {

        float explosionForce = 2.5f;
        float explosionRadius = 5f;
        float explosionUpward = 0f;

            Vector3 explosionPos = transform.position;

            Collider[] colliders = Physics.OverlapSphere(explosionPos, 25f);
            foreach (Collider hit in colliders) {
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                if (rb != null && hit.gameObject.tag == "Cube") {
                    rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, explosionUpward);
                }
            }

        SoundManager.Instance.PlayPicthedInGameSoundEffect(breakBlockClip);
        }


    private void CheckAngels() {

        /*  float xRotation = CorrectedAngle(this.transform.rotation.x, this.transform.eulerAngles.x);
          float zRotation = CorrectedAngle(this.transform.rotation.z, this.transform.eulerAngles.z);

          Debug.Log(xRotation + " " + zRotation);

          if (xRotation >= 10f || zRotation >= 10f) {
             // block_Controller.FailAngleBlocks();
          }*/

        if (hasAttached && !block_Controller.counting && block_Controller.GetLastActiveBlock() == this.gameObject) {
            if (Mathf.Abs(this.transform.rotation.x) >= 0.0871f || Mathf.Abs(this.transform.rotation.z) >= 0.0871) { //  0.0871 = about 10 degrees
                block_Controller.FailAngleBlocks();
            }
        }
    }

    private void FixedUpdate() {
        CheckAngels();

    }

    private void Start() {

        particleSys = this.gameObject.GetComponent<ParticleSystem>();
        particleSys.Stop();

        coroutine = DeleteBlockAfterTime(2f);
        StartCoroutine(coroutine);

      //  this.gameObject.GetComponent<Renderer>().material.color = Color.red;
    }

    private IEnumerator DeleteBlockAfterTime(float time) {
        while (true) {
            yield return new WaitForSeconds(time);

            if (!hasAttached) {
                CreateCubes(false);
                block_Controller.ClearCubes(0.005f, 0.5f);
            }
        }
    }
    private void OnTriggerEnter(Collider other) {

        if (!hasAttached) {
            if (other.gameObject.tag == "Block") {

                Vector3 point = GetTouchPoint(other.gameObject);
                Attach(other.gameObject.transform, point);

            } else if (other.gameObject.tag == "Floor" && block_Controller.CountAttchedBlocks() <= 0) { // check so only 1 can have floor accses
                hasAttached = true;
                block_Controller.AttachedCompleted();
                SoundManager.Instance.Play(normalLanding_Clip);
            }
        }
    }


    private Vector3 GetTouchPoint(GameObject obj) {

            Vector3 hitPoint = new Vector3(-1f, -1f, -1f);
            RaycastHit hit;

        //Debug.DrawRay(this.transform.position, this.transform.up, Color.green, 2, false);

        if (Physics.Raycast(this.transform.position, -this.transform.up, out hit)) {
                Debug.Log("Point of contact: " + hit.point);
                hitPoint = hit.point;
        }
        return hitPoint;
    }

    public void BoxColiderControl(bool bol) {
        BoxCollider BlockCollider;

        Component[] boxColiders = this.gameObject.GetComponents(typeof(BoxCollider));

        foreach (BoxCollider box in boxColiders) {
            if (!box.isTrigger) {
                BlockCollider = box;
                BlockCollider.enabled = bol;
            }
        }
    }

    private float CorrectedAngle(float normalRot, float eularRot) { // gives strange numbers with rigidbody

        if (normalRot < 0f) {
            float calcAngle = Mathf.Abs(eularRot - 360f);

            return Mathf.Round(calcAngle * 1000f) / 1000f;
        } else {
            return Mathf.Round(eularRot * 1000f) / 1000f;
        }
    }

    private bool GoodLanding(GameObject obj1, GameObject obj2) {

        if (obj1 && obj2) {

            Vector3 obj1Vec = new Vector3(obj1.transform.position.x, 0f, 0f);
            Vector3 obj2Vec = new Vector3(obj2.transform.position.x, 0f, 0f);

            float distance = Vector3.Distance(obj1Vec, obj2Vec);

            if (distance <= 0.225f) {
                return true;
            }
        }
        return false;
    }


    private void Attach(Transform obj, Vector3 touchPoint) {


        if (obj.gameObject && !hasAttached && obj.GetComponent<Block>().hasAttached == true && obj.GetComponent<Block>().amtAttached <= 0) {
            obj.GetComponent<Block>().amtAttached += 1;

            Color newColor = new Color(Random.Range(0f,1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            this.gameObject.GetComponent<Renderer>().material.color = newColor;

            BoxColiderControl(false);

            this.transform.rotation = obj.rotation;

            // do check if position is to fair or near
            this.transform.position = new Vector3(this.transform.position.x, touchPoint.y + this.transform.localScale.y/2f, this.transform.position.z);

    
               /*   float blockSizeZ = obj.transform.localScale.z;
                  float rad = CorrectedAngle(obj.transform.rotation.z, obj.transform.eulerAngles.z) * Mathf.Deg2Rad;
                  float zOffset = Mathf.Abs((Mathf.Abs(obj.transform.position.x) - Mathf.Abs(this.transform.position.x))); // use x
                  float blockTopY = Mathf.Sin(rad) * (((blockSizeZ - zOffset) * (this.transform.localScale.z / blockSizeZ)));

                  float sin = Mathf.Sin(rad);

                  Debug.Log("Top: " + blockTopY + " Angle: " + CorrectedAngle(obj.transform.rotation.z, obj.transform.eulerAngles.z) + "  Block Length: " + blockSizeZ + " SIN: " + sin + " Rad: " + rad + " zOffset: " + zOffset + " BlockSize: " + (blockSizeZ - zOffset));

                  this.transform.rotation = obj.rotation;
                  this.transform.position = new Vector3(this.transform.position.x, (blockTopY + (this.transform.localScale.y/2) + (obj.transform.position.y)), this.transform.position.z);*/


            this.gameObject.AddComponent<FixedJoint>();
            this.GetComponent<FixedJoint>().connectedBody = obj.gameObject.GetComponent<Rigidbody>();
            this.GetComponent<FixedJoint>().enablePreprocessing = false;

            this.GetComponent<Block>().hasAttached = true;

            if (!GoodLanding(this.gameObject, obj.gameObject)) {
                block_Controller.IncreasePenaltyPoint(1);
                SoundManager.Instance.Play(normalLanding_Clip);
            } else {
                particleSys.Play();
                SoundManager.Instance.Play(goodLanding_Clip);
                this.transform.gameObject.GetComponent<Rigidbody>().mass = this.transform.gameObject.GetComponent<Rigidbody>().mass / 10f;
                // block_Controller.AddNiceScore(10);
                block_Controller.IncreaseGoodLanding();
            }

            BoxColiderControl(true);
            block_Controller.AttachedCompleted();
            Debug.Log("Attched");
        }
    }
  }
