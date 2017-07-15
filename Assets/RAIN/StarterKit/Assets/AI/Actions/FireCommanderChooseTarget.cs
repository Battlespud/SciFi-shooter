using UnityEngine;
using RAIN.Action;
using System.Collections.Generic;
using RAIN.Core;

[RAINAction]
public class FireCommanderChooseTarget : RAINAction
{
    private FireCommanderElement _fireCommander = null;

    private List<GameObject> _targets = new List<GameObject>();
    private int _lastTarget = -1;

    public override void Start(RAIN.Core.AI ai)
    {
        _fireCommander = ai.GetCustomElement<FireCommanderElement>();

        if (_targets.Count == 0)
        {
            _targets.Add(GameObject.Find("situationone"));
            _targets.Add(GameObject.Find("situationtwo"));
            _targets.Add(GameObject.Find("situationthree"));
        }

        int tNextTarget = Random.Range(0, 3);
        if (tNextTarget == _lastTarget)
            tNextTarget = (tNextTarget + 1) % _targets.Count;

        _lastTarget = tNextTarget;

        _fireCommander.CommandTarget = _targets[tNextTarget];
    }

    public override ActionResult Execute(AI ai)
    {
        if (_fireCommander.CommandTarget == null)
            return ActionResult.FAILURE;

        return ActionResult.SUCCESS;
    }
}