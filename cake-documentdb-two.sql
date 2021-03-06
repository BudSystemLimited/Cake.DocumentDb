USE [cake-documentdb-two]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OtherRecords]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[OtherRecords](
		[Id] [int] NOT NULL,
		[Title] [varchar](50) NOT NULL,
		[LocalTitle] [varchar](50) NOT NULL,
		[RandomTitle] [varchar](50) NOT NULL
	) ON [PRIMARY];


	INSERT [dbo].[OtherRecords] ([Id], [Title], [LocalTitle], [RandomTitle]) VALUES (1, N'Title1', N'Local Title', N'foo')

	INSERT [dbo].[OtherRecords] ([Id], [Title], [LocalTitle], [RandomTitle]) VALUES (2, N'Title 2', N'Another local Title', N'bar')

END
GO