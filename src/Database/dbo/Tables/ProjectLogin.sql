CREATE TABLE [dbo].[ProjectLogin] (
    [ProjectLoginId] INT IDENTITY (1, 1) NOT NULL,
    [ProjectId]      INT NOT NULL,
    [LoginId]        INT NOT NULL,
    CONSTRAINT [PK_ProjectLogin] PRIMARY KEY CLUSTERED ([ProjectLoginId] ASC),
    CONSTRAINT [FK_ProjectLogin_Login] FOREIGN KEY ([LoginId]) REFERENCES [dbo].[Login] ([LoginId]),
    CONSTRAINT [FK_ProjectLogin_Project] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[Project] ([ProjectId])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_ProjectLogin]
    ON [dbo].[ProjectLogin]([ProjectId] ASC, [LoginId] ASC);

