Print 'Checking to see if the database already exists'
if db_id('Coding_Test_001') is null
BEGIN
	Print 'Database does not exist, so creating it'
	create database Coding_Test_001
	Print 'After creating the database'
END
ELSE
BEGIN
	Print 'Database already exists'
END

