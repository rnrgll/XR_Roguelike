using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.VisionOS;
using UnityEngine;
using UnityEngine.UI;

public class VideoOption : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown; // Dropdown 옵션 값 넣기 위해 변수 선언
    public Toggle fullScreenToggle; // 토글 버튼으로 스크린 모드 설정
    public TMP_Dropdown testDD;

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
        fullScreenToggle.isOn = isFullScreen;
        currentFullScreenMode = isFullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;

        fullScreenToggle.onValueChanged.AddListener(ChangeFullScreenMode);
    }
    void ChangeResolution(int index)
    {
        Resolution selected = resolutions[index];
        Screen.SetResolution(
            selected.width,
            selected.height,
            currentFullScreenMode,
            selected.refreshRateRatio
            );
    }

    void ChangeFullScreenMode(bool isFullScreen)
    {
        currentFullScreenMode = isFullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;

        //현재 선택된 해상도 다시 적용
        ChangeResolution(resolutionDropdown.value);
    }

    void OnDestroy()
    {
        resolutionDropdown.onValueChanged.RemoveListener(ChangeResolution);
        fullScreenToggle.onValueChanged.RemoveListener(ChangeFullScreenMode);
    }
}
