<Query Kind="Program">
  <Reference Relative="..\money.web\bin\money.common.dll">C:\Src\money\money.web\bin\money.common.dll</Reference>
  <Reference Relative="..\money.web\bin\money.web.dll">C:\Src\money\money.web\bin\money.web.dll</Reference>
  <NuGetReference>Microsoft.AspNet.Mvc</NuGetReference>
  <Namespace>Ext = money.web.Support.Extensions</Namespace>
  <Namespace>money.common</Namespace>
  <Namespace>money.web.Models.Entities</Namespace>
  <Namespace>money.web.Concrete</Namespace>
</Query>

void Main()
{
    var connectionString = "Server=localhost;Database=money;Trusted_Connection=yes";
    
    var uow = new UnitOfWork(connectionString);
    
    var db = new QueryHelper(uow, connectionString);
    
    var plm = new PersistentSessionManager(db);
    
    // plm.CreatePersistentLoginSession(1);
    
    plm.DestroyPersistentLoginSession(1, "8291050770084597166");
    
    uow.CommitChanges();
}