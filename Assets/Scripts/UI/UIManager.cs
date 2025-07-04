using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Linq;

/// <summary>
/// A class that manages UI pages and handles game UI logic including pause, navigation, and effects.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Page Management")]
    public List<UIPage> pages;
    public int currentPage = 0;
    public int defaultPage = 0;

    [Header("Pause Settings")]
    public int pausePageIndex = 1;
    public bool allowPause = true;

    [Header("Polish Effects")]
    public GameObject navigationEffect;
    public GameObject clickEffect;
    public GameObject backEffect;

    [Header("Input Actions & Controls")]
    public InputAction pauseAction;

    private bool isPaused = false;
    private List<UIelement> UIelements;
    [HideInInspector] public EventSystem eventSystem;

    public UnityEvent<int> onPageChanged;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            pauseAction.Enable();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnDestroy()
    {
        pauseAction.Disable();
    }

    private void Start()
    {
        SetUpEventSystem();
        SetUpUIElements();
        InitializeFirstPage();
        UpdateUI();
    }

    private void Update()
    {
        CheckPauseInput();
    }

    private void OnEnable()
    {
        pauseAction.Enable();
    }

    private void OnDisable()
    {
        pauseAction.Disable();
    }

    private void SetUpUIElements()
    {
        UIelements = FindObjectsOfType<UIelement>().ToList();
    }

    private void SetUpEventSystem()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            Debug.LogWarning("There is no event system in the scene but you are trying to use the UIManager.\n" +
                             "All UI in Unity requires an Event System to run.\n" +
                             "You can add one by right clicking in the hierarchy and selecting UI -> EventSystem.");
        }
    }

    private void CheckPauseInput()
    {
        if (pauseAction.triggered)
        {
            TogglePause();
        }
    }

    public void TogglePause()
{
    if (!allowPause) return;

    isPaused = !isPaused;

    if (isPaused)
    {
        if (CursorManager.instance != null)
        {
            CursorManager.instance.ChangeCursorMode(CursorManager.CursorState.Menu);
        }
        GoToPage(pausePageIndex);
        Time.timeScale = 0;
    }
    else
    {
        if (CursorManager.instance != null)
        {
            CursorManager.instance.ChangeCursorMode(CursorManager.CursorState.FPS);
        }
        GoToPage(defaultPage);
        Time.timeScale = 1;
    }
}


    public void CreateBackEffect()
    {
        if (backEffect)
            Instantiate(backEffect, transform.position, Quaternion.identity);
    }

    public void CreateClickEffect()
    {
        if (clickEffect)
            Instantiate(clickEffect, transform.position, Quaternion.identity);
    }

    public void CreateNavigationEffect()
    {
        if (navigationEffect)
            Instantiate(navigationEffect, transform.position, Quaternion.identity);
    }

    public void UpdateUI()
    {
        foreach (UIelement uiElement in UIelements)
        {
            uiElement.UpdateUI();
        }
    }

    private void InitializeFirstPage()
    {
        GoToPage(defaultPage);
    }

    public void GoToPage(int pageIndex)
    {
        if (pageIndex < pages.Count && pages[pageIndex] != null)
        {
            SetActiveAllPages(false);
            pages[pageIndex].gameObject.SetActive(true);
            pages[pageIndex].SetSelectedUIToDefault();
            currentPage = pageIndex;
            onPageChanged?.Invoke(pageIndex);
        }
    }

    public void GoToPageByName(string pageName)
    {
        UIPage page = pages.Find(item => item.name == pageName);
        if (page != null)
        {
            GoToPage(pages.IndexOf(page));
        }
    }

    public void SetActiveAllPages(bool activated)
    {
        foreach (UIPage page in pages)
        {
            if (page != null)
                page.gameObject.SetActive(activated);
        }
    }
}
