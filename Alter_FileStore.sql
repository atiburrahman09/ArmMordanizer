

/****** Object:  Table [dbo].[FileStore]    Script Date: 5/29/2022 3:15:41 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FileStore]') AND type in (N'U'))
DROP TABLE [dbo].[FileStore]
GO

/****** Object:  Table [dbo].[FileStore]    Script Date: 5/29/2022 3:15:41 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[FileStore](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[FileName] [nvarchar](max) NOT NULL,
	[ExecutionTime] [datetime] NOT NULL,
	[Status] [bit] NOT NULL,
	[TableName] [nvarchar](max) NULL,
 CONSTRAINT [PK_FileStore] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


