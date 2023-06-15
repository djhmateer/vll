CREATE TABLE [dbo].[ProjectStatus] (
    [ProjectStatusId] INT           IDENTITY (1, 1) NOT NULL,
    [Name]            NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_ProjectStatus] PRIMARY KEY CLUSTERED ([ProjectStatusId] ASC)
);

