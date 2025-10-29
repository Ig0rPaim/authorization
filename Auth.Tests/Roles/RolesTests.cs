using Auth.Domain;
using Action = Auth.Domain.Action;

namespace Auth.Tests.Roles;

public class RolesTests
{
    [Fact]
    public void Create()
    {
        //arrange
        var page = new Page(Guid.NewGuid(), "test page");
        var auths = new List<Authorization>()
        {
            Authorization.Create(page, Action.See, true)
        };
        var role = Role.Create("test role", "description", auths, true);

        //act
        bool addedNow = role.Added;

        //assert
        Assert.False(addedNow);
    }
    
    [Fact]
    public void CreateToAdd()
    {
        //arrange
        var page = new Page(Guid.NewGuid(), "test page");
        var auths = new List<Authorization>()
        {
            Authorization.Create(page, Action.See, true)
        };
        var role = Role.CreateToAdd("test role", "description", auths, true);

        //act
        bool addedNow = role.Added;

        //assert
        Assert.True(addedNow);
    }

    [Fact]
    public void AnyAuthorizationsAreInRole()
    {
        //arrange
        var page1 = new Page(Guid.NewGuid(), "test page");
        var auth1 = Authorization.CreateToAdd(page1, Action.See, true);
        var page2 = new Page(Guid.NewGuid(), "test page");
        var auth2 = Authorization.CreateToAdd(page2, Action.Create, true);
        var auths = new List<Authorization>()
        {
            auth1, auth2
        };
        
        var role1 = Role.Create("test role 1", "description", new List<Authorization>(), true);
        var role2 = Role.Create("test role 2", "description", new List<Authorization>(){auth1}, true);
        var role3 = Role.Create("test role 3", "description", auths, true);
        
        //act
        var notContainAnyAuthorization = role1.AnyAuthorizationAreInRole(auths);
        var containAnyAuthorization = role2.AnyAuthorizationAreInRole(auths);
        var containsAllAuthorization = role3.AnyAuthorizationAreInRole(auths);

        //assert
        Assert.False(notContainAnyAuthorization.IsValide());
        Assert.True(containAnyAuthorization.IsValide());
        Assert.True(containsAllAuthorization.IsValide());
    }

    [Fact]
    public void RoleWithAllAuths()
    {
        //arrange
        ArrangeRoleThatContainsAuthorization(out Authorization auth1_see,
            out Authorization auth1_edit,
            out Authorization auth1_create,
            out Authorization auth1_delete,
            out Authorization auth2_see,
            out Authorization auth2_edit,
            out Authorization auth2_create,
            out Authorization auth2_delete);
        var auths = new List<Authorization>()
        {
            auth1_see,
            auth1_edit,
            auth1_create,
            auth1_delete,
            auth2_see,
            auth2_edit,
            auth2_create,
            auth2_delete
        };
        var role = Role.Create("test role", "test", auths, true);
        
        //act
        var result = role.AuthorizationsInRole(ref auths);

        //assert
        Assert.True(result.IsValide());
    }
    
    [Fact]
    public void RoleWithAnyAuths()
    {
        //arrange
        ArrangeRoleThatContainsAuthorization(out Authorization auth1_see,
            out Authorization auth1_edit,
            out Authorization auth1_create,
            out Authorization auth1_delete,
            out Authorization auth2_see,
            out Authorization auth2_edit,
            out Authorization auth2_create,
            out Authorization auth2_delete);
        var auths = new List<Authorization>()
        {
            auth1_see,
            auth1_edit,
            auth1_create,
            auth1_delete,
        };
        var authsAll = new List<Authorization>()
        {
            auth2_see,
            auth2_edit,
            auth2_create,
            auth2_delete
        }.Concat(auths).ToList();
        var difference = authsAll.Where(a => !auths.Contains(a)).ToList();
        var role = Role.Create("test role", "test", auths, true);
        
        //act
        var result = role.AuthorizationsInRole(ref authsAll);

        //assert
        Assert.True(result.IsValide());
        Assert.Equal(difference, authsAll);
    }
    
    [Fact]
    public void RoleWithMoreAuths()
    {
        //arrange
        ArrangeRoleThatContainsAuthorization(out Authorization auth1_see,
            out Authorization auth1_edit,
            out Authorization auth1_create,
            out Authorization auth1_delete,
            out Authorization auth2_see,
            out Authorization auth2_edit,
            out Authorization auth2_create,
            out Authorization auth2_delete);
        var auths = new List<Authorization>()
        {
            auth1_see,
            auth1_edit,
            auth1_create,
            auth1_delete,
        };
        var authsAll = new List<Authorization>()
        {
            auth2_see,
            auth2_edit,
            auth2_create,
            auth2_delete
        }.Concat(auths).ToList();
        var role = Role.Create("test role", "test", authsAll, true);
        
        //act
        var result = role.AuthorizationsInRole(ref auths);

        //assert
        Assert.True(result.IsValide());
    }

    #region private methods
    public void ArrangeRoleThatContainsAuthorization(
        out Authorization auth1_see,
        out Authorization auth1_edit,
        out Authorization auth1_create,
        out Authorization auth1_delete,
        out Authorization auth2_see,
        out Authorization auth2_edit,
        out Authorization auth2_create,
        out Authorization auth2_delete)
    {
        //arrange
        var page = new Page(Guid.NewGuid(), "test page");
        var page2 = new Page(Guid.NewGuid(), "test page");
        
        auth1_see = Authorization.Create(page, Action.See, true);
        auth1_edit = Authorization.Create(page, Action.Edit, true);
        auth1_create = Authorization.Create(page, Action.Create, true);
        auth1_delete = Authorization.Create(page, Action.Delete, true);

        auth2_see = Authorization.Create(page2, Action.See, true);
        auth2_edit = Authorization.Create(page2, Action.Edit, true);
        auth2_create = Authorization.Create(page2, Action.Create, true);
        auth2_delete = Authorization.Create(page2, Action.Delete, true);
        
        var allAuth = new List<Authorization>()
        {
            auth1_see,
            auth1_edit,
            auth1_create,
            auth1_delete,
            
            auth2_see,
            auth2_edit,
            auth2_create,
            auth2_delete
        };
        
        
        var allAuth1 = new List<Authorization>()
        {
            auth1_see,
            auth1_edit,
            auth1_create,
            auth1_delete,
        };
        
        var authsWithAllAuth1AndAnyAuth2 = allAuth1.Concat(new List<Authorization>() { auth2_see }).ToList();
        
        var authsWithSomeAuth2 = new List<Authorization>() { auth2_delete, auth2_create, auth2_edit }; 
        
        var roleWithAllAuths1 = Role
            .Create("test role", 
                "description", 
                allAuth1, true);
        
        var roleWithMoreAuths = Role
            .Create("test role",
            "description",
            authsWithAllAuth1AndAnyAuth2, 
            true);
        
        var roleWithMinusAuths = Role
            .Create("test role", 
            "description", 
            authsWithSomeAuth2, 
            false);
    }
    #endregion
}