using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WeaponAttachmentSystemUI : MonoBehaviour {


    [SerializeField] private WeaponBodyListSO weaponBodyListSO;
    [SerializeField] private Button pistolBodyButton;
    [SerializeField] private Button rifleABodyButton;
    [SerializeField] private Button rifleBBodyButton;
    [SerializeField] private Button randomizeButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Button togglePartsButton;
    [SerializeField] private Transform loadContainer;
    [SerializeField] private Transform loadTemplate;
    [SerializeField] private Camera screenshotCamera;


    private Action<Texture2D> onScreenshotTaken;


    private void Awake() {
        RenderPipelineManager.endFrameRendering += RenderPipelineManager_endFrameRendering;

        pistolBodyButton.onClick.AddListener(() => {
            WeaponAttachmentSystem.Instance.SetWeaponBody(weaponBodyListSO.pistolWeaponBodySO);
        });
        rifleABodyButton.onClick.AddListener(() => {
            WeaponAttachmentSystem.Instance.SetWeaponBody(weaponBodyListSO.rifleAWeaponBodySO);
        });
        rifleBBodyButton.onClick.AddListener(() => {
            WeaponAttachmentSystem.Instance.SetWeaponBody(weaponBodyListSO.rifleBWeaponBodySO);
        });

        randomizeButton.onClick.AddListener(() => {
            WeaponAttachmentSystem.Instance.RandomizeParts();
        });

        togglePartsButton.onClick.AddListener(() => {
            WeaponBodyUI.Instance.ToggleVisibility();
        });

        playButton.onClick.AddListener(() => {
            WeaponAttachmentSystemGame.weaponCompleteJson = WeaponAttachmentSystem.Instance.GetWeaponComplete().Save();
            SceneManager.LoadScene(1);
        });

        saveButton.onClick.AddListener(SaveWeapon);

        loadTemplate.gameObject.SetActive(false);

        WeaponSaveSystem.GetSaveFilenameList();
    }

    private void Start() {
        UpdateLoadButtons();
    }

    private void RenderPipelineManager_endFrameRendering(ScriptableRenderContext arg1, Camera[] arg2) {
        if (onScreenshotTaken != null) {
            RenderTexture renderTexture = screenshotCamera.targetTexture;

            Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
            renderResult.ReadPixels(rect, 0, 0);

            RenderTexture.ReleaseTemporary(renderTexture);
            screenshotCamera.targetTexture = null;

            onScreenshotTaken(renderResult);
            onScreenshotTaken = null;

            screenshotCamera.enabled = false;
        }
    }

    private void UpdateLoadButtons() {
        foreach (Transform child in loadContainer) {
            if (child == loadTemplate) continue;
            Destroy(child.gameObject);
        }

        int loadFileCount = 0;
        foreach (string filename in WeaponSaveSystem.GetSaveFilenameList()) {
            WeaponSaveSystem.Load(filename, out string json, out Texture2D screenshotTexture2D);

            Transform loadTransform = Instantiate(loadTemplate, loadContainer);
            loadTransform.gameObject.SetActive(true);
            WeaponLoadButtonUI weaponLoadButtonUI = loadTransform.GetComponent<WeaponLoadButtonUI>();

            weaponLoadButtonUI.Setup(json, screenshotTexture2D);

            loadFileCount++;
        }

        GridLayoutGroup loadContainerGridLayoutGroup = loadContainer.GetComponent<GridLayoutGroup>();
        float rowHeight = loadContainerGridLayoutGroup.cellSize.y + loadContainerGridLayoutGroup.spacing.y;

        int numberOfLoadFilesPerRow = 2;
        loadContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(0, rowHeight * (Mathf.Floor(loadFileCount / numberOfLoadFilesPerRow) + 1));
    }

    private void SaveWeapon() {
        WeaponComplete weaponComplete = WeaponAttachmentSystem.Instance.GetWeaponComplete();

        string json = weaponComplete.Save();

        byte[] jsonByteArray = Encoding.Unicode.GetBytes(json);

        TakeScreenshot(256, 256, (Texture2D screenshotTexture) => {
            byte[] screenshotByteArray = screenshotTexture.EncodeToPNG();

            List<byte> byteList = new List<byte>(jsonByteArray);
            byteList.AddRange(screenshotByteArray);

            WeaponSaveSystem.Save(json, screenshotTexture);

            UpdateLoadButtons();
        });
    }

    private void TakeScreenshot(int width, int height, Action<Texture2D> onScreenshotTaken) {
        WeaponAttachmentSystem.Instance.ResetWeaponRotation();

        screenshotCamera.targetTexture = RenderTexture.GetTemporary(width, height, 16);
        screenshotCamera.enabled = true;

        this.onScreenshotTaken = onScreenshotTaken;
    }

}