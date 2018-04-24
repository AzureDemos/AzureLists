CREATE TABLE [dbo].[Task]
(
	[Id] NVARCHAR(50) NOT NULL PRIMARY KEY, 
    [Title] NVARCHAR(75) NOT NULL, 
    [Notes] NVARCHAR(200) NULL, 
    [Important] BIT NULL, 
    [CompletedDate] DATETIME NULL, 
    [DueDate] DATETIME NULL, 
    [ListId] NVARCHAR(50) NOT NULL, 
    CONSTRAINT [FK_Task_List] FOREIGN KEY ([ListId]) REFERENCES [List]([Id]) 
	ON DELETE CASCADE
)
