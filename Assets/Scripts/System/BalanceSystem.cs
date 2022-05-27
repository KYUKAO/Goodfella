using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BalanceSystem :MonoBehaviour
{

    public float timeInTotal;
    public float timeRange;
    public float maxTimeRange;
    public float minTimeRange;
    public float changeRate;
    public bool isTiming;
    public KeyCode balanceKey;
    public GameObject BalanceImage;

    private bool isSuccess;
    private float timer;
    private BalanceSliderComponent[] balanceSliders;
    private BalancePointerComponent balancePointer;
    private void OnEnable()
    {
        ResetTimeRange();
        BalanceImage.SetActive(true);
        timer = 0;
        balanceSliders = BalanceImage.GetComponentsInChildren<BalanceSliderComponent>();
        balancePointer = BalanceImage.GetComponentInChildren<BalancePointerComponent>();
        StartNewCircle();
    }
    private void OnDisable()
    {
        if(BalanceImage)
             BalanceImage.SetActive(false);
    }
    private void Update()
    {
        timer += Time.deltaTime;
        timeRange = Mathf.Lerp(timeRange, minTimeRange, changeRate);
        foreach (var balanceSlider in balanceSliders)
        {
            balanceSlider.GetComponent<Image>().fillAmount = timeRange / timeInTotal * 0.50f;
        }
        balancePointer.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, -timer / timeInTotal*360.0f);
        if(timer>timeInTotal)
        {
            Debug.Log("one circle");
            StartNewCircle();
        }
        if(isTiming&& Input.GetKeyDown(balanceKey))
        {
            isSuccess = true;
            Debug.Log("success");
        }
    }
    private void StartNewCircle()
    {
        timer = 0;
        Invoke("EnableBalanceTiming", (timeInTotal - timeRange) / 2.0f);
        foreach (var balanceSlider in balanceSliders)
        {
            balanceSlider.GetComponent<Image>().fillAmount = timeRange / timeInTotal * 0.50f;
        }
    }
    private void ResetTimeRange()
    {
        timeRange = maxTimeRange;
    }
    private void EnableBalanceTiming()
    {
        isTiming = true;
        isSuccess = false;
        Invoke("DisableBalanceTiming", timeRange);
    }
    private void DisableBalanceTiming()
    {
        isTiming = false;
        if(!isSuccess)
        {
            Debug.Log("fail");
            Evently.Instance.Publish(new SeperateEvent());
        }
    }
}
