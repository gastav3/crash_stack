using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Monetization;

public class Ad_Handler : MonoBehaviour
{
    private string gameId = "3242746";

    private RewardedAdsPlacement reward_ad;
    private NormalAd normal_ad;

    private static Ad_Handler _instance;
    public static Ad_Handler Instance { get { return _instance; } }

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    private void Start() {
        Monetization.Initialize(gameId, false); // Set to false to monitize for real

        reward_ad = this.gameObject.AddComponent<RewardedAdsPlacement>() as RewardedAdsPlacement;
        normal_ad = this.gameObject.AddComponent<NormalAd>() as NormalAd;
    }

    public void ShowRewardVideoForAmount(int amt) {
        reward_ad.ShowAd(amt);
    }

    public void ShowNormalAd() {
        normal_ad.ShowAd();
    }

    public void MaybeShowAd(int range) {
        int rand = Random.Range(1, range);

        if (rand == 1) {
            normal_ad.ShowAd();
        }
    }
}
