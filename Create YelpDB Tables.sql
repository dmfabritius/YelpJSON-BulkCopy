USE [YelpDB]
GO
/****** Object:  Table [Attributes]    Script Date: 8/11/2020 8:05:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Attributes]
(
	[AttributeID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	CONSTRAINT [PK_Attributes] PRIMARY KEY CLUSTERED 
(
	[AttributeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [BusinessAttributes]    Script Date: 8/11/2020 8:05:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [BusinessAttributes]
(
	[BusinessID] [nvarchar](50) NOT NULL,
	[AttributeID] [int] NOT NULL,
	[Value] [nvarchar](max) NULL,
	CONSTRAINT [PK_BusinessAttributes] PRIMARY KEY CLUSTERED 
(
	[BusinessID] ASC,
	[AttributeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [BusinessCategories]    Script Date: 8/11/2020 8:05:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [BusinessCategories]
(
	[BusinessID] [nvarchar](50) NOT NULL,
	[CategoryID] [int] NOT NULL,
	CONSTRAINT [PK_BusinessCategories] PRIMARY KEY CLUSTERED 
(
	[BusinessID] ASC,
	[CategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [Businesses]    Script Date: 8/11/2020 8:05:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Businesses]
(
	[BusinessID] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Address] [nvarchar](max) NULL,
	[City] [nvarchar](max) NULL,
	[State] [nvarchar](max) NULL,
	[PostalCode] [nvarchar](max) NULL,
	[Latitude] [float] NULL,
	[Longitude] [float] NULL,
	[Stars] [float] NULL,
	[ReviewCount] [int] NULL,
	[IsOpen] [int] NULL,
	CONSTRAINT [PK_Businesses] PRIMARY KEY CLUSTERED 
(
	[BusinessID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [Categories]    Script Date: 8/11/2020 8:05:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Categories]
(
	[CategoryID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED 
(
	[CategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [Checkins]    Script Date: 8/11/2020 8:05:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Checkins]
(
	[BusinessID] [nvarchar](50) NOT NULL,
	[CheckinDate] [datetime] NOT NULL,
	CONSTRAINT [PK_Checkins] PRIMARY KEY CLUSTERED 
(
	[BusinessID] ASC,
	[CheckinDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [Tips]    Script Date: 8/11/2020 8:05:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Tips]
(
	[BusinessID] [nvarchar](50) NOT NULL,
	[UserID] [nvarchar](50) NOT NULL,
	[TipDate] [datetime] NOT NULL,
	[Likes] [int] NOT NULL,
	[Text] [nvarchar](max) NOT NULL,
	CONSTRAINT [PK_Tips] PRIMARY KEY CLUSTERED 
(
	[BusinessID] ASC,
	[UserID] ASC,
	[TipDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [UserFriends]    Script Date: 8/11/2020 8:05:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [UserFriends]
(
	[UserID] [nvarchar](50) NOT NULL,
	[FriendID] [nvarchar](50) NOT NULL,
	CONSTRAINT [PK_UserFriends] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[FriendID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [Users]    Script Date: 8/11/2020 8:05:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Users]
(
	[UserID] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[YelpingSince] [datetime] NULL,
	[AverageStars] [float] NULL,
	[Fans] [int] NULL,
	[Cool] [int] NULL,
	[Funny] [int] NULL,
	[Useful] [int] NULL,
	[TipCount] [int] NULL,
	CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [Tips] ADD  CONSTRAINT [DF_Tips_Likes]  DEFAULT ((0)) FOR [Likes]
GO
ALTER TABLE [BusinessAttributes]  WITH NOCHECK ADD  CONSTRAINT [FK_BusinessAttributes_Attributes] FOREIGN KEY([AttributeID])
REFERENCES [Attributes] ([AttributeID])
GO
ALTER TABLE [BusinessAttributes] CHECK CONSTRAINT [FK_BusinessAttributes_Attributes]
GO
ALTER TABLE [BusinessAttributes]  WITH NOCHECK ADD  CONSTRAINT [FK_BusinessAttributes_Businesses] FOREIGN KEY([BusinessID])
REFERENCES [Businesses] ([BusinessID])
GO
ALTER TABLE [BusinessAttributes] CHECK CONSTRAINT [FK_BusinessAttributes_Businesses]
GO
ALTER TABLE [BusinessCategories]  WITH NOCHECK ADD  CONSTRAINT [FK_BusinessCategories_Businesses] FOREIGN KEY([BusinessID])
REFERENCES [Businesses] ([BusinessID])
GO
ALTER TABLE [BusinessCategories] CHECK CONSTRAINT [FK_BusinessCategories_Businesses]
GO
ALTER TABLE [BusinessCategories]  WITH NOCHECK ADD  CONSTRAINT [FK_BusinessCategories_Categories] FOREIGN KEY([CategoryID])
REFERENCES [Categories] ([CategoryID])
GO
ALTER TABLE [BusinessCategories] CHECK CONSTRAINT [FK_BusinessCategories_Categories]
GO
ALTER TABLE [Checkins]  WITH NOCHECK ADD  CONSTRAINT [FK_Checkins_Businesses] FOREIGN KEY([BusinessID])
REFERENCES [Businesses] ([BusinessID])
GO
ALTER TABLE [Checkins] CHECK CONSTRAINT [FK_Checkins_Businesses]
GO
ALTER TABLE [Tips]  WITH NOCHECK ADD  CONSTRAINT [FK_Tips_Businesses] FOREIGN KEY([BusinessID])
REFERENCES [Businesses] ([BusinessID])
GO
ALTER TABLE [Tips] CHECK CONSTRAINT [FK_Tips_Businesses]
GO
ALTER TABLE [Tips]  WITH NOCHECK ADD  CONSTRAINT [FK_Tips_Users] FOREIGN KEY([UserID])
REFERENCES [Users] ([UserID])
GO
ALTER TABLE [Tips] CHECK CONSTRAINT [FK_Tips_Users]
GO
ALTER TABLE [UserFriends]  WITH NOCHECK ADD  CONSTRAINT [FK_UserFriends_Users] FOREIGN KEY([UserID])
REFERENCES [Users] ([UserID])
GO
ALTER TABLE [UserFriends] CHECK CONSTRAINT [FK_UserFriends_Users]
GO
ALTER TABLE [UserFriends]  WITH NOCHECK ADD  CONSTRAINT [FK_UserFriends_Users1] FOREIGN KEY([FriendID])
REFERENCES [Users] ([UserID])
GO
ALTER TABLE [UserFriends] CHECK CONSTRAINT [FK_UserFriends_Users1]
GO
