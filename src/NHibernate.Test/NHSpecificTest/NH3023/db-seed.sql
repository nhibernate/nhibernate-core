create table DeadlockHelper (
	Id int identity(1, 1),
	CLId int,
	Data uniqueidentifier
)
create clustered index IX_CLId on DeadlockHelper(CLId)
go

set nocount on

insert into DeadlockHelper values (2, newid())

-- Boosting speed by inserting more and more at each loop (from 25s with old code down to 9s on my setup)
while ident_current('DeadlockHelper') <= 5000
	insert into DeadlockHelper select top 1000 2, newid() from DeadlockHelper

delete from DeadlockHelper where Id > 5001

update DeadlockHelper set CLId = 1 where Id <= 10
update DeadlockHelper set CLId = 3 where Id > 4900