CREATE TABLE [dbo].[Link] (
    [LinkId]      INT            IDENTITY (1, 1) NOT NULL,
    [ProjectId]   INT            NOT NULL,
    [Url]         NVARCHAR (MAX) NOT NULL,
    [Description] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Link] PRIMARY KEY CLUSTERED ([LinkId] ASC),
    CONSTRAINT [FK_Link_Project] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[Project] ([ProjectId])
);

