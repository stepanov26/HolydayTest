using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPopup {

    #region Fields
    private const string menuPath = GameController.menusPath + "infoPopup";

    private Button infoCloseButton;
    private Button infoBackgroundClose;
    private GameObject infoPopUp;
    private RectTransform infoPopUpRect;

    public bool infoMenuIsOpen;

    private float animationDuration = 0.3f;
    #endregion

    #region Initialize
    public void Initialize() {

        infoPopUp = GameObject.Find(menuPath);
        infoPopUpRect = GameObject.Find(menuPath + "/bg").GetComponent<RectTransform>();

        infoCloseButton = GameObject.Find(menuPath + "/bg/closeButton").GetComponent<Button>();
        infoCloseButton.onClick.AddListener(() => {
            ClosePopup();
        });

        infoBackgroundClose = GameObject.Find(menuPath + "/background").GetComponent<Button>();
        infoBackgroundClose.onClick.AddListener(() => {
            ClosePopup();
        });
        
        GameEntities.Translator.LanguageChanged += OnLanguageChanged;
        OnLanguageChanged();
    }
    #endregion

    public void OpenPopup() {
        GameEntities.GameController.StartCoroutine(GameEntities.GameController.InitializeMenu(MenuName.InfoPopup, Initialize, OpenPopupActions));
    }

    private void OpenPopupActions() {

        if (!infoMenuIsOpen) {
            GameEntities.GameController.StartCoroutine(TogglePopupWithZoomAnimation(true, infoPopUpRect, Vector3.zero, Vector3.one));
            infoPopUp.transform.SetAsLastSibling();
            infoMenuIsOpen = true;
        }
    }

    public void ClosePopup() {

        if (infoMenuIsOpen) {
            GameEntities.GameController.StartCoroutine(TogglePopupWithZoomAnimation(false, infoPopUpRect, Vector3.one, Vector3.zero));
            infoMenuIsOpen = false;
            GameEntities.Translator.LanguageChanged -= OnLanguageChanged;
        }
    }

    private IEnumerator TogglePopupWithZoomAnimation(bool Open, RectTransform PopupRect, Vector3 StartScale, Vector3 FinalScale) {

        PopupRect.localScale = StartScale;

        if (Open) {
            PopupRect.DOKill(true);
            PopupRect.parent.gameObject.SetActive(true);
        }

        yield return new WaitForEndOfFrame();

        PopupRect.DOScale(FinalScale, animationDuration).OnComplete(() => { if (!Open) PopupRect.parent.gameObject.SetActive(false); });
    }

    private void OnLanguageChanged()
    {
        var translator = GameEntities.Translator;
        
        GameObject.Find(menuPath + "/bg/title").GetComponent<TextMeshProUGUI>().text = translator.GetLocalization("howTheGameWorks");
        GameObject.Find(menuPath + "/bg/content/generalInfo").GetComponent<TextMeshProUGUI>().text = translator.GetLocalization("generalInfo");
        GameObject.Find(menuPath + "/bg/content/upgradeInfo").GetComponent<TextMeshProUGUI>().text = translator.GetLocalization("upgradeInfo");
        GameObject.Find(menuPath + "/bg/content/progressInfo").GetComponent<TextMeshProUGUI>().text = translator.GetLocalization("progressInfo");
    }
}