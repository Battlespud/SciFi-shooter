using UnityEngine;
using RAIN.Core;
using RAIN.Serialization;
using RAIN.Navigation;
using RAIN.Navigation.Targets;

[RAINSerializableClass]
public class FireTruckElement : CustomAIElement
{
    public GameObject CommandTarget
    {
        get { return AI.WorkingMemory.GetItem<GameObject>("commandtarget"); }
        set
        {
            AI.WorkingMemory.SetItem("commandtarget", value);

            if (value == null)
                Target = null;
            else
                Target = NavigationManager.Instance.GetNavigationTarget(value.name + "_" + AI.Body.name);
        }
    }

    public NavigationTarget Target
    {
        get { return AI.WorkingMemory.GetItem<NavigationTarget>("target"); }
        set
        {
            AI.WorkingMemory.SetItem("target", value);
            AI.WorkingMemory.SetItem("arrived", false);
        }
    }

    public bool Arrived
    {
        get
        {
            return AI.WorkingMemory.GetItem<bool>("arrived");
        }
    }
}
