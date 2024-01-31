using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block_Controller : MonoBehaviour
{
    /// <summary>
    /// Blocks get bigger and bigger until player destroyes them or the block tower tips
    /// 
    /// A minimum that increases, start at 10 and increase 2 or 3
    /// Maybe motion control???
    /// When tower tips lose all points
    /// </summary>
    /// 
    public AudioClip musicClip;

    private IEnumerator velcoityFixCour;
    private IEnumerator removeBlockCour;
    public float penaltySize = 0.15f;

    public GameObject FakeBlock_Prefab;
    private GameObject FakeBlock;

    public float startY = 1f;

    public int cubePieces = 500;
    private ObjectPool pool;

    public GameObject cubePrefab;
    public GameObject blockPrefab;

    public ScoreCounter scoreCounter;
    private Main_Ui inGameUI;


    public int standardPieces = 6; // standardPieces * standardPieces = real amount

    private int goodLandings = 0;
    private int turns = 0;
    private int startMinBlocks = 10;
    private int increaseBlocks = 15;
    private int minBlocks = 0;

    private bool clearing = false;
    public bool counting = false;
    private bool failed = false;

    [SerializeField]
    private List<Block> blocks;

    private void Awake() {
        blocks = new List<Block>();

        pool = new ObjectPool();
        pool.prefabObj = cubePrefab;
        FillPool();

        scoreCounter = new ScoreCounter();
    }

    public ObjectPool GetPool() {
        return pool;
    }

    private void FillPool() {

       GameObject parent = Instantiate(new GameObject(), transform.position, Quaternion.identity);
       parent.name = "Cubes_Parent";

        for (int i = 0; i < cubePieces; i++) { // amount
            if (pool.prefabObj) {
                GameObject obj = Instantiate(pool.prefabObj, new Vector3(0, 0, 0), Quaternion.identity);
                obj.transform.SetParent(parent.transform);
                pool.AddObjectToPool(obj);
            }
        }
    }

    public void RestartGame() { // maybe in use later
        Time.timeScale = 0f;

        scoreCounter.SetScore(0);

        ClearCubes(0.005f, 1f);
        blocks.Clear();
        failed = false;
        clearing = false;

        Time.timeScale = 1f;
    }

    private GameObject lastDropped;
    private float WaitSpawnTimer = 0f;
    public void SpawnBlock() {
        if (!lastDropped || !lastDropped.activeSelf || lastDropped.GetComponent<Block>().hasAttached) {
            if (blockPrefab && !failed && !counting && !clearing) {

                //     if (WaitSpawnTimer < Time.time) {

                AntiTip();

                float YPos = (startY) + (blockPrefab.transform.localScale.y * (GetBlockList().Count));

                GameObject newBlock = Instantiate(blockPrefab, new Vector3(FakeBlock.transform.position.x, YPos, 0), Quaternion.identity);
                newBlock.transform.localScale = new Vector3(blockPrefab.transform.localScale.x + (GetPenaltyBlocks() * penaltySize), blockPrefab.transform.localScale.y, blockPrefab.transform.localScale.z + (GetPenaltyBlocks() * penaltySize));

                GameObject lastBlock = GetLastActiveBlock();
                if (lastBlock) {
                    newBlock.transform.rotation = lastBlock.transform.rotation;
                }

                newBlock.GetComponent<Block>().pool = pool;
                newBlock.GetComponent<Block>().block_Controller = this.gameObject.GetComponent<Block_Controller>();
                blocks.Add(newBlock.GetComponent<Block>());

                newBlock.GetComponent<Rigidbody>().mass = newBlock.transform.localScale.x / (blockPrefab.transform.localScale.x * 4f); // to much mass causes bonciness

                lastDropped = newBlock;
            }
        }
    }

    private void StartBlock() {
        GameObject newBlock = Instantiate(blockPrefab, new Vector3(0f, FakeBlock.transform.localPosition.y/2, 0), Quaternion.identity);

        newBlock.GetComponent<Block>().pool = pool;
        newBlock.GetComponent<Block>().block_Controller = this.gameObject.GetComponent<Block_Controller>();
        blocks.Add(newBlock.GetComponent<Block>());
    }

    public List<Block> GetBlockList() {
        return this.blocks;
    }

    private void Start() {
      //  SoundManager.Instance.PlayMusic(musicClip);

        inGameUI = Main_Ui.Instance;
        FakeBlock = Instantiate(FakeBlock_Prefab, new Vector3(0, startY, 0), Quaternion.identity);

        CalculateMinBlocks();
        inGameUI.CurrentBlocks(CountAttchedBlocks(), minBlocks);
    }

    private void FixedUpdate() {
        CheckNextSide();
        FakeBlockMove();
    }

    private float maxSide = 3f;
    private bool right = true;

    private void CheckNextSide() {

        float YPos = (startY) + (blockPrefab.transform.localScale.y * (GetBlockList().Count));

        Vector3 RightSidePos = new Vector3(maxSide + (GetPenaltyBlocks() * 0.08f), YPos, FakeBlock.transform.position.z);
        Vector3 LeftSidePos = new Vector3(-maxSide - (GetPenaltyBlocks() * 0.08f), YPos, FakeBlock.transform.position.z);

        float posX = FakeBlock.transform.position.x;

        if (right && posX >= RightSidePos.x) {
            right = false;
        }

        if (!right && posX <= LeftSidePos.x) {
            right = true;
        }
    }


    private void FakeBlockMove() {

        if (!failed && !counting) {

            FakeBlock.GetComponent<MeshRenderer>().enabled = true;

            float YPos = (startY) + (blockPrefab.transform.localScale.y * (GetBlockList().Count));

            float ZPos = FakeBlock.transform.position.z;
            GameObject LastActive = GetLastActiveBlock();

            if (LastActive) {
                ZPos = LastActive.transform.position.z;
            }


            Vector3 RightSidePos = new Vector3(maxSide + (GetPenaltyBlocks() * 0.08f), YPos, FakeBlock.transform.position.z);
            Vector3 LeftSidePos = new Vector3(-maxSide - (GetPenaltyBlocks() * 0.08f), YPos, FakeBlock.transform.position.z);

            float penatltyTime = (0.2f * (GetPenaltyBlocks())) + 6.5f;

            if (right) {
                FakeBlock.transform.position = Vector3.MoveTowards(FakeBlock.transform.position, RightSidePos, Time.deltaTime * penatltyTime);
            } else {
                FakeBlock.transform.position = Vector3.MoveTowards(FakeBlock.transform.position, LeftSidePos, Time.deltaTime * penatltyTime);
            }
           


            FakeBlock.transform.position = new Vector3(FakeBlock.transform.position.x, YPos, ZPos);
            FakeBlock.transform.localScale = new Vector3(blockPrefab.transform.localScale.x + (GetPenaltyBlocks() * penaltySize), blockPrefab.transform.localScale.y, blockPrefab.transform.localScale.z + (GetPenaltyBlocks() * penaltySize));

        } else {

            FakeBlock.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    public void DropBlock() {
        if (FakeBlock && !counting && !clearing) {
            FakeBlock.transform.localScale = new Vector3(blockPrefab.transform.localScale.x + (GetPenaltyBlocks() * penaltySize), blockPrefab.transform.localScale.y, blockPrefab.transform.localScale.z + (GetPenaltyBlocks() * penaltySize));
        }
    }

    public GameObject GetLastActiveBlock() {
        for (int i = GetBlockList().Count; i > 0; i--) {
            if (GetBlockList()[i - 1] && GetBlockList()[i - 1].gameObject && GetBlockList()[i - 1].gameObject.activeSelf) {
                return GetBlockList()[i - 1].gameObject;
            }
        }
        return null;
         }
    

    private int penaltyBlocks = 0;
    public int GetPenaltyBlocks() {
        return this.penaltyBlocks;
    }

    public void ResetPenaltyPoints() {
        this.penaltyBlocks = 0;
    }

    public void IncreasePenaltyPoint(int amt) {
        this.penaltyBlocks += amt;
    }

    public void DestroyBlocks() {

        if (!failed) {

            int attchedBlocksAmt = CountAttchedBlocks();

            if (attchedBlocksAmt >= minBlocks) {

                int points = scoreCounter.CountDestructionPoints(blocks);
                SaveScore();

                RemoveAffordableBlocks();
                ClearCubes(0.005f, 1.5f); // too low time causes cube leftover
                blocks.Clear();
                ResetPenaltyPoints();

                inGameUI.AddScoreBonus(points);
                inGameUI.AddStackBonus(1.25f, attchedBlocksAmt);

                scoreCounter.AddScore(points + (goodLandings * 10));

                goodLandings = 0;
                turns += 1;

                CalculateMinBlocks();
                inGameUI.CurrentBlocks(0, minBlocks);

                counting = true;
            }
        }
    }

    private void RemoveAffordableBlocks() { // removes the top blocks so we dont have to loop trough the pool a few times

        List<GameObject> attchedBlocksListTemp = new List<GameObject>(GetAllAttchedBlocks());
        List<GameObject> blocksLeftList = new List<GameObject>(attchedBlocksListTemp);

        foreach (Block b in GetDestroyBlocks(attchedBlocksListTemp)) { // break up the top blocks
            if (b && b.gameObject && b.gameObject.activeSelf) {
                b.CreateCubes(false);
                blocksLeftList.Remove(b.gameObject);
            }
        }

            foreach (GameObject b in blocksLeftList) { // destroy the rest
                if (b && b && b.activeSelf) {
                    Block bScript = b.GetComponent<Block>();
                    bScript.CreateCubes(true);
                }
            }
    }

    private List<Block> GetDestroyBlocks(List<GameObject> blockListArg) { // gets the last blocks, so we dont spawn cubes for the bottom ones

        List<Block> tempBlockList = new List<Block>();

        int blocksAfford = standardPieces * standardPieces;
        int total = (cubePieces / blocksAfford) + 1;

        int startPos = (blockListArg.Count - total);

        if (startPos < 0) {
            startPos = 0;
        }

        for (int i = startPos; i < blockListArg.Count; i++) {
            Block tempBlock = blockListArg[i].GetComponent<Block>();
            if (tempBlock && tempBlock.gameObject && tempBlock.gameObject.activeSelf) {
                tempBlockList.Add(tempBlock);
            }
        }
        return tempBlockList;
    }

    public int GetGoodLandingPoints() {
        return (goodLandings * 10);
    }

        public void IncreaseGoodLanding() {
        goodLandings++;
    }

    public void AddNiceScore(int points) {

        scoreCounter.AddScore(points);
        inGameUI.AddScoreViaFunction(points);
    }

    private void CalculateMinBlocks() {
        minBlocks = (increaseBlocks * turns) + startMinBlocks;
    }

    public void ClearCubes(float time, float pauseTime) {

        if (CountAttchedBlocks() > 0) {

            removeBlockCour = RemoveCubeAfterTime(time, pauseTime);
            StartCoroutine(removeBlockCour);

            float createCubesTime = CountAttchedBlocks() * 0.025f; // block create time - check Block.cs

            //   WaitSpawnTimer = Time.time + ((time * pool.GetPoolDic().Count) + pauseTime) + createCubesTime;
        }
    }

    private int antiTipInterval = 30;
    private int lastAntiTip = 0;
    private void AntiTip() {

        int attachedAmt = CountAttchedBlocks();

        if ((attachedAmt - antiTipInterval)> lastAntiTip) {

            GameObject block = GetAllAttchedBlocks()[attachedAmt - antiTipInterval];
            Rigidbody rigid = block.GetComponent<Rigidbody>();
            Block blockScript = block.GetComponent<Block>();

            if (block && rigid) {

                rigid.velocity = Vector3.zero;
                rigid.angularVelocity = Vector3.zero;

               // block.GetComponent<Renderer>().material.color = Color.red;
                rigid.constraints = RigidbodyConstraints.FreezeAll;

                blockScript.BoxColiderControl(false);

                lastAntiTip += antiTipInterval;
            }
        }
    }

    private IEnumerator RemoveCubeAfterTime(float time, float pauseTime) {

        clearing = true;

        yield return new WaitForSeconds(pauseTime);
        WaitForSeconds wait = new WaitForSeconds(time);

        for(int i = pool.GetPoolDic().Count - 1; i >= 0; i--) {
            if (pool.GetPoolDic()[i] && pool.GetPoolDic()[i].gameObject.activeSelf) {
                pool.GetPoolDic()[i].gameObject.SetActive(false);
                yield return wait;
            }
        }

        clearing = false;
    }

    private IEnumerator FixbouncinessCourFunc() {
            yield return new WaitForSeconds(0.1f);
             Fixbounciness();
        }
  


    public void Fixbounciness() {
        foreach (GameObject obj in GetAllAttchedBlocks()) {
            if (obj && obj.GetComponent<Rigidbody>()) {

                Rigidbody rigid = obj.GetComponent<Rigidbody>();
                rigid.velocity = Vector3.zero;
                rigid.angularVelocity = Vector3.zero;
            }
        }
    }

    public void AttachedCompleted() {
        inGameUI.CurrentBlocks(CountAttchedBlocks(), minBlocks);

        velcoityFixCour = FixbouncinessCourFunc();
        StartCoroutine(velcoityFixCour);
    }

    public List<GameObject> GetAllAttchedBlocks() {
        List<GameObject> activeBlocks = new List<GameObject>();

        foreach (Block obj in GetBlockList()) {
            if (obj && obj.gameObject && obj.gameObject.activeSelf) {
                activeBlocks.Add(obj.gameObject);
            }
        }
        return activeBlocks;
    }

    public int CountAttchedBlocks() {
        int count = 0;

        foreach (Block obj in GetBlockList()) {
            if (obj && obj.gameObject && obj.gameObject.activeSelf && obj.hasAttached) {
                count += 1;
            }
        }
        return count;
    }



    public void FailAngleBlocks() {
        failed = true;

        SaveScore();
        CalculateMinBlocks();
        inGameUI.FailGame();

        RemoveAffordableBlocks();
        ClearCubes(0.005f, 5f);
        blocks.Clear();

        inGameUI.CurrentBlocks(CountAttchedBlocks(), minBlocks);
    }

    private void SaveScore() {
        int stackAmt = GetAllAttchedBlocks().Count;
        int currentMaxStack = Data_Script.Instance.GetPlayerStackScore();

        int highScore = Data_Script.Instance.GetPlayerHighScore();
        int currentScore = scoreCounter.GetScore();

        if (currentScore > highScore) {
            Data_Script.Instance.SetNewHighScore(currentScore);
        }

        if (stackAmt > currentMaxStack) {
            Data_Script.Instance.SetNewStackScore(stackAmt);
        }
    }
}
