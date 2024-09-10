using TMPro;
using UnityEngine;
using VentLib.Logging;
using VentLib.Options.Extensions;
using VentLib.Options.Interfaces;
using VentLib.Options.UI.Controllers;
using VentLib.Options.UI.Options;
using VentLib.Utilities.Extensions;

namespace VentLib.Options.UI.Renderer;

public class SettingsRenderer: IGameOptionRenderer
{
    private static readonly StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(SettingsRenderer));
    private static readonly Color[] Colors = { Color.green, Color.red, Color.blue };
    private float Height = 0f;
    
    public void SetHeight(float height) => Height = height;
    public float GetHeight() => Height;
    public void RenderTabs(IGameOptionTab[] tabs)
    {
        
    }

    public void PreRender(GameOption option, RenderOptions renderOptions, GameOptionsMenu menu)
    {
        bool isTitle = option.IsTitle;
        
        if (isTitle)
        {
            return;
        }
        
        OptionBehaviour Behaviour = option.GetBehaviour();
        // Behaviour.transform.FindChild("Title Text").GetComponent<RectTransform>().sizeDelta = new Vector2(3.5f, 0.37f);
        
        if (option.OptionType != Enum.OptionType.Bool) {
            Behaviour.transform.FindChild("MinusButton (1)").localPosition += new Vector3(0.3f, 0f, 0f);
            Behaviour.transform.FindChild("PlusButton (1)").localPosition += new Vector3(0.3f, 0f, 0f);
            Behaviour.transform.FindChild("Value_TMP (1)").localPosition += new Vector3(0.3f, 0f, 0f);
            Behaviour.transform.FindChild("ValueBox").localPosition += new Vector3(0.3f, 0f, 0f);
        } else {
            Behaviour.transform.FindChild("Toggle").localPosition += new Vector3(0.3f, 0f, 0f);
        }
        Behaviour.transform.FindChild("Title Text").transform.localPosition -= new Vector3(0.15f, 0f, 0f);
    }

    
    public void Render(GameOption option, (int level, int index) info, RenderOptions renderOptions, GameOptionsMenu menu)
    {
        if (option.OptionType == Enum.OptionType.Title)
        {
            CategoryHeaderMasked categoryHeader = (option as UndefinedOption).Header.Get();
            categoryHeader.transform.localPosition = new Vector3(-0.903f, Height, -2f);
            categoryHeader.transform.parent = menu.settingsContainer;
            categoryHeader.gameObject.SetActive(SettingsOptionController.ModSettingsOpened);
            Height -= 0.64f;
            return;
        }
        int lvl = info.level - 1;
        OptionBehaviour Behaviour = option.GetBehaviour();
        
        Transform transform = Behaviour.transform;
        SpriteRenderer render = Behaviour.transform.Find("LabelBackground").GetComponent<SpriteRenderer>();
        if (lvl > 0)
        {
            render.color = Colors[Mathf.Clamp((lvl - 1) % 3, 0, 2)];
            render.size = new Vector2((float)(4.8f - (lvl - 1) * 0.2), 0.45f); // was -0.95
            Behaviour.transform.Find("Title Text").transform.localPosition = new Vector3(-0.885f + 0.23f * Mathf.Clamp(lvl - 1, 0, int.MaxValue), 0f);
            //transform.FindChild("Title_TMP").GetComponent<RectTransform>().sizeDelta = new Vector2(3.4f, 0.37f);
            render.transform.localPosition = new Vector3(0.1f + 0.11f * (lvl - 1), 0f);
        }

        transform.localPosition = new Vector3(0.952f, Height, -2f);   
        transform.parent = menu.settingsContainer;
        Behaviour.gameObject.SetActive(SettingsOptionController.ModSettingsOpened);
        Height -= 0.45f;
    }

    public void PostRender(GameOptionsMenu menu)
    {
        if (!SettingsOptionController.ModSettingsOpened) return;
		menu.scrollBar.SetYBoundsMax(-Height - 1.65f);
    }

    public void Close()
    {
    }
}