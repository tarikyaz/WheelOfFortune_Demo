using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BlockingWait : MonoBehaviour
{
    [SerializeField]
    Slider sliderRef;

    [SerializeField]
    GameObject finishedPopup;

    [SerializeField] float adsDuration = 2;
    Tween sliderAnimTween;
    bool adsIsRewarded => sliderRef.value == 1;
    private void OnEnable()
    {
        RestAnimatin();
        Debug.Log("BlockingWait:OnEnable");
        sliderAnimTween = sliderRef.DOValue(1, adsDuration).From(0).OnComplete(() =>
        {
            sliderRef.value = 1;
            finishedPopup.gameObject.SetActive(true);
        });
    }

    public void CloseBlockingScreen()
    {
        if (adsIsRewarded)
        {
            BaseEvents.CallAddCoins(1);
        }
        RestAnimatin();
        finishedPopup.SetActive(false);
        gameObject.SetActive(false);
        Debug.Log("BlockingWait:CloseBlockingScreen");
    }
    void RestAnimatin()
    {
        sliderAnimTween.Pause();
        sliderAnimTween.Kill();
        sliderRef.value = 0;
        finishedPopup.gameObject.SetActive(false);
    }
}
