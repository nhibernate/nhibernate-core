alter table OrderDetail  drop constraint FK436A79ACF8608B
alter table OrderDetail  drop constraint FK436A79ACF1C0E0E
drop table Product
drop table OrderDetail
drop table `Order`
create table Product (
  Id UNIQUEIDENTIFIER not null,
   Version INT not null,
   Name TEXT(255) not null,
   UnitPrice REAL null,
   UnitsInStock INT null,
   primary key (Id)
)
create table OrderDetail (Id INT !!! REPLACE THE PRECEEDING 'INT' AND THIS PLACEHOLDER WITH 'COUNTER' !!!, ProductName TEXT(255) not null, UnitPrice REAL null, Quantity INT null, `Order` INT not null, Product UNIQUEIDENTIFIER null, primary key (Id))
create table `Order` (Id INT !!! REPLACE THE PRECEEDING 'INT' AND THIS PLACEHOLDER WITH 'COUNTER' !!!, `Date` DATETIME null, Customer TEXT(255) not null, TotalPrice REAL null, primary key (Id))
alter table OrderDetail  add constraint FK436A79ACF8608B foreign key (`Order`) references `Order` 
alter table OrderDetail  add constraint FK436A79ACF1C0E0E foreign key (Product) references Product 
