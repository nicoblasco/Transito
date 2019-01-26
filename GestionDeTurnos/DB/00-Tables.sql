USE [Transito]
GO

/****** Object:  Table [dbo].[__MigrationHistory]    Script Date: 26/01/2019 15:27:22 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[__MigrationHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ContextKey] [nvarchar](300) NOT NULL,
	[Model] [varbinary](max) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK_dbo.__MigrationHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC,
	[ContextKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Audits]    Script Date: 26/01/2019 15:27:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Audits](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UsuarioId] [int] NOT NULL,
	[WindowId] [int] NOT NULL,
	[Accion] [nvarchar](max) NULL,
	[Fecha] [datetime] NOT NULL,
	[Clave] [nvarchar](max) NULL,
	[Entidad] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Audits] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[CallCenterTurns]    Script Date: 26/01/2019 15:27:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CallCenterTurns](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DNI] [nvarchar](max) NULL,
	[Nombre] [nvarchar](max) NULL,
	[Apellido] [nvarchar](max) NULL,
	[TipoTramite] [nvarchar](max) NULL,
	[FechaTurno] [datetime] NOT NULL,
	[Fecha] [datetime] NOT NULL,
	[Asignado] [bit] NOT NULL,
	[Gestion] [nvarchar](max) NULL,
	[Tel_Particular] [nvarchar](max) NULL,
	[Tel_Celular] [nvarchar](max) NULL,
	[Estado] [nvarchar](max) NULL,
	[Barrio] [nvarchar](max) NULL,
	[Vencimiento_licencia] [nvarchar](max) NULL,
	[FechaModificacion] [datetime] NULL,
	[UsuarioId] [int] NULL,
 CONSTRAINT [PK_dbo.CallCenterTurns] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Countries]    Script Date: 26/01/2019 15:27:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Countries](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Descripcion] [nvarchar](max) NULL,
	[Predeterminado] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.Countries] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[LicenseClasses]    Script Date: 26/01/2019 15:27:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[LicenseClasses](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Codigo] [nvarchar](max) NULL,
	[Descripcion] [nvarchar](max) NULL,
	[Enable] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.LicenseClasses] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[LicenseLicenseClasses]    Script Date: 26/01/2019 15:27:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[LicenseLicenseClasses](
	[LicenseClass_Id] [int] NOT NULL,
	[License_Id] [int] NOT NULL,
 CONSTRAINT [PK_dbo.LicenseLicenseClasses] PRIMARY KEY CLUSTERED 
(
	[License_Id] ASC,
	[LicenseClass_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Licenses]    Script Date: 26/01/2019 15:27:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Licenses](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PersonId] [int] NOT NULL,
	[FechaOtorgamiento] [datetime] NULL,
	[FechaVencimiento] [datetime] NULL,
	[TypesLicenseId] [int] NULL,
	[Estado] [nvarchar](max) NULL,
	[FechaRecibo] [datetime] NULL,
	[FechaRetiro] [datetime] NULL,
	[Firma] [nvarchar](max) NULL,
	[TurnId] [int] NULL,
 CONSTRAINT [PK_dbo.Licenses] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[MedicalPersons]    Script Date: 26/01/2019 15:27:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[MedicalPersons](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PersonId] [int] NOT NULL,
	[TurnId] [int] NOT NULL,
	[Avoi] [int] NOT NULL,
	[Avod] [int] NOT NULL,
	[Fuma] [bit] NOT NULL,
	[Profesional] [bit] NOT NULL,
	[ConduceConAnteojos] [bit] NOT NULL,
	[VisionMonocular] [bit] NOT NULL,
	[Discromatopsia] [bit] NOT NULL,
	[HTA] [bit] NOT NULL,
	[DBT] [bit] NOT NULL,
	[GAA] [bit] NOT NULL,
	[AcidoUrico] [bit] NOT NULL,
	[Colesterol] [bit] NOT NULL,
	[Observacion] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.MedicalPersons] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Modules]    Script Date: 26/01/2019 15:27:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Modules](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Descripcion] [nvarchar](max) NULL,
	[Enable] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.Modules] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[People]    Script Date: 26/01/2019 15:27:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[People](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](max) NULL,
	[Apellido] [nvarchar](max) NULL,
	[Dni] [nvarchar](max) NULL,
	[FechaNacimiento] [datetime] NULL,
	[Tel_Particular] [nvarchar](max) NULL,
	[Tel_Celular] [nvarchar](max) NULL,
	[Barrio] [nvarchar](max) NULL,
	[Vencimiento_licencia] [datetime] NULL,
	[Calle] [nvarchar](max) NULL,
	[StreetId] [int] NULL,
	[Altura] [nvarchar](max) NULL,
	[CountryId] [int] NULL,
	[CalleNro] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.People] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Permissions]    Script Date: 26/01/2019 15:27:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Permissions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WindowId] [int] NOT NULL,
	[RolId] [int] NOT NULL,
	[Consulta] [bit] NOT NULL,
	[AltaModificacion] [bit] NOT NULL,
	[Baja] [bit] NOT NULL,
	[Fecha] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.Permissions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Processes]    Script Date: 26/01/2019 15:27:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Processes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[FechaInicio] [datetime] NOT NULL,
	[FechaFin] [datetime] NULL,
	[Detalle] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Processes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[ProcessLogs]    Script Date: 26/01/2019 15:27:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProcessLogs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Fecha] [datetime] NOT NULL,
	[Name] [nvarchar](max) NULL,
	[IsOk] [bit] NOT NULL,
	[ErrorDescripcion] [nvarchar](max) NULL,
	[ProcessId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.ProcessLogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Rols]    Script Date: 26/01/2019 15:27:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Rols](
	[RolId] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](max) NULL,
	[Descripcion] [nvarchar](max) NULL,
	[IsAdmin] [bit] NOT NULL,
	[WindowId] [int] NULL,
 CONSTRAINT [PK_dbo.Rols] PRIMARY KEY CLUSTERED 
(
	[RolId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Sectors]    Script Date: 26/01/2019 15:27:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Sectors](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Descripcion] [nvarchar](max) NULL,
	[Medico] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.Sectors] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[SectorWorkflows]    Script Date: 26/01/2019 15:27:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SectorWorkflows](
	[Orden] [int] NOT NULL,
	[SectorID] [int] NOT NULL,
	[WorkflowID] [int] NOT NULL,
 CONSTRAINT [PK_dbo.SectorWorkflows] PRIMARY KEY CLUSTERED 
(
	[WorkflowID] ASC,
	[SectorID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Settings]    Script Date: 26/01/2019 15:27:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Settings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Clave] [nvarchar](max) NULL,
	[Texto1] [nvarchar](max) NULL,
	[Texto2] [nvarchar](max) NULL,
	[Numero1] [int] NULL,
	[Numero2] [int] NULL,
	[Logico1] [bit] NULL,
	[Logico2] [bit] NULL,
	[Fecha1] [datetime] NULL,
	[Fecha2] [datetime] NULL,
 CONSTRAINT [PK_dbo.Settings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Status]    Script Date: 26/01/2019 15:27:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Status](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Orden] [int] NOT NULL,
	[Descripcion] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Status] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Streets]    Script Date: 26/01/2019 15:27:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Streets](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Descripcion] [nvarchar](max) NULL,
	[Baja] [nvarchar](max) NULL,
	[DescripcionOficial] [nvarchar](max) NULL,
	[Codigo] [nvarchar](max) NULL,
	[DescripcionGoogle] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Streets] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Terminals]    Script Date: 26/01/2019 15:27:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Terminals](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Descripcion] [nvarchar](max) NULL,
	[IP] [nvarchar](max) NULL,
	[SectorID] [int] NOT NULL,
	[Enable] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.Terminals] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Trackings]    Script Date: 26/01/2019 15:27:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Trackings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TurnID] [int] NOT NULL,
	[TerminalID] [int] NULL,
	[SectorID] [int] NOT NULL,
	[UsuarioID] [int] NULL,
	[FechaIngreso] [datetime] NULL,
	[FechaSalida] [datetime] NULL,
	[Tiempo] [time](7) NULL,
	[FechaCreacion] [datetime] NOT NULL,
	[CantidadDeLlamados] [int] NOT NULL,
	[StatusID] [int] NULL,
	[Alerta] [bit] NOT NULL,
	[Enable] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.Trackings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Turns]    Script Date: 26/01/2019 15:27:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Turns](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Turno] [nvarchar](max) NULL,
	[TypesLicenseID] [int] NOT NULL,
	[PersonID] [int] NOT NULL,
	[FechaIngreso] [datetime] NOT NULL,
	[Secuencia] [int] NOT NULL,
	[FechaTurno] [datetime] NOT NULL,
	[FechaSalida] [datetime] NULL,
	[Tiempo] [time](7) NULL,
	[Enable] [bit] NOT NULL,
	[CallCenterTurnId] [int] NULL,
 CONSTRAINT [PK_dbo.Turns] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[TypesLicenses]    Script Date: 26/01/2019 15:27:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TypesLicenses](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Descripcion] [nvarchar](max) NULL,
	[Codigo] [nvarchar](max) NULL,
	[Referencia] [nvarchar](max) NULL,
	[NumeroInicial] [int] NOT NULL,
 CONSTRAINT [PK_dbo.TypesLicenses] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Usuarios]    Script Date: 26/01/2019 15:27:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Usuarios](
	[UsuarioId] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](max) NULL,
	[Apellido] [nvarchar](max) NULL,
	[Nombreusuario] [nvarchar](max) NULL,
	[Contraseña] [nvarchar](max) NULL,
	[Enable] [bit] NOT NULL,
	[RolId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.Usuarios] PRIMARY KEY CLUSTERED 
(
	[UsuarioId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Windows]    Script Date: 26/01/2019 15:27:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Windows](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ModuleId] [int] NOT NULL,
	[Descripcion] [nvarchar](max) NULL,
	[Enable] [bit] NOT NULL,
	[Url] [nvarchar](max) NULL,
	[Orden] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Windows] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Workflows]    Script Date: 26/01/2019 15:27:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Workflows](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TypesLicenseID] [int] NOT NULL,
 CONSTRAINT [PK_dbo.Workflows] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CallCenterTurns] ADD  DEFAULT ('1900-01-01T00:00:00.000') FOR [FechaTurno]
GO

ALTER TABLE [dbo].[CallCenterTurns] ADD  DEFAULT ('1900-01-01T00:00:00.000') FOR [Fecha]
GO

ALTER TABLE [dbo].[CallCenterTurns] ADD  DEFAULT ((0)) FOR [Asignado]
GO

ALTER TABLE [dbo].[Countries] ADD  DEFAULT ((0)) FOR [Predeterminado]
GO

ALTER TABLE [dbo].[Processes] ADD  DEFAULT ('1900-01-01T00:00:00.000') FOR [FechaInicio]
GO

ALTER TABLE [dbo].[ProcessLogs] ADD  DEFAULT ((0)) FOR [ProcessId]
GO

ALTER TABLE [dbo].[Rols] ADD  DEFAULT ((0)) FOR [IsAdmin]
GO

ALTER TABLE [dbo].[Sectors] ADD  DEFAULT ((0)) FOR [Medico]
GO

ALTER TABLE [dbo].[Trackings] ADD  DEFAULT ('1900-01-01T00:00:00.000') FOR [FechaCreacion]
GO

ALTER TABLE [dbo].[Trackings] ADD  DEFAULT ((0)) FOR [CantidadDeLlamados]
GO

ALTER TABLE [dbo].[Trackings] ADD  DEFAULT ((0)) FOR [Alerta]
GO

ALTER TABLE [dbo].[Trackings] ADD  DEFAULT ((0)) FOR [Enable]
GO

ALTER TABLE [dbo].[Turns] ADD  DEFAULT ((0)) FOR [Secuencia]
GO

ALTER TABLE [dbo].[Turns] ADD  DEFAULT ('1900-01-01T00:00:00.000') FOR [FechaTurno]
GO

ALTER TABLE [dbo].[Turns] ADD  DEFAULT ((0)) FOR [Enable]
GO

ALTER TABLE [dbo].[TypesLicenses] ADD  DEFAULT ((0)) FOR [NumeroInicial]
GO

ALTER TABLE [dbo].[Audits]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Audits_dbo.Usuarios_UsuarioId] FOREIGN KEY([UsuarioId])
REFERENCES [dbo].[Usuarios] ([UsuarioId])
GO

ALTER TABLE [dbo].[Audits] CHECK CONSTRAINT [FK_dbo.Audits_dbo.Usuarios_UsuarioId]
GO

ALTER TABLE [dbo].[Audits]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Audits_dbo.Windows_WindowId] FOREIGN KEY([WindowId])
REFERENCES [dbo].[Windows] ([Id])
GO

ALTER TABLE [dbo].[Audits] CHECK CONSTRAINT [FK_dbo.Audits_dbo.Windows_WindowId]
GO

ALTER TABLE [dbo].[CallCenterTurns]  WITH CHECK ADD  CONSTRAINT [FK_dbo.CallCenterTurns_dbo.Usuarios_UsuarioId] FOREIGN KEY([UsuarioId])
REFERENCES [dbo].[Usuarios] ([UsuarioId])
GO

ALTER TABLE [dbo].[CallCenterTurns] CHECK CONSTRAINT [FK_dbo.CallCenterTurns_dbo.Usuarios_UsuarioId]
GO

ALTER TABLE [dbo].[LicenseLicenseClasses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.LicenseClassLicenses_dbo.LicenseClasses_LicenseClass_Id] FOREIGN KEY([LicenseClass_Id])
REFERENCES [dbo].[LicenseClasses] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[LicenseLicenseClasses] CHECK CONSTRAINT [FK_dbo.LicenseClassLicenses_dbo.LicenseClasses_LicenseClass_Id]
GO

ALTER TABLE [dbo].[LicenseLicenseClasses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.LicenseClassLicenses_dbo.Licenses_License_Id] FOREIGN KEY([License_Id])
REFERENCES [dbo].[Licenses] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[LicenseLicenseClasses] CHECK CONSTRAINT [FK_dbo.LicenseClassLicenses_dbo.Licenses_License_Id]
GO

ALTER TABLE [dbo].[Licenses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Licenses_dbo.People_PersonId] FOREIGN KEY([PersonId])
REFERENCES [dbo].[People] ([Id])
GO

ALTER TABLE [dbo].[Licenses] CHECK CONSTRAINT [FK_dbo.Licenses_dbo.People_PersonId]
GO

ALTER TABLE [dbo].[Licenses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Licenses_dbo.Turns_TurnId] FOREIGN KEY([TurnId])
REFERENCES [dbo].[Turns] ([Id])
GO

ALTER TABLE [dbo].[Licenses] CHECK CONSTRAINT [FK_dbo.Licenses_dbo.Turns_TurnId]
GO

ALTER TABLE [dbo].[Licenses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Licenses_dbo.TypesLicenses_TypesLicenseId] FOREIGN KEY([TypesLicenseId])
REFERENCES [dbo].[TypesLicenses] ([Id])
GO

ALTER TABLE [dbo].[Licenses] CHECK CONSTRAINT [FK_dbo.Licenses_dbo.TypesLicenses_TypesLicenseId]
GO

ALTER TABLE [dbo].[MedicalPersons]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MedicalPersons_dbo.People_PersonId] FOREIGN KEY([PersonId])
REFERENCES [dbo].[People] ([Id])
GO

ALTER TABLE [dbo].[MedicalPersons] CHECK CONSTRAINT [FK_dbo.MedicalPersons_dbo.People_PersonId]
GO

ALTER TABLE [dbo].[MedicalPersons]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MedicalPersons_dbo.Turns_TurnId] FOREIGN KEY([TurnId])
REFERENCES [dbo].[Turns] ([Id])
GO

ALTER TABLE [dbo].[MedicalPersons] CHECK CONSTRAINT [FK_dbo.MedicalPersons_dbo.Turns_TurnId]
GO

ALTER TABLE [dbo].[People]  WITH CHECK ADD  CONSTRAINT [FK_dbo.People_dbo.Countries_CountryId] FOREIGN KEY([CountryId])
REFERENCES [dbo].[Countries] ([Id])
GO

ALTER TABLE [dbo].[People] CHECK CONSTRAINT [FK_dbo.People_dbo.Countries_CountryId]
GO

ALTER TABLE [dbo].[People]  WITH CHECK ADD  CONSTRAINT [FK_dbo.People_dbo.Streets_StreetId] FOREIGN KEY([StreetId])
REFERENCES [dbo].[Streets] ([Id])
GO

ALTER TABLE [dbo].[People] CHECK CONSTRAINT [FK_dbo.People_dbo.Streets_StreetId]
GO

ALTER TABLE [dbo].[Permissions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Permissions_dbo.Rols_RolId] FOREIGN KEY([RolId])
REFERENCES [dbo].[Rols] ([RolId])
GO

ALTER TABLE [dbo].[Permissions] CHECK CONSTRAINT [FK_dbo.Permissions_dbo.Rols_RolId]
GO

ALTER TABLE [dbo].[Permissions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Permissions_dbo.Windows_WindowId] FOREIGN KEY([WindowId])
REFERENCES [dbo].[Windows] ([Id])
GO

ALTER TABLE [dbo].[Permissions] CHECK CONSTRAINT [FK_dbo.Permissions_dbo.Windows_WindowId]
GO

ALTER TABLE [dbo].[ProcessLogs]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ProcessLogs_dbo.Processes_ProcessId] FOREIGN KEY([ProcessId])
REFERENCES [dbo].[Processes] ([Id])
GO

ALTER TABLE [dbo].[ProcessLogs] CHECK CONSTRAINT [FK_dbo.ProcessLogs_dbo.Processes_ProcessId]
GO

ALTER TABLE [dbo].[Rols]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Rols_dbo.Windows_Window_Id] FOREIGN KEY([WindowId])
REFERENCES [dbo].[Windows] ([Id])
GO

ALTER TABLE [dbo].[Rols] CHECK CONSTRAINT [FK_dbo.Rols_dbo.Windows_Window_Id]
GO

ALTER TABLE [dbo].[SectorWorkflows]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SectorWorkflows_dbo.Sectors_Sector_Id] FOREIGN KEY([SectorID])
REFERENCES [dbo].[Sectors] ([Id])
GO

ALTER TABLE [dbo].[SectorWorkflows] CHECK CONSTRAINT [FK_dbo.SectorWorkflows_dbo.Sectors_Sector_Id]
GO

ALTER TABLE [dbo].[SectorWorkflows]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SectorWorkflows_dbo.Workflows_Workflow_Id] FOREIGN KEY([WorkflowID])
REFERENCES [dbo].[Workflows] ([Id])
GO

ALTER TABLE [dbo].[SectorWorkflows] CHECK CONSTRAINT [FK_dbo.SectorWorkflows_dbo.Workflows_Workflow_Id]
GO

ALTER TABLE [dbo].[Terminals]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Terminals_dbo.Sectors_SectorID] FOREIGN KEY([SectorID])
REFERENCES [dbo].[Sectors] ([Id])
GO

ALTER TABLE [dbo].[Terminals] CHECK CONSTRAINT [FK_dbo.Terminals_dbo.Sectors_SectorID]
GO

ALTER TABLE [dbo].[Trackings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Trackings_dbo.Sectors_SectorID] FOREIGN KEY([SectorID])
REFERENCES [dbo].[Sectors] ([Id])
GO

ALTER TABLE [dbo].[Trackings] CHECK CONSTRAINT [FK_dbo.Trackings_dbo.Sectors_SectorID]
GO

ALTER TABLE [dbo].[Trackings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Trackings_dbo.Status_StatusID] FOREIGN KEY([StatusID])
REFERENCES [dbo].[Status] ([Id])
GO

ALTER TABLE [dbo].[Trackings] CHECK CONSTRAINT [FK_dbo.Trackings_dbo.Status_StatusID]
GO

ALTER TABLE [dbo].[Trackings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Trackings_dbo.Terminals_TerminalID] FOREIGN KEY([TerminalID])
REFERENCES [dbo].[Terminals] ([Id])
GO

ALTER TABLE [dbo].[Trackings] CHECK CONSTRAINT [FK_dbo.Trackings_dbo.Terminals_TerminalID]
GO

ALTER TABLE [dbo].[Trackings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Trackings_dbo.Turns_TurnID] FOREIGN KEY([TurnID])
REFERENCES [dbo].[Turns] ([Id])
GO

ALTER TABLE [dbo].[Trackings] CHECK CONSTRAINT [FK_dbo.Trackings_dbo.Turns_TurnID]
GO

ALTER TABLE [dbo].[Trackings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Trackings_dbo.Usuarios_UsuarioID] FOREIGN KEY([UsuarioID])
REFERENCES [dbo].[Usuarios] ([UsuarioId])
GO

ALTER TABLE [dbo].[Trackings] CHECK CONSTRAINT [FK_dbo.Trackings_dbo.Usuarios_UsuarioID]
GO

ALTER TABLE [dbo].[Turns]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Turns_dbo.CallCenterTurns_CallCenterTurnId] FOREIGN KEY([CallCenterTurnId])
REFERENCES [dbo].[CallCenterTurns] ([Id])
GO

ALTER TABLE [dbo].[Turns] CHECK CONSTRAINT [FK_dbo.Turns_dbo.CallCenterTurns_CallCenterTurnId]
GO

ALTER TABLE [dbo].[Turns]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Turns_dbo.People_PersonID] FOREIGN KEY([PersonID])
REFERENCES [dbo].[People] ([Id])
GO

ALTER TABLE [dbo].[Turns] CHECK CONSTRAINT [FK_dbo.Turns_dbo.People_PersonID]
GO

ALTER TABLE [dbo].[Turns]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Turns_dbo.TypesLicenses_TypesLicenseID] FOREIGN KEY([TypesLicenseID])
REFERENCES [dbo].[TypesLicenses] ([Id])
GO

ALTER TABLE [dbo].[Turns] CHECK CONSTRAINT [FK_dbo.Turns_dbo.TypesLicenses_TypesLicenseID]
GO

ALTER TABLE [dbo].[Usuarios]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Usuarios_dbo.Rols_RolId] FOREIGN KEY([RolId])
REFERENCES [dbo].[Rols] ([RolId])
GO

ALTER TABLE [dbo].[Usuarios] CHECK CONSTRAINT [FK_dbo.Usuarios_dbo.Rols_RolId]
GO

ALTER TABLE [dbo].[Windows]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Windows_dbo.Modules_ModuleId] FOREIGN KEY([ModuleId])
REFERENCES [dbo].[Modules] ([Id])
GO

ALTER TABLE [dbo].[Windows] CHECK CONSTRAINT [FK_dbo.Windows_dbo.Modules_ModuleId]
GO

ALTER TABLE [dbo].[Workflows]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Workflows_dbo.TypesLicenses_TypesLicenseID] FOREIGN KEY([TypesLicenseID])
REFERENCES [dbo].[TypesLicenses] ([Id])
GO

ALTER TABLE [dbo].[Workflows] CHECK CONSTRAINT [FK_dbo.Workflows_dbo.TypesLicenses_TypesLicenseID]
GO

