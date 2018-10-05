
insert into [dbo].[Modules] (Descripcion,Enable) values ('Configuración',1);
insert into [dbo].[Modules] (Descripcion,Enable) values ('ABM Maestros',1);

insert into [dbo].[Windows] (Descripcion,Enable,Url,Orden,ModuleId) values ('Roles',1,'Rols',1,(select Id from [dbo].[Modules] where Descripcion='Configuración'))
insert into [dbo].[Windows] (Descripcion,Enable,Url,Orden,ModuleId) values ('Usuarios',1,'Usuarios',2,(select Id from [dbo].[Modules] where Descripcion='Configuración'))
insert into [dbo].[Windows] (Descripcion,Enable,Url,Orden,ModuleId) values ('Permisos',1,'Permissions',3,(select Id from [dbo].[Modules] where Descripcion='Configuración'))
insert into [dbo].[Windows] (Descripcion,Enable,Url,Orden,ModuleId) values ('Auditoria',1,'Audits',4,(select Id from [dbo].[Modules] where Descripcion='Configuración'))


insert into [dbo].[Windows] (Descripcion,Enable,Url,Orden,ModuleId) values ('Tipos de Licencia',1,'TypesLicenses',1,(select Id from [dbo].[Modules] where Descripcion='ABM Maestros'))
insert into [dbo].[Windows] (Descripcion,Enable,Url,Orden,ModuleId) values ('Sectores',1,'Sectors',1,(select Id from [dbo].[Modules] where Descripcion='ABM Maestros'))



