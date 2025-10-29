using Auth.Domain;
using Action = Auth.Domain.Action;

namespace Auth.Tests.Authorizations;

public class AuthorizationTests
{
    [Fact]
    public void Create()
    {
        //arrange
        var page = new Page(Guid.NewGuid(), "test page");
        var auth = Authorization.Create(page, Action.See, true);

        //act
        bool added = auth.Added;
        
        //assert
        Assert.False(added);
    }
    
    [Fact]
    public void CreateToAdd()
    {
        //arrange
        var page = new Page(Guid.NewGuid(), "test page");
        var auth = Authorization.CreateToAdd(page, Action.See, true);

        //act
        bool addedNow = auth.Added;
        
        //assert
        Assert.True(addedNow);
    }

    #region equals
    [Fact]
    public void DifferentGuidSameAction()
    {
        //arrange
        var guid = Guid.NewGuid();
        var guid2 = Guid.NewGuid();
        var page1 = new Page(guid, "test page 1");
        var page2 = new Page(guid2, "test page 2");
        var auth1 = Authorization.Create(page1, Action.See, true);
        var auth2 = Authorization.Create(page2, Action.Edit, true);
        
        //act
        var differentGuidSameAction = auth1.Equals(auth2);

        //assert
        Assert.False(differentGuidSameAction);
    }
    
    [Fact]
    public void SameGuidDifferentAction()
    {
        //arrange
        var guid = Guid.NewGuid();
        var page = new Page(guid, "test page 1");
        var page2 = new Page(guid, "test page 2");
        var auth = Authorization.Create(page, Action.See, true);
        var auth2 = Authorization.Create(page2, Action.Edit, true);
        //act
        var sameGuidDifferentAction = auth.Equals(auth2);
        
        //assert
        Assert.False(sameGuidDifferentAction);
    }
    
    [Fact]
    public void SameGuidSameAction()
    {
        //arrange
        var guid = Guid.NewGuid();
        var page = new Page(guid, "test page 1");
        var page2 = new Page(guid, "test page 2");
        var auth = Authorization.Create(page, Action.See, true);
        var auth2 = Authorization.Create(page2, Action.See, true);
        
        //act
        var sameGuidSameAction = auth.Equals(auth2);
        
        //assert
        Assert.True(sameGuidSameAction);
    }
    #endregion
}