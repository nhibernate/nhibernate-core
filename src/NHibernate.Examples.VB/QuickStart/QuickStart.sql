use nhibernate
go

CREATE TABLE users_vb 
(
  LogonID nvarchar(20) NOT NULL,
  Name nvarchar(40) default NULL,
  Password nvarchar(20) default NULL,
  EmailAddress nvarchar(40) default NULL,
  LastLogon datetime default NULL,
  PRIMARY KEY  (LogonID)

)
go


