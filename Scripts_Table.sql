
/****** Object:  Table [dbo].[SystemGlobalProperties]    Script Date: 5/9/2022 3:27:03 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SystemGlobalProperties]') AND type in (N'U'))
DROP TABLE [dbo].[SystemGlobalProperties]
GO
/****** Object:  Table [dbo].[MapperConfiguration]    Script Date: 5/9/2022 3:27:03 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MapperConfiguration]') AND type in (N'U'))
DROP TABLE [dbo].[MapperConfiguration]
GO
/****** Object:  Table [dbo].[FileStore]    Script Date: 5/9/2022 3:27:03 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FileStore]') AND type in (N'U'))
DROP TABLE [dbo].[FileStore]
GO
/****** Object:  Table [dbo].[FileStore]    Script Date: 5/9/2022 3:27:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FileStore](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[FileName] [nvarchar](max) NOT NULL,
	[ExecutionTime] [date] NOT NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK_FileStore] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MapperConfiguration]    Script Date: 5/9/2022 3:27:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MapperConfiguration](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SourceTable] [nvarchar](max) NOT NULL,
	[DestinationTable] [nvarchar](max) NOT NULL,
	[SQL] [nvarchar](max) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedDate] [date] NOT NULL,
	[UpdatedDate] [date] NULL,
 CONSTRAINT [PK_MapperConfiguration] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SystemGlobalProperties]    Script Date: 5/9/2022 3:27:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemGlobalProperties](
	[PropertyID] [int] IDENTITY(1,1) NOT NULL,
	[PropertyName] [nvarchar](255) NULL,
	[PropertyValue] [nvarchar](255) NULL,
	[PropertyGroup] [nvarchar](255) NULL,
	[PropertyNote] [nvarchar](255) NULL,
	[PropertyType] [nvarchar](255) NULL,
 CONSTRAINT [PK_SystemGlobalProperties] PRIMARY KEY CLUSTERED 
(
	[PropertyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
