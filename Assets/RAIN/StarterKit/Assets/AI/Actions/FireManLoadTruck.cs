using RAIN.Action;

[RAINAction]
class FireManLoadTruck : RAINAction
{
    private FireManElement _fireMan = null;

    public override void Start(RAIN.Core.AI ai)
    {
        _fireMan = ai.GetCustomElement<FireManElement>();

        _fireMan.LoadTruck();
    }

    public override ActionResult Execute(RAIN.Core.AI ai)
    {
        if (_fireMan.IsTruckLoaded && _fireMan.FireTruck.Arrived)
        {
            _fireMan.UnloadTruck();

            return ActionResult.SUCCESS;
        }

        return ActionResult.RUNNING;
    }
}
