using System;
using UnityEngine;
using TMPro;
using VentLib.Options.Interfaces;
using VentLib.Utilities.Optionals;
using VentLib.Utilities.Attributes;

using Object = UnityEngine.Object;
using VentLib.Utilities.Extensions;

namespace VentLib.Options.UI.Controllers.Search;

[LoadStatic]
public static class SearchBarController
{
    public static string CurrentText
    {
        get 
        {
            if (Enabled && searchBarObject.Exists()) return searchBarObject.Get().Text.ToLower();
            else return "";
        }
    }
    private static UnityOptional<GameSettingMenu> _lastInitialized = new();
    private static UnityOptional<FreeChatInputField> searchBarObject = new();
    private static ISearchBar SearchBarInfo;
    internal static bool Enabled = true;

    static SearchBarController()
    {
        SearchBarInfo = new SearchBar();
    }

    internal static void HandleOpen(GameSettingMenu menu)
    {
        _lastInitialized = UnityOptional<GameSettingMenu>.NonNull(menu);
        if (!Enabled) return;
        searchBarObject.OrElseSet(GetSearchBar);
        FreeChatInputField searchbar = searchBarObject.Get();
        searchbar.transform.SetParent(menu.GamePresetsButton.transform.parent, false);
        searchbar.transform.localScale = new Vector3(0.3f, 0.59f, 1);
        searchbar.transform.localPosition = new Vector3(-2.07f, -2.57f, -30f); 
    }

    public static void SetEnabled(bool isEnabled)
    {
        if (Enabled == isEnabled) return;
        Enabled = isEnabled;
        if (!_lastInitialized.Exists())
        {
            return;
        }
        if (Enabled) HandleOpen(_lastInitialized.Get());
        else if (searchBarObject.Exists()) searchBarObject.Get().Destroy();
    }

    public static void SetSearchInfo(ISearchBar newSearchInfo)
    {
        if (newSearchInfo == null) throw new ArgumentNullException("Argument newSearchInfo is null in SetSearchInfo.");
        SearchBarInfo = newSearchInfo;
    }

    internal static FreeChatInputField GetSearchBar()
    {
        FreeChatInputField searchBar = Object.Instantiate(DestroyableSingleton<ChatController>.Instance.freeChatField);
        searchBar.textArea.outputText.transform.localScale = new Vector3(3.5f, 2f, 1f);
        searchBar.name = "SearchBar";

        searchBar.textArea.characterLimit = SearchBarInfo.CharLimit();

        searchBar.FindChild<TextMeshPro>("CharCounter (TMP)").gameObject.SetActive(false);
        searchBar.FindChild<PassiveButton>("ChatSendButton").gameObject.SetActive(false);

        searchBar.FindChild<SpriteRenderer>("Background").sprite = SearchBarInfo.SearchBarSprite();

        if (!SearchBarInfo.HoverColor()) searchBar.textArea.Background = null;

        GameObject searchIcon = Object.Instantiate(searchBar, searchBar.transform).gameObject;
        searchIcon.name = "SearchIcon";
        searchIcon.GetComponents<Component>().ForEach(c => {
           if (c is not Transform) c.Destroy();
        });
        searchIcon.transform.DestroyChildren();
        searchIcon.transform.localScale = new Vector3(0.1f, 0.05f, 1f);
        searchIcon.transform.localPosition = new Vector3(SearchBarInfo.IconLocalPosition().x, SearchBarInfo.IconLocalPosition().y, -1f);

        SpriteRenderer iconRenderer = searchIcon.AddComponent<SpriteRenderer>();
        iconRenderer.sprite = SearchBarInfo.SearchIconSprite();

        return searchBar;
    }
}