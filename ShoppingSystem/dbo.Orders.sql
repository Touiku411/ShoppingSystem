USE [C:\USERS\TENGY\SOURCE\REPOS\SHOPPINGSYSTEM\SHOPPINGSYSTEM\DATABASE.MDF]
GO

/****** 物件: Table [dbo].[Orders] 指令碼日期: 2025/6/18 下午 03:07:18 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Orders] (
    [Id]         INT        IDENTITY (1, 1) NOT NULL,
    [OrderDate]  DATETIME   NOT NULL,
    [TotalPrice] INT        NOT NULL,
    [UserName]   NCHAR (50) NOT NULL
);


