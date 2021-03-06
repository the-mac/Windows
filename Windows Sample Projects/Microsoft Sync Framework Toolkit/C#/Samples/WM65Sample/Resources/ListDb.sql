-- Database information:
-- locale identifier: 1033
-- encryption mode: 
-- case sensitive: False
-- [IsDirty] column is used to track changes
-- [IsTombstone] column is used to track local deletes

-- Note: The ETag and EditUri for the entities are empty for this CTP release and so we don't need to persist them.

CREATE TABLE [Item] ([ID] uniqueidentifier NOT NULL   
, [ListID] uniqueidentifier NOT NULL   
, [UserID] uniqueidentifier NOT NULL   
, [Name] nvarchar(50) NOT NULL  
, [Description] nvarchar(250) NULL  
, [Priority] int NULL   
, [Status] int NULL   
, [StartDate] datetime NULL   
, [EndDate] datetime NULL
, [IsDirty] bit DEFAULT (0) 
, [IsTombstone] bit DEFAULT (0)
-- Column to save the unique id for the record sent by the server.
, [_MetadataID] nvarchar(250) NULL
);
GO
CREATE TABLE [List] ([ID] uniqueidentifier NOT NULL DEFAULT newid()  
, [Name] nvarchar(100) NOT NULL  
, [Description] nvarchar(250) NULL  
, [UserID] uniqueidentifier NOT NULL   
, [CreatedDate] datetime NOT NULL DEFAULT getdate()  
, [IsDirty] bit DEFAULT (0)
, [IsTombstone] bit DEFAULT (0)
, [_MetadataID] nvarchar(250)  NULL
);
GO
CREATE TABLE [Priority] ([ID] int NOT NULL   
, [Name] nvarchar(50) NOT NULL  
, [IsDirty] bit DEFAULT (0)
, [IsTombstone] bit DEFAULT (0)
, [_MetadataID] nvarchar(250) NULL
);
GO
CREATE TABLE [Status] ([ID] int NOT NULL   
, [Name] nvarchar(50) NULL  
, [IsDirty] bit DEFAULT (0)
, [IsTombstone] bit DEFAULT (0)
, [_MetadataID] nvarchar(250) NULL
);
GO
CREATE TABLE [Tag] ([ID] int NOT NULL   
, [Name] nvarchar(100) NULL  
, [IsDirty] bit DEFAULT (0)
, [IsTombstone] bit DEFAULT (0)
, [_MetadataID] nvarchar(250) NULL
);
GO
CREATE TABLE [TagItemMapping] ([TagID] int NOT NULL   
, [ItemID] uniqueidentifier NOT NULL   
, [UserID] uniqueidentifier NOT NULL   
, [IsDirty] bit DEFAULT (0)
, [IsTombstone] bit DEFAULT (0)
, [_MetadataID] nvarchar(250) NULL
);
GO
CREATE TABLE [User] ([ID] uniqueidentifier NOT NULL   
, [Name] nvarchar(50) NOT NULL  
, [IsDirty] bit DEFAULT (0)
, [IsTombstone] bit DEFAULT (0)
, [_MetadataID] nvarchar(250) NULL
);
GO
CREATE TABLE [__sync] ([Anchor] image NULL);
GO
INSERT INTO [__sync] VALUES (null);
GO
ALTER TABLE [Item] ADD PRIMARY KEY ([ID]);
GO
ALTER TABLE [List] ADD PRIMARY KEY ([ID]);
GO
ALTER TABLE [Priority] ADD PRIMARY KEY ([ID]);
GO
ALTER TABLE [Status] ADD PRIMARY KEY ([ID]);
GO
ALTER TABLE [Tag] ADD PRIMARY KEY ([ID]);
GO
ALTER TABLE [TagItemMapping] ADD PRIMARY KEY ([TagID],[ItemID],[UserID]);
GO
ALTER TABLE [User] ADD PRIMARY KEY ([ID]);
GO
GO
CREATE UNIQUE INDEX [UQ__Item__000000000000005F] ON [Item] ([ID] Asc);
GO
CREATE UNIQUE INDEX [UQ__List__0000000000000035] ON [List] ([ID] Asc);
GO
CREATE UNIQUE INDEX [UQ__Priority__0000000000000043] ON [Priority] ([ID] Asc);
GO
CREATE UNIQUE INDEX [UQ__Tag__0000000000000018] ON [Tag] ([ID] Asc);
GO
CREATE UNIQUE INDEX [UQ__User__000000000000000A] ON [User] ([ID] Asc);
GO

