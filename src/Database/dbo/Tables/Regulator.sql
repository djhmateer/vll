CREATE TABLE [dbo].[Regulator] (
    [RegulatorId]  INT            IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (255) NOT NULL,
    [ContactEmail] NVARCHAR (255) NULL,
    CONSTRAINT [PK_Regulator] PRIMARY KEY CLUSTERED ([RegulatorId] ASC)
);

