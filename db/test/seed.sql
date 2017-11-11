USE $(DatabaseName)

GO

INSERT INTO Users 
    (Email, Password) 
VALUES 
    ('test@test.com', 'AEZZ5aNVqw55mqQxpbovL3yQIcQMOkIe+6ACUFZUCjh/jE49I4uQ1MMGjVercSayZQ==')

INSERT INTO Accounts 
    (UserID, Name, Type, IsMainAccount, IsIncludedInNetWorth, StartingBalance, CurrentBalance, DisplayOrder) 
VALUES 
    (1, 'TEST ACCOUNT', 0, 1, 1, 100, 100, 100)

INSERT INTO Categories 
    (AccountID, Name, CategoryType, ExpenseType) 
VALUES 
    (1, 'Salary', 1, NULL),
    (1, 'Bills', 0, 0)

INSERT INTO Parties 
    (AccountID, Name) 
VALUES 
    (1, 'MegaCorp Inc'),
    (1, 'Energy Suppliers Ltd')

INSERT INTO MonthlyBudgets 
    (AccountID, StartDate, EndDate) 
VALUES 
    (1, '2017-01-01', '2017-01-31')

INSERT INTO Categories_MonthlyBudgets 
    (MonthlyBudgetID, CategoryID, Amount) 
VALUES 
    (1, 1, 1000),
    (1, 2, 500)

GO
