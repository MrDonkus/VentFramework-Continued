using System;
using UnityEngine;
using System.Collections.Generic;
using VentLib.Logging;
using VentLib.Options.Interfaces;
using VentLib.Utilities.Collections;
using VentLib.Utilities.Optionals;

namespace VentLib.Options.UI.Tabs;

public abstract class VanillaTab: IGameOptionTab
{
    protected readonly StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(VanillaTab));
    protected UnityOptional<RoleSettingsTabButton> TabButton = UnityOptional<RoleSettingsTabButton>.Null();
    protected UnityOptional<RolesSettingsMenu> RelatedMenu = UnityOptional<RolesSettingsMenu>.Null();
    protected RoleRulesCategory roleCategory;

    private OrderedSet<GameOption> options = new();
    private readonly List<Action<IGameOptionTab>> callbacks = new();

    public void Activate()
    {
        log.Info($"Activated Vanilla Tab: \"{GetType().Name}\"", "TabSwitch");
        PassiveButton().IfPresent(pb => pb.SelectButton(true));
        RelatedMenu.IfPresent(menu => menu.ChangeTab(roleCategory, TabButton.Get().Button));
    }
    
    public void Deactivate()
    {
        log.Debug($"Deactivated Vanilla Tab: \"{GetType().Name}\"", "TabSwitch");
        PassiveButton().IfPresent(pb => pb.SelectButton(false));
        RelatedMenu.IfPresent(menu => {
            for (int i = 0; i < menu.advancedSettingChildren.Count; i++)
		    {
		    	UnityEngine.Object.Destroy(menu.advancedSettingChildren[i].gameObject);
		    }
            menu.ControllerSelectable.Clear();
            menu.advancedSettingChildren.Clear();
        });
    }

    // Ignores this when looking for options
    public bool Ignore() => true;

    public void AddEventListener(Action<IGameOptionTab> callback) => callbacks.Add(callback);

    public void AddOption(GameOption option)
    {
        if (options.Contains(option)) return;
        options.Add(option);
    }

    public void RemoveOption(GameOption option) => options.Remove(option);

    public void HandleClick()
    {
        callbacks.ForEach(cb => cb(this));
    }

    public abstract StringOption InitializeOption(StringOption sourceBehavior);

    public abstract void Setup(RolesSettingsMenu menu);

    public void SetPosition(Vector2 position)
    {
        TabButton.IfPresent(btn => btn.transform.localPosition = new Vector3(position.x, position.y, -2f));
    }
    
    public void Show()
    {
        TabButton.IfPresent(button => button.gameObject.SetActive(true));
    }

    public void Hide()
    {
        TabButton.IfPresent(button => button.gameObject.SetActive(false));
    }

    public abstract List<GameOption> PreRender();

    public Optional<Vector3> GetPosition() => TabButton.Map(btn => btn.transform.localPosition);

    public List<GameOption> GetOptions() => options.AsList();

    protected abstract UnityOptional<PassiveButton> PassiveButton();
}