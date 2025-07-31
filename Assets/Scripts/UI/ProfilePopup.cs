using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfilePopup {
    #region Fields

    private const string menuPath = GameController.menusPath + "profilePopup";

    private Button closeButton;
    private Button backgroundClose;
    private GameObject popup;
    private RectTransform popupRect;
    
    private Button englishLanguageButton;
    private GameObject englishLanguageTickIcon;
    
    private Button russianLanguageButton;
    private GameObject russianLanguageTickIcon;

    private Slider _bananaUpgradesSlider;
    private TextMeshProUGUI _bananaUpgradesText;
    private TextMeshProUGUI _bananaUpgradesEffectText;
    
    private Slider _bananaClicksSlider;
    private TextMeshProUGUI _bananaClicksText;
    private TextMeshProUGUI _bananaClicksEffectText;

    public bool menuIsOpen;

    private float animationDuration = 0.3f;

    #endregion

    #region Initialize

    public void Initialize()
    {
        popup = GameObject.Find(menuPath);
        popupRect = GameObject.Find(menuPath + "/bg").GetComponent<RectTransform>();

        closeButton = GameObject.Find(menuPath + "/bg/closeButton").GetComponent<Button>();
        closeButton.onClick.AddListener(ClosePopup);

        backgroundClose = GameObject.Find(menuPath + "/background").GetComponent<Button>();
        backgroundClose.onClick.AddListener(ClosePopup);
        
        InitializeAchievements();
        InitializeLanguageChangePanel();
        
        GameEntities.Translator.LanguageChanged += OnLanguageChanged;
        OnLanguageChanged();
    }
    
    private void InitializeAchievements()
    {
        _bananaUpgradesSlider = GameObject.Find(menuPath + "/bg/bananaUpgradesPanel/progressBar/slider").GetComponent<Slider>();
        _bananaUpgradesText = GameObject.Find(menuPath + "/bg/bananaUpgradesPanel/progressBar/value").GetComponent<TextMeshProUGUI>();
        _bananaUpgradesEffectText = GameObject.Find(menuPath + "/bg/bananaUpgradesPanel/effectLayout/quantity").GetComponent<TextMeshProUGUI>();
        
        _bananaClicksSlider = GameObject.Find(menuPath + "/bg/bananaClicksPanel/progressBar/slider").GetComponent<Slider>();
        _bananaClicksText = GameObject.Find(menuPath + "/bg/bananaClicksPanel/progressBar/value").GetComponent<TextMeshProUGUI>();
        _bananaClicksEffectText = GameObject.Find(menuPath + "/bg/bananaClicksPanel/effectLayout/quantity").GetComponent<TextMeshProUGUI>();
    }

    private void InitializeLanguageChangePanel()
    {
        englishLanguageButton =  GameObject.Find(menuPath + "/bg/languagePanel/layout/englishToggle").GetComponent<Button>();
        englishLanguageTickIcon = GameObject.Find(menuPath + "/bg/languagePanel/layout/englishToggle/frame/tickIcon");
        englishLanguageButton.onClick.AddListener(() => ChangeLanguage(Languages.English));
        
        russianLanguageButton =  GameObject.Find(menuPath + "/bg/languagePanel/layout/russianToggle").GetComponent<Button>();
        russianLanguageTickIcon = GameObject.Find(menuPath + "/bg/languagePanel/layout/russianToggle/frame/tickIcon");
        russianLanguageButton.onClick.AddListener(() => ChangeLanguage(Languages.Russian));
    }
    #endregion
    
    public void OpenPopup()
    {
        GameEntities.GameController.StartCoroutine(GameEntities.GameController.InitializeMenu(MenuName.ProfilePopup, Initialize, OpenPopupActions));
    }

    private void OpenPopupActions()
    {
        if (!menuIsOpen)
        {
            GameEntities.GameController.StartCoroutine(TogglePopupWithZoomAnimation(true, popupRect, Vector3.zero,
                Vector3.one));
            popup.transform.SetAsLastSibling();
            menuIsOpen = true;
            OpenBananaClicksAction();
            OpenBananaUpgradesAction();
        }
    }

    private void OpenBananaClicksAction()
    {
        var achievement = Achievement.BananasClicked;
        var currentMilestoneIndex = GameEntities.Achievements.GetCurrentMilestoneIndex(Achievement.BananasClicked);
        var nextMilestoneIndex = Mathf.Clamp(currentMilestoneIndex + 1, 0, Achievements.achievementMilestones[achievement].Count - 1);

        var previousValue = currentMilestoneIndex <= 0 ? 0 : Achievements.achievementMilestones[achievement][currentMilestoneIndex];
        var nextValue = Achievements.achievementMilestones[achievement][nextMilestoneIndex];
        var currentValue = GameEntities.Achievements.AchievementsValues[achievement];

        _bananaClicksSlider.minValue = previousValue;
        _bananaClicksSlider.maxValue = nextValue;
        _bananaClicksSlider.value = currentValue;
        _bananaClicksText.text = $"{currentValue}/{nextValue}";
        _bananaClicksEffectText.text = $"+{GameEntities.Achievements.GetAchievementEffect(achievement)}";
    }
    
    private void OpenBananaUpgradesAction()
    {
        var achievement = Achievement.BananaUpgrades;
        var currentMilestoneIndex = GameEntities.Achievements.GetCurrentMilestoneIndex(achievement);
        var nextMilestoneIndex = Mathf.Clamp(currentMilestoneIndex + 1, 0, Achievements.achievementMilestones[achievement].Count - 1);

        var previousValue = currentMilestoneIndex <= 0 ? 0 : Achievements.achievementMilestones[achievement][currentMilestoneIndex];
        var nextValue = Achievements.achievementMilestones[achievement][nextMilestoneIndex];
        var currentValue = GameEntities.Achievements.AchievementsValues[achievement];

        _bananaUpgradesSlider.minValue = previousValue;
        _bananaUpgradesSlider.maxValue = nextValue;
        _bananaUpgradesSlider.value = currentValue;
        _bananaUpgradesText.text = $"{currentValue}/{nextValue}";
        _bananaUpgradesEffectText.text = $"+{GameEntities.Achievements.GetAchievementEffect(achievement)}";
    }

    public void ClosePopup()
    {
        if (menuIsOpen)
        {
            GameEntities.GameController.StartCoroutine(TogglePopupWithZoomAnimation(false, popupRect, Vector3.one, Vector3.zero));
            menuIsOpen = false;
        }
    }

    private IEnumerator TogglePopupWithZoomAnimation(bool Open, RectTransform PopupRect, Vector3 StartScale, Vector3 FinalScale)
    {
        PopupRect.localScale = StartScale;

        if (Open)
        {
            PopupRect.DOKill(true);
            PopupRect.parent.gameObject.SetActive(true);
        }

        yield return new WaitForEndOfFrame();

        PopupRect.DOScale(FinalScale, animationDuration).OnComplete(() =>
        {
            if (!Open) PopupRect.parent.gameObject.SetActive(false);
        });
    }

    public void OnLanguageChanged()
    {
        var translator = GameEntities.Translator;
        
        englishLanguageTickIcon.SetActive(translator.CurrentLanguage == Languages.English);
        russianLanguageTickIcon.SetActive(translator.CurrentLanguage == Languages.Russian);
        
        GameObject.Find(menuPath + "/bg/bananaClicksPanel/title").GetComponent<TextMeshProUGUI>().text = translator.GetLocalization("totalBananaClicks");
        GameObject.Find(menuPath + "/bg/bananaUpgradesPanel/title").GetComponent<TextMeshProUGUI>().text = translator.GetLocalization("totalBananaUpgrades");
        GameObject.Find(menuPath + "/bg/languagePanel/title").GetComponent<TextMeshProUGUI>().text = translator.GetLocalization("totalBananaUpgrades");
    }

    private void ChangeLanguage(Languages language)
    {
        GameEntities.Translator.ChangeLanguage(language);
    }
}
