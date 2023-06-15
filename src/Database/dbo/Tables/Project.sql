CREATE TABLE [dbo].[Project] (
    [ProjectId]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]               NVARCHAR (255) NOT NULL,
    [ProjectStatusId]    INT            NOT NULL,
    [IsPublic]           BIT            NOT NULL,
    [PromoterLoginId]    INT            NULL,
    [Description]        NVARCHAR (MAX) NULL,
    [Keywords]           NVARCHAR (MAX) NULL,
    [DateTimeCreatedUtc] DATETIME2 (7)  NULL,
    [ResearchNotes]      NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Project] PRIMARY KEY CLUSTERED ([ProjectId] ASC),
    CONSTRAINT [FK_Project_ProjectStatus] FOREIGN KEY ([ProjectStatusId]) REFERENCES [dbo].[ProjectStatus] ([ProjectStatusId])
);

