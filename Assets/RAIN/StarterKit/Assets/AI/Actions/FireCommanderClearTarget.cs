using RAIN.Action;

[RAINAction]
class FireManClearTarget : RAINAction
{
    private FireCommanderElement _fireCommander = null;

    public override void Start(RAIN.Core.AI ai)
    {
        _fireCommander = ai.GetCustomElement<FireCommanderElement>();

        _fireCommander.ClearTarget();
    }
}
