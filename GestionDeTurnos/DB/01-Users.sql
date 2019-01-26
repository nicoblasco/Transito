insert into Rols (Nombre,Descripcion, IsAdmin) values ('Administrador','Administrador',1);

insert into Usuarios (nombreusuario,enable,RolId,Contraseña) values
('admin',1,(select RolId from Rols where Nombre='Administrador' ),'1234');




