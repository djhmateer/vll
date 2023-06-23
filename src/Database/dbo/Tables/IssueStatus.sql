CREATE TABLE [dbo].[IssueStatus] (
    [IssueStatusId] INT            IDENTITY (1, 1) NOT NULL,
    [Name]          NVARCHAR (255) NOT NULL,
    CONSTRAINT [PK_IssueStatus] PRIMARY KEY CLUSTERED ([IssueStatusId] ASC)
);

