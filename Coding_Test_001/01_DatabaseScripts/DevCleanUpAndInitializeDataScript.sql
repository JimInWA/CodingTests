USE [Coding_Test_001]
GO

select count(1) from [dbo].[SimpleTasks]
delete from [dbo].[SimpleTasks]

DBCC CHECKIDENT ("[Coding_Test_001].[dbo].[SimpleTasks]", RESEED, 0);

select count(1) from [dbo].[SimpleTasks]
insert into [dbo].[SimpleTasks] values ('a', 0)
insert into [dbo].[SimpleTasks] values ('b', 1)

select * from [dbo].[SimpleTasks]

