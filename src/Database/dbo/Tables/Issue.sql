CREATE TABLE [dbo].[Issue] (
    [IssueId]            INT            IDENTITY (1, 1) NOT NULL,
    [ProjectId]          INT            NOT NULL,
    [RegulatorId]        INT            NULL,
    [Name]               NVARCHAR (255) NOT NULL,
    [IssueStatusId]      INT            NOT NULL,
    [IsPublic]           BIT            NOT NULL,
    [Description]        NVARCHAR (MAX) NULL,
    [Keywords]           NVARCHAR (MAX) NULL,
    [Response]           NVARCHAR (MAX) NULL,
    [DateTimeCreatedUtc] DATETIME2 (7)  CONSTRAINT [DF_Issue_DateTimeCreatedUtc] DEFAULT (getutcdate()) NULL,
    CONSTRAINT [PK_Issue] PRIMARY KEY CLUSTERED ([IssueId] ASC),
    CONSTRAINT [FK_Issue_Project] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[Project] ([ProjectId]),
    CONSTRAINT [FK_Issue_Regulator] FOREIGN KEY ([RegulatorId]) REFERENCES [dbo].[Regulator] ([RegulatorId])
);





