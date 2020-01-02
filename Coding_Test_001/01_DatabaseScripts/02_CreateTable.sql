USE [Coding_Test_001]
GO

Print 'Checking to see if the table already exists'
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='SimpleTasks' and xtype='U')
BEGIN
	Print 'Table does not exist, so creating it'

	-- Description is showing in SSMS as a keyword, but not shown as a reserved word here: https://docs.microsoft.com/en-us/sql/t-sql/language-elements/reserved-keywords-transact-sql?view=sql-server-ver15
	-- Going to use [Description] to avoid any issues

	CREATE TABLE [dbo].[SimpleTasks](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[Description] [nvarchar](400) NOT NULL,  -- coding test didn't say the size of the task description, but using a composite primary key of 450 gives "The maximum key length for a clustered index is 900 bytes" so going to use a vaule of 400
		[IsComplete] [bit] NOT NULL,
		CONSTRAINT PK_SimpleTasks PRIMARY KEY ([Description], IsComplete)
	) ON [PRIMARY]

	Print 'After creating the table'
END
ELSE
BEGIN
	Print 'Table already exists'
END
GO