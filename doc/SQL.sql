USE [Barcode]
GO
/****** Object:  Table [dbo].[T_LOG_GR_STOCK]    Script Date: 22/11/2023 16:28:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[T_LOG_GR_STOCK](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DocMat] [nvarchar](50) NULL,
	[Batch] [nvarchar](50) NULL,
	[EntryQnt] [int] NULL,
	[EntryUom] [nvarchar](50) NULL,
	[FacNo] [nvarchar](50) NULL,
	[Material] [nvarchar](50) NULL,
	[StgeLoc] [nvarchar](50) NULL,
	[MoveType] [nvarchar](50) NULL,
	[Plant] [nvarchar](50) NULL,
	[Custid] [nvarchar](50) NULL,
	[Kanban] [nvarchar](50) NULL,
	[EMessage] [nvarchar](max) NULL,
	[StockDate] [datetime] NULL,
	[UpdDate] [datetime] NULL,
 CONSTRAINT [PK_T_LOG_GR_STOCK] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
