USE [AppQuanLyMoi]
GO
/****** Object:  Table [dbo].[Accounts]    Script Date: 3/10/2025 6:44:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Accounts](
	[Email] [nvarchar](255) NOT NULL,
	[CustomerCount] [int] NULL,
	[StartDate] [datetime] NULL,
	[ExpireDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Customers]    Script Date: 3/10/2025 6:44:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customers](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[FacebookLink] [nvarchar](255) NULL,
	[RegistrationDate] [datetime] NULL,
	[ExpirationDate] [datetime] NULL,
	[Package] [nvarchar](50) NULL,
	[AccountEmail] [nvarchar](255) NULL,
	[ContinueSubscription] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[Accounts] ([Email], [CustomerCount], [StartDate], [ExpireDate]) VALUES (N'cc@gmail.com', 0, NULL, NULL)
INSERT [dbo].[Accounts] ([Email], [CustomerCount], [StartDate], [ExpireDate]) VALUES (N'gpt4-77779@thangtinstore.com', 3, CAST(N'2025-01-24T00:00:00.000' AS DateTime), CAST(N'2025-02-24T00:00:00.000' AS DateTime))
INSERT [dbo].[Accounts] ([Email], [CustomerCount], [StartDate], [ExpireDate]) VALUES (N'gpt4-8379@thangtinstore.com', 18, CAST(N'2025-03-03T00:00:00.000' AS DateTime), CAST(N'2025-04-03T00:00:00.000' AS DateTime))
INSERT [dbo].[Accounts] ([Email], [CustomerCount], [StartDate], [ExpireDate]) VALUES (N'gpt4-9379@thangtinstore.com', 12, CAST(N'2025-02-26T00:00:00.000' AS DateTime), CAST(N'2025-03-26T00:00:00.000' AS DateTime))
INSERT [dbo].[Accounts] ([Email], [CustomerCount], [StartDate], [ExpireDate]) VALUES (N'gpt4dc-2236@thangtinstore.com', 12, CAST(N'2025-02-15T00:00:00.000' AS DateTime), CAST(N'2025-03-15T00:00:00.000' AS DateTime))
INSERT [dbo].[Accounts] ([Email], [CustomerCount], [StartDate], [ExpireDate]) VALUES (N'gpt4dc-3668@thangtinstore.com', 10, CAST(N'2025-02-19T00:00:00.000' AS DateTime), CAST(N'2025-03-19T00:00:00.000' AS DateTime))
INSERT [dbo].[Accounts] ([Email], [CustomerCount], [StartDate], [ExpireDate]) VALUES (N'gpt4dc-4039@thangtinstore.com', 11, CAST(N'2025-02-10T00:00:00.000' AS DateTime), CAST(N'2025-03-10T00:00:00.000' AS DateTime))
INSERT [dbo].[Accounts] ([Email], [CustomerCount], [StartDate], [ExpireDate]) VALUES (N'gpt4dc-6679@thangtinstore.com', 4, CAST(N'2025-03-10T00:00:00.000' AS DateTime), CAST(N'2025-04-10T00:00:00.000' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[Customers] ON 

INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (1, N'Nguyễn Hiếu', CAST(N'2025-02-07T00:00:00.000' AS DateTime), CAST(N'2025-03-07T00:00:00.000' AS DateTime), N'goi1', N'gpt4-9379@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (2, N'Duy Tân', CAST(N'2025-02-10T00:00:00.000' AS DateTime), CAST(N'2025-03-10T00:00:00.000' AS DateTime), N'goi1', N'gpt4-9379@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (3, N'Thảo', CAST(N'2025-02-11T00:00:00.000' AS DateTime), CAST(N'2025-03-11T00:00:00.000' AS DateTime), N'goi1', N'gpt4dc-4039@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (4, N'Nguyên An', CAST(N'2025-01-23T00:00:00.000' AS DateTime), CAST(N'2026-01-23T00:00:00.000' AS DateTime), N'goi12', N'gpt4-9379@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (5, N'Phạm Hồng Đăng', CAST(N'2025-02-05T00:00:00.000' AS DateTime), CAST(N'2025-03-05T00:00:00.000' AS DateTime), N'goi1', N'gpt4dc-3668@thangtinstore.com', 0)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (7, N'Trần Đăng Phước', CAST(N'2025-02-07T00:00:00.000' AS DateTime), CAST(N'2025-03-07T00:00:00.000' AS DateTime), N'goi1', N'gpt4dc-3668@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (8, N'Hoàng Đàm', CAST(N'2025-03-10T00:00:00.000' AS DateTime), CAST(N'2026-03-10T00:00:00.000' AS DateTime), N'goi12', N'gpt4dc-3668@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (9, N'Thành Hiếu', CAST(N'2025-02-12T00:00:00.000' AS DateTime), CAST(N'2025-03-12T00:00:00.000' AS DateTime), N'goi1', N'gpt4dc-2236@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (10, N'Lê Nguyễn Xuân Ngân', CAST(N'2025-02-12T00:00:00.000' AS DateTime), CAST(N'2025-03-12T00:00:00.000' AS DateTime), N'goi1', N'gpt4-9379@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (11, N'Nguyễn Hưng', CAST(N'2025-02-11T00:00:00.000' AS DateTime), CAST(N'2025-03-11T00:00:00.000' AS DateTime), N'goi1', N'gpt4dc-4039@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (12, N'Phúc', CAST(N'2025-02-18T00:00:00.000' AS DateTime), CAST(N'2025-03-18T00:00:00.000' AS DateTime), N'goi1', N'gpt4dc-4039@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (13, N'Nguyễn Viết Chính', CAST(N'2025-02-13T00:00:00.000' AS DateTime), CAST(N'2025-03-13T00:00:00.000' AS DateTime), N'goi1', N'gpt4dc-4039@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (14, N'Thanh Trung', CAST(N'2025-03-05T00:00:00.000' AS DateTime), CAST(N'2025-04-05T00:00:00.000' AS DateTime), N'goi1', N'gpt4-8379@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (15, N'Mai Hoàng Lâm', CAST(N'2025-01-24T00:00:00.000' AS DateTime), CAST(N'2025-02-24T00:00:00.000' AS DateTime), N'goi1', N'gpt4dc-4039@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (16, N'Quân Nguyễn', CAST(N'2025-03-10T00:00:00.000' AS DateTime), CAST(N'2025-04-10T00:00:00.000' AS DateTime), N'goi1', N'gpt4-8379@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (17, N'Nhut', CAST(N'2025-02-13T00:00:00.000' AS DateTime), CAST(N'2025-03-13T00:00:00.000' AS DateTime), N'goi1', N'gpt4dc-4039@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (18, N'Nguyễn Văn Đạt', CAST(N'2025-03-10T00:00:00.000' AS DateTime), CAST(N'2025-04-10T00:00:00.000' AS DateTime), N'goi1', N'gpt4-8379@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (19, N'NT Tịnh', CAST(N'2025-02-10T00:00:00.000' AS DateTime), CAST(N'2025-03-10T00:00:00.000' AS DateTime), N'goi1', N'gpt4dc-4039@thangtinstore.com', 0)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (20, N'Đỗ Quỳnh Nga', CAST(N'2025-03-10T00:00:00.000' AS DateTime), CAST(N'2025-04-10T00:00:00.000' AS DateTime), N'goi1', N'gpt4-8379@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (21, N'Lê Nhựt Tân', CAST(N'2025-02-05T00:00:00.000' AS DateTime), CAST(N'2025-03-05T00:00:00.000' AS DateTime), N'goi1', N'gpt4dc-2236@thangtinstore.com', 0)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (22, N'Trương Công Tấn Sang', CAST(N'2025-02-14T00:00:00.000' AS DateTime), CAST(N'2025-03-14T00:00:00.000' AS DateTime), N'goi1', N'gpt4-8379@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (23, N'Bùi Gia Vĩ', CAST(N'2025-02-03T00:00:00.000' AS DateTime), CAST(N'2025-03-03T00:00:00.000' AS DateTime), N'goi1', N'gpt4-9379@thangtinstore.com', 0)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (24, N'Nhật Huỳnh', CAST(N'2025-02-18T00:00:00.000' AS DateTime), CAST(N'2025-03-18T00:00:00.000' AS DateTime), N'goi1', N'gpt4dc-3668@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (25, N'Thúy Vy', CAST(N'2025-03-04T00:00:00.000' AS DateTime), CAST(N'2025-04-04T00:00:00.000' AS DateTime), N'goi1', N'gpt4-8379@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (26, N'Nguyễn Hoàng Phúc', CAST(N'2025-01-15T00:00:00.000' AS DateTime), CAST(N'2025-02-15T00:00:00.000' AS DateTime), N'goi1', N'', 0)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (27, N'Bùi Tuấn Kiệt', CAST(N'2025-02-10T00:00:00.000' AS DateTime), CAST(N'2025-05-10T00:00:00.000' AS DateTime), N'goi3', N'gpt4-9379@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (28, N'Nguyễn Tấn Minh', CAST(N'2025-03-09T00:00:00.000' AS DateTime), CAST(N'2025-04-09T00:00:00.000' AS DateTime), N'goi1', N'gpt4-8379@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (29, N'DucAnh Here', CAST(N'2025-02-10T00:00:00.000' AS DateTime), CAST(N'2026-02-10T00:00:00.000' AS DateTime), N'goi12', N'gpt4dc-4039@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (30, N'Chí Bảo', CAST(N'2025-02-10T00:00:00.000' AS DateTime), CAST(N'2025-03-10T00:00:00.000' AS DateTime), N'goi1', N'gpt4dc-4039@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (31, N'Thu Huyen Pham', CAST(N'2025-02-10T00:00:00.000' AS DateTime), CAST(N'2025-03-10T00:00:00.000' AS DateTime), N'goi1', N'gpt4dc-2236@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (32, N'Nguyễn Khang', CAST(N'2025-02-11T00:00:00.000' AS DateTime), CAST(N'2025-03-11T00:00:00.000' AS DateTime), N'goi1', N'gpt4dc-4039@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (33, N'Thế Đặng', CAST(N'2025-01-19T00:00:00.000' AS DateTime), CAST(N'2025-02-19T00:00:00.000' AS DateTime), N'goi1', N'', 0)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (34, N'Huy Hoàng', CAST(N'2025-02-12T00:00:00.000' AS DateTime), CAST(N'2025-03-12T00:00:00.000' AS DateTime), N'goi1', N'gpt4dc-3668@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (35, N'Trần Đức Nghĩa', CAST(N'2025-01-21T00:00:00.000' AS DateTime), CAST(N'2025-02-21T00:00:00.000' AS DateTime), N'goi1', N'', 0)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (36, N'Thiện Hưng', CAST(N'2025-02-06T00:00:00.000' AS DateTime), CAST(N'2025-03-06T00:00:00.000' AS DateTime), N'goi1', N'gpt4dc-2236@thangtinstore.com', 0)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (37, N'Thiện Hưng', CAST(N'2025-02-04T00:00:00.000' AS DateTime), CAST(N'2025-03-04T00:00:00.000' AS DateTime), N'goi1', N'gpt4-77779@thangtinstore.com', 0)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (38, N'Trần Kim Lộc', CAST(N'2025-02-18T00:00:00.000' AS DateTime), CAST(N'2025-03-18T00:00:00.000' AS DateTime), N'goi1', N'gpt4dc-2236@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (39, N'Vũ', CAST(N'2025-02-17T00:00:00.000' AS DateTime), CAST(N'2025-03-17T00:00:00.000' AS DateTime), N'goi1', N'gpt4-8379@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (40, N'Phương Na', CAST(N'2025-02-17T00:00:00.000' AS DateTime), CAST(N'2025-03-17T00:00:00.000' AS DateTime), N'goi1', N'gpt4dc-2236@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (41, N'Duc Huy', CAST(N'2025-02-17T00:00:00.000' AS DateTime), CAST(N'2025-03-17T00:00:00.000' AS DateTime), N'goi1', N'gpt4dc-2236@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (42, N'Phúc Nguyễn', CAST(N'2025-02-12T00:00:00.000' AS DateTime), CAST(N'2025-03-12T00:00:00.000' AS DateTime), N'goi1', N'gpt4dc-4039@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (43, N'Minh Tuấn', CAST(N'2025-02-19T00:00:00.000' AS DateTime), CAST(N'2025-03-19T00:00:00.000' AS DateTime), N'goi1', N'gpt4dc-2236@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (44, N'Minh Khánh', CAST(N'2025-02-18T00:00:00.000' AS DateTime), CAST(N'2025-03-18T00:00:00.000' AS DateTime), N'goi1', N'gpt4dc-2236@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (45, N'Hoang Ngan', CAST(N'2025-02-18T00:00:00.000' AS DateTime), CAST(N'2025-03-18T00:00:00.000' AS DateTime), N'goi1', N'gpt4dc-2236@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (46, N'Mai Thanh Tân', CAST(N'2025-02-18T00:00:00.000' AS DateTime), CAST(N'2025-03-18T00:00:00.000' AS DateTime), N'goi1', N'gpt4dc-2236@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (47, N'Huỳnh Trần Phước Sang', CAST(N'2025-02-19T00:00:00.000' AS DateTime), CAST(N'2025-05-19T00:00:00.000' AS DateTime), N'goi3', N'gpt4dc-3668@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (48, N'Mai Hoàng', CAST(N'2025-02-24T00:00:00.000' AS DateTime), CAST(N'2025-03-24T00:00:00.000' AS DateTime), N'goi1', N'gpt4dc-3668@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (49, N'Vương Thùy Trang', CAST(N'2025-02-23T00:00:00.000' AS DateTime), CAST(N'2025-05-23T00:00:00.000' AS DateTime), N'goi3', N'gpt4dc-3668@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (50, N'Xuân Mai', CAST(N'2025-02-20T00:00:00.000' AS DateTime), CAST(N'2025-03-20T00:00:00.000' AS DateTime), N'goi1', N'gpt4dc-3668@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (51, N'Nguyễn Duy', CAST(N'2025-02-24T00:00:00.000' AS DateTime), CAST(N'2025-03-24T00:00:00.000' AS DateTime), N'goi1', N'gpt4dc-3668@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (52, N'Phúc', CAST(N'2025-02-17T00:00:00.000' AS DateTime), CAST(N'2025-03-17T00:00:00.000' AS DateTime), N'goi1', N'gpt4dc-2236@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (53, N'Khương Trần', CAST(N'2025-03-02T00:00:00.000' AS DateTime), CAST(N'2025-04-02T00:00:00.000' AS DateTime), N'goi1', N'gpt4-9379@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (54, N'Thùy Linh', CAST(N'2025-03-03T00:00:00.000' AS DateTime), CAST(N'2025-06-03T00:00:00.000' AS DateTime), N'goi3', N'gpt4-9379@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (55, N'Lê Hoàng', CAST(N'2025-02-26T00:00:00.000' AS DateTime), CAST(N'2025-03-26T00:00:00.000' AS DateTime), N'goi1', N'gpt4-9379@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (56, N'Lưu Phương Như', CAST(N'2025-03-03T00:00:00.000' AS DateTime), CAST(N'2025-04-03T00:00:00.000' AS DateTime), N'goi1', N'gpt4-9379@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (57, N'Bao Thi', CAST(N'2025-02-28T00:00:00.000' AS DateTime), CAST(N'2025-08-28T00:00:00.000' AS DateTime), N'goi6', N'gpt4-9379@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (58, N'Thanh Tú Phạm', CAST(N'2025-02-28T00:00:00.000' AS DateTime), CAST(N'2025-03-28T00:00:00.000' AS DateTime), N'goi1', N'gpt4-9379@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (59, N'Linh Chi', CAST(N'2025-03-04T00:00:00.000' AS DateTime), CAST(N'2025-06-04T00:00:00.000' AS DateTime), N'goi3', N'gpt4dc-6679@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (61, N'Huỳnh Thị Minh Phụng', CAST(N'2025-03-06T00:00:00.000' AS DateTime), CAST(N'2025-04-06T00:00:00.000' AS DateTime), N'goi1', N'gpt4-8379@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (62, N'Huỳnh Minh Trí', CAST(N'2025-03-05T00:00:00.000' AS DateTime), CAST(N'2025-04-05T00:00:00.000' AS DateTime), N'goi1', N'gpt4-8379@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (63, N'Phạm Anh Khôi', CAST(N'2025-03-08T00:00:00.000' AS DateTime), CAST(N'2025-06-08T00:00:00.000' AS DateTime), N'goi3', N'gpt4dc-6679@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (64, N'Trần Đức Nghĩa', CAST(N'2025-03-07T00:00:00.000' AS DateTime), CAST(N'2025-06-07T00:00:00.000' AS DateTime), N'goi3', N'gpt4dc-6679@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (65, N'Nguyễn Ngọc Yến Mai', CAST(N'2025-03-09T00:00:00.000' AS DateTime), CAST(N'2025-04-09T00:00:00.000' AS DateTime), N'goi1', N'gpt4-8379@thangtinstore.com', 1)
INSERT [dbo].[Customers] ([ID], [FacebookLink], [RegistrationDate], [ExpirationDate], [Package], [AccountEmail], [ContinueSubscription]) VALUES (66, N'Truong Thanh', CAST(N'2025-03-10T00:00:00.000' AS DateTime), CAST(N'2025-04-10T00:00:00.000' AS DateTime), N'goi1', N'gpt4dc-6679@thangtinstore.com', 1)
SET IDENTITY_INSERT [dbo].[Customers] OFF
GO
ALTER TABLE [dbo].[Customers] ADD  DEFAULT ((1)) FOR [ContinueSubscription]
GO
