
insert into [dbo].[Modules] (Descripcion,Enable) values ('Menu Principal',1);
insert into [dbo].[Modules] (Descripcion,Enable) values ('Licencias',1);
insert into [dbo].[Modules] (Descripcion,Enable) values ('Configuración',1);
insert into [dbo].[Modules] (Descripcion,Enable) values ('ABM Maestros',1);

insert into [dbo].[Windows] (Descripcion,Enable,Url,Orden,ModuleId) values ('Monitoreo',1,'Turns/Index',1,(select Id from [dbo].[Modules] where Descripcion='Menu Principal'))
insert into [dbo].[Windows] (Descripcion,Enable,Url,Orden,ModuleId) values ('Atencion al cliente',1,'Trackings/Index',1,(select Id from [dbo].[Modules] where Descripcion='Menu Principal'))
insert into [dbo].[Windows] (Descripcion,Enable,Url,Orden,ModuleId) values ('Busquedas',1,'Turns/Search',3,(select Id from [dbo].[Modules] where Descripcion='Menu Principal'))
insert into [dbo].[Windows] (Descripcion,Enable,Url,Orden,ModuleId) values ('Call Center',1,'CallCenterTurns/Index',4,(select Id from [dbo].[Modules] where Descripcion='Menu Principal'))
insert into [dbo].[Windows] (Descripcion,Enable,Url,Orden,ModuleId) values ('Historial Medico',1,'MedicalPersons/Index',5,(select Id from [dbo].[Modules] where Descripcion='Menu Principal'))

insert into [dbo].[Windows] (Descripcion,Enable,Url,Orden,ModuleId) values ('Licencias',1,'Licenses/Index',1,(select Id from [dbo].[Modules] where Descripcion='Licencias'))


insert into [dbo].[Windows] (Descripcion,Enable,Url,Orden,ModuleId) values ('Tipos de Licencia',1,'TypesLicenses/Index',1,(select Id from [dbo].[Modules] where Descripcion='ABM Maestros'))
insert into [dbo].[Windows] (Descripcion,Enable,Url,Orden,ModuleId) values ('Sectores',1,'Sectors/Index',2,(select Id from [dbo].[Modules] where Descripcion='ABM Maestros'))
insert into [dbo].[Windows] (Descripcion,Enable,Url,Orden,ModuleId) values ('Terminales',1,'Terminals/Index',3,(select Id from [dbo].[Modules] where Descripcion='ABM Maestros'))
insert into [dbo].[Windows] (Descripcion,Enable,Url,Orden,ModuleId) values ('Clases de Licencia',1,'LicenseClasses/Index',4,(select Id from [dbo].[Modules] where Descripcion='ABM Maestros'))


insert into [dbo].[Windows] (Descripcion,Enable,Url,Orden,ModuleId) values ('Roles',1,'Rols/Index',1,(select Id from [dbo].[Modules] where Descripcion='Configuración'))
insert into [dbo].[Windows] (Descripcion,Enable,Url,Orden,ModuleId) values ('Usuarios',1,'Usuarios/Index',2,(select Id from [dbo].[Modules] where Descripcion='Configuración'))
insert into [dbo].[Windows] (Descripcion,Enable,Url,Orden,ModuleId) values ('Permisos',1,'Permissions/Index',3,(select Id from [dbo].[Modules] where Descripcion='Configuración'))
insert into [dbo].[Windows] (Descripcion,Enable,Url,Orden,ModuleId) values ('Auditoria',1,'Audits/Index',4,(select Id from [dbo].[Modules] where Descripcion='Configuración'))
insert into [dbo].[Windows] (Descripcion,Enable,Url,Orden,ModuleId) values ('Sistema',1,'Settings/Edit',5,(select Id from [dbo].[Modules] where Descripcion='Configuración'))
insert into [dbo].[Windows] (Descripcion,Enable,Url,Orden,ModuleId) values ('Procesos',1,'Processes/Index',6,(select Id from [dbo].[Modules] where Descripcion='Configuración'))

