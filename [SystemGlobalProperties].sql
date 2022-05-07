
/****** Object:  Table [dbo].[SystemGlobalProperties]    Script Date: 5/7/2022 12:57:13 PM ******/
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
SET IDENTITY_INSERT [dbo].[SystemGlobalProperties] ON 
GO
INSERT [dbo].[SystemGlobalProperties] ([PropertyID], [PropertyName], [PropertyValue], [PropertyGroup], [PropertyNote], [PropertyType]) VALUES (150, N'UploadTimeInterval', N'1000', N'Mordanizer', N'timeInterVal', N'Numeric')
GO
INSERT [dbo].[SystemGlobalProperties] ([PropertyID], [PropertyName], [PropertyValue], [PropertyGroup], [PropertyNote], [PropertyType]) VALUES (151, N'UploadQueue', N'C:\ARMS\arm-wwwroot\Data\Queue\', N'Mordanizer', N'armFilePath', NULL)
GO
INSERT [dbo].[SystemGlobalProperties] ([PropertyID], [PropertyName], [PropertyValue], [PropertyGroup], [PropertyNote], [PropertyType]) VALUES (152, N'UploadCompletePath', N'C:\ARMS\arm-wwwroot\Data\Queue\Uploaded', N'Mordanizer', N'armFileCompletePath', NULL)
GO
INSERT [dbo].[SystemGlobalProperties] ([PropertyID], [PropertyName], [PropertyValue], [PropertyGroup], [PropertyNote], [PropertyType]) VALUES (153, N'UploadLogFile', N'C:\ARMS\arm-wwwroot\Data\Logs\upload_DDMMYY.log', N'Mordanizer', N'logFile', NULL)
GO
SET IDENTITY_INSERT [dbo].[SystemGlobalProperties] OFF
GO
