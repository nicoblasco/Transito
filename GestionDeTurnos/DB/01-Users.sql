insert into Rols (Nombre,Descripcion) values ('Administrador','Administrador');

insert into Usuarios (nombreusuario,enable,RolId,Contraseņa) values
('admin',1,(select RolId from Rols where Nombre='Administrador' ),'1234');




