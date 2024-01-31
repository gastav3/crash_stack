using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Monetization;
using System.Collections.Generic;
using System.Collections;

public class RewardedAdsPlacement : MonoBehaviour {

    private GameObject _gameManangerObject;

    private string placementId = "rewardedVideo";

    private int rewardAmount = 0;

    private void Start() {

    }

    public void ShowAd(int rewardAmt) {
        rewardAmount = rewardAmt;
        StartCoroutine(WaitForAd());
    }

    IEnumerator WaitForAd() {
        while (!Monetization.IsReady(placementId)) {
            yield return null;
        }

        ShowAdPlacementContent ad = null;
        ad = Monetization.GetPlacementContent(placementId) as ShowAdPlacementContent;

        if (ad != null) {
            ad.Show(AdFinished);
        }
    }

    void AdFinished(ShowResult result) {
        if (result == ShowResult.Finished) {

          
        }
    }
}