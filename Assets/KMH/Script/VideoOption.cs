using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VideoOption : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown; // Dropdown 옵션 값 넣기 위해 변수 선언
    public TMP_Dropdown fullScreenDropdown; // Dropdown 옵션 값 넣기 위해 변수 선언

    List<Resolution> resolutions = new List<Resolution>();
    private FullScreenMode currentFullScreenMode;

    private void Start()
    {
        InitUI();
    }


    void InitUI()
    {
        //AddRange 함수 이용하여 지원하는 해상도 리스트에 넣기
        resolutions.AddRange(Screen.resolutions);

        resolutionDropdown.options.Clear(); // Dropdown에 있는 기존 정보 제거

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Count; i++)
        {
            Resolution res = resolutions[i];
            string optionText = $"{res.width}x{res.height}@{res.refreshRateRatio.value}hz";
            resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(optionText));

            if (res.width == Screen.currentResolution.width &&
                res.height == Screen.currentResolution.height &&
                res.refreshRateRatio.Equals(Screen.currentResolution.refreshRateRatio))
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();


        //현재 전체 화면 상태 설정
        bool isFullScreen = Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen || Screen.fullScreenMode == FullScreenMode.FullScreenWindow;
        currentFullScreenMode = isFullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;

        resolutionDropdown.onValueChanged.AddListener(ChangeResolution);
        fullScreenDropdown.onValueChanged.AddListener(ChangeFullScreenMode);
    }
    public void ChangeResolution(int index)
    {
        Resolution selected = resolutions[index];
        Screen.SetResolution(
            selected.width,
            selected.height,
            currentFullScreenMode,
            selected.refreshRateRatio
            );
        Debug.Log(index);
    }

    public void ChangeFullScreenMode(int index)
    {
        if (index == 0)
        {
            currentFullScreenMode = FullScreenMode.FullScreenWindow;
            Debug.Log("전체화면");
        }
        else
        {
            currentFullScreenMode = FullScreenMode.Windowed;
            Debug.Log("창모드");
        }

        //현재 선택된 해상도 다시 적용
        ChangeResolution(resolutionDropdown.value);
    }

    void OnDestroy()
    {
        resolutionDropdown.onValueChanged.RemoveListener(ChangeResolution);
        fullScreenDropdown.onValueChanged.RemoveListener(ChangeFullScreenMode);
    }
}