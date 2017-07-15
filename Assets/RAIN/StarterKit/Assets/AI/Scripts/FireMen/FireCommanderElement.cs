using UnityEngine;
using RAIN.Core;
using RAIN.Serialization;
using System.Collections.Generic;

[RAINSerializableClass]
public class FireCommanderElement : FireManElement
{
    [RAINSerializableField]
    private List<GameObject> _fireMenObjects = new List<GameObject>();

    private List<FireManElement> _fireMen = new List<FireManElement>();

    public bool AreFireMenLoaded
    {
        get
        {
            for (int i = 0; i < _fireMen.Count; i++)
            {
                if (!_fireMen[i].IsTruckLoaded)
                    return false;
            }

            return true;
        }
    }

    public override void AIStart()
    {
        base.AIStart();

        // And our fire men
        _fireMen.Clear();
        for (int i = 0; i < _fireMenObjects.Count; i++)
            _fireMen.Add(_fireMenObjects[i].GetComponentInChildren<AIRig>().AI.GetCustomElement<FireManElement>());
    }

    public void LoadFireMen()
    {
        // This lets our firemen know where we're heading (they'll figure out their own targets)
        for (int i = 0; i < _fireMen.Count; i++)
            _fireMen[i].CommandTarget = CommandTarget;
    }

    public void ClearTarget()
    {
        // Remove our firemen targets first
        for (int i = 0; i < _fireMen.Count; i++)
            _fireMen[i].CommandTarget = null;

        // Then our truck
        FireTruck.CommandTarget = null;

        // Now our own
        CommandTarget = null;
    }

    public override void LoadTruck()
    {
        base.LoadTruck();

        // Once we're in the truck, we're set to go (truck target is command + truck name)
        FireTruck.CommandTarget = CommandTarget;
    }
}
