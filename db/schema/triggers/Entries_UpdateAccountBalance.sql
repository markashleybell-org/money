-- Create Entries_UpdateAccountBalance trigger
IF OBJECT_ID(N'[Entries_UpdateAccountBalance]') IS NOT NULL
BEGIN
    DROP TRIGGER [Entries_UpdateAccountBalance]
END

GO

CREATE TRIGGER 
    [Entries_UpdateAccountBalance]
ON 
    [Entries]
FOR 
    INSERT, 
    UPDATE, 
    DELETE
AS
BEGIN
    
    DECLARE @AccountsWithChangedEntries TABLE (ID int)

    INSERT INTO 
        @AccountsWithChangedEntries 
    SELECT 
        [AccountID] 
    FROM 
        [DELETED] 
    UNION ALL 
    SELECT 
        [AccountID] 
    FROM 
        [INSERTED]

    UPDATE 
        [a]
    SET 
        [a].[CurrentBalance] = [a].[StartingBalance] + ISNULL((
            SELECT 
                SUM([e].[Amount]) 
            FROM 
                [Entries] [e] 
            WHERE 
                [e].[AccountID] = [a].[ID]
        ), 0) 
    FROM 
        [Accounts] [a]
    INNER JOIN
        @AccountsWithChangedEntries [ac] ON [ac].[ID] = [a].[ID]
    
END

GO
