namespace Auth.Domain;

public class Authorization : Atributte
{
    public Page Page { get; private set; }
    public Action Action { get; private set; }
    public bool Active {get; private set;}

    private Authorization(Page page, Action action, bool active)
    {
        Page = page;
        Action = action;
        Active = active;
    }

    public void Activate() => Active = true;
    public void Deactivate() => Active = false;
    public static Authorization Create(Page page, Action action, bool active) => new Authorization(page, action, active);
    public static Authorization CreateToAdd(Page page, Action action, bool active)
    {
        var auth = new Authorization(page, action, active);
        auth.AddedNow();
        return auth;
    }
    public override bool Equals(object? obj)
    {
        if (obj is Authorization authorization)
        {
            return authorization.Page.Guid == this.Page.Guid &&
                   authorization.Action == this.Action;
        }
        return base.Equals(obj);
    }
}