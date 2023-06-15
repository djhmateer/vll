CREATE TABLE [dbo].[Issue] (
    [IssueId]            INT            IDENTITY (1, 1) NOT NULL,
    [ProjectId]          INT            NOT NULL,
    [DateTimeCreatedUtc] DATETIME2 (7)  NULL,
    [RegulatorId]        INT            NULL,
    [Name]               NVARCHAR (255) NOT NULL,
    [Description]        NVARCHAR (MAX) NULL,
    [Keywords]           NVARCHAR (MAX) NULL,
    [Response]           NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Issue] PRIMARY KEY CLUSTERED ([IssueId] ASC),
    CONSTRAINT [FK_Issue_Project] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[Project] ([ProjectId])
);

