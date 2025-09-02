USE [C:\USERS\TENGY\SOURCE\REPOS\SHOPPINGSYSTEM\SHOPPINGSYSTEM\DATABASE.MDF]
GO

/****** 物件: Table [dbo].[Products] 指令碼日期: 2025/6/18 下午 03:07:32 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Products] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (100) NULL,
    [Price]     INT            NULL,
    [Category]  NVARCHAR (50)  NULL,
    [ImagePath] NVARCHAR (200) NULL
);


