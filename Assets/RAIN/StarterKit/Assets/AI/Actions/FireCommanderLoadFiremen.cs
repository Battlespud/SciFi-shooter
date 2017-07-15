using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Action;
using RAIN.Core;

[RAINAction]
public class FireCommanderLoadFiremen : RAINAction
{
    private FireCommanderElement _fireCommander = null;

    public override void Start(RAIN.Core.AI ai)
    {
        _fireCommander = ai.GetCustomElement<FireCommanderElement>();

        _fireCommander.LoadFireMen();
    }

    public override ActionResult Execute(RAIN.Core.AI ai)
    {
        if (_fireCommander.AreFireMenLoaded)
            return ActionResult.SUCCESS;

        return ActionResult.RUNNING;
    }
}