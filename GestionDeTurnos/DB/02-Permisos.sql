
insert into [dbo].[Modules] (Descripcion,Enable) values ('Configuraci�n',1);

insert into [dbo].[Windows] (Descripcion,Enable,Url,Orden,ModuleId) values ('Roles',1,'Rols',1,(select Id from [dbo].[Modules] where Descripcion='Configuraci�n'))
insert into [dbo].[Windows] (Descripcion,Enable,Url,Orden,ModuleId) values ('Usuarios',1,'Usuarios',2,(select Id from [dbo].[Modules] where Descripcion='Configuraci�n'))
insert into [dbo].[Windows] (Descripcion,Enable,Url,Orden,ModuleId) values ('Permisos',1,'Permissions',3,(select Id from [dbo].[Modules] where Descripcion='Configuraci�n'))
insert into [dbo].[Windows] (Descripcion,Enable,Url,Orden,ModuleId) values ('Auditoria',1,'Audits',4,(select Id from [dbo].[Modules] where Descripcion='Configuraci�n'))



