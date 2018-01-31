<Query Kind="Program">
  <Reference Relative="..\money.web\bin\money.common.dll">C:\Src\money\money.web\bin\money.common.dll</Reference>
  <Reference Relative="..\money.web\bin\money.web.dll">C:\Src\money\money.web\bin\money.web.dll</Reference>
  <NuGetReference>Microsoft.AspNet.Mvc</NuGetReference>
  <Namespace>Ext = money.web.Support.Extensions</Namespace>
  <Namespace>money.common</Namespace>
  <Namespace>money.web.Models.Entities</Namespace>
</Query>

void Main()
{
    IEnumerable<Account> accountList() => new List<Account> {  
        new Account(100, 1, "Current", AccountType.Current, 100.00M, true, true, 100),
        new Account(200, 1, "Savings", AccountType.Savings, 500.00M, false, true, 200)
    };
    
    Ext.TypesSelectListItems(EntryType.Credit, accountList).Dump();
    Ext.TypesSelectListItems(EntryType.Debit, accountList).Dump();
    Ext.TypesSelectListItems(EntryType.Transfer, accountList).Dump();
    
    Ext.TypesSelectListItems(EntryType.Credit | EntryType.Debit, accountList).Dump();
    Ext.TypesSelectListItems(EntryType.Credit | EntryType.Transfer, accountList).Dump();
    Ext.TypesSelectListItems(EntryType.Debit | EntryType.Transfer, accountList).Dump();
    
    Ext.TypesSelectListItems(EntryType.Credit | EntryType.Debit | EntryType.Transfer, accountList).Dump();
}

