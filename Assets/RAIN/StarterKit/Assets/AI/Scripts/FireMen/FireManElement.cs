using UnityEngine;
using RAIN.Core;
using RAIN.Serialization;
using RAIN.Navigation.Targets;
using RAIN.Navigation;

[RAINSerializableClass]
public class FireManElement : CustomAIElement
{
    [RAINSerializableField]
    private GameObject _fireTruckObject = null;

    private FireTruckElement _fireTruck = null;

    private Transform _oldParent = null;

    public GameObject FireTruckObject
    {
        get { return _fireTruckObject; }
        set
        {
            if (_fireTruckObject == value)
                return;

            _fireTruckObject = value;

            if (_fireTruckObject == null)
                _fireTruck = null;
            else
                _fireTruck = _fireTruckObject.GetComponentInChildren<AIRig>().AI.GetCustomElement<FireTruckElement>();
        }
    }

    public FireTruckElement FireTruck
    {
        get { return _fireTruck; }
    }

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
        set { AI.WorkingMemory.SetItem("target", value); }
    }

    public bool IsTruckLoaded
    {
        get { return AI.WorkingMemory.GetItem<bool>("istruckloaded"); }
    }

    public override void AIStart()
    {
        base.AIStart();

        // Resolve our fire truck element
        _fireTruck = _fireTruckObject.GetComponentInChildren<AIRig>().AI.GetCustomElement<FireTruckElement>();

        // Set this for the behavior tree
        AI.WorkingMemory.SetItem("firetruck", _fireTruckObject);
    }

    public virtual void LoadTruck()
    {
        // Turn off our renderers (a cheap move, but works alright)
        Renderer[] tRenderers = AI.Body.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < tRenderers.Length; i++)
            tRenderers[i].enabled = false;

        // Parent us to the truck
        _oldParent = AI.Body.transform.parent;
        AI.Body.transform.parent = _fireTruckObject.transform;

        // And loaded
        AI.WorkingMemory.SetItem("istruckloaded", true);
    }

    public virtual void UnloadTruck()
    {
        // Turn them back on, won't work if we had some disabled for other reasons
        Renderer[] tRenderers = AI.Body.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < tRenderers.Length; i++)
            tRenderers[i].enabled = true;

        // Unparent us
        AI.Body.transform.parent = _oldParent;

        // And unloaded
        AI.WorkingMemory.SetItem("istruckloaded", false);
    }
}
