USE [C:\USERS\TENGY\SOURCE\REPOS\SHOPPINGSYSTEM\SHOPPINGSYSTEM\DATABASE.MDF]
GO

/****** 物件: Table [dbo].[OrderItems] 指令碼日期: 2025/6/18 下午 03:06:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[OrderItems] (
    [Id]        INT IDENTITY (1, 1) NOT NULL,
    [OrderId]   INT NOT NULL,
    [ProductId] INT NOT NULL,
    [Quantity]  INT NOT NULL,
    [UnitPrice] INT NOT NULL
);


