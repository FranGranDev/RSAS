IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [FirstName] nvarchar(max) NOT NULL,
    [LastName] nvarchar(max) NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Products] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Stocks] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Location] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Stocks] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(128) NOT NULL,
    [ProviderKey] nvarchar(128) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(128) NOT NULL,
    [Name] nvarchar(128) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [StockProducts] (
    [StockId] int NOT NULL,
    [ProductId] int NOT NULL,
    [Quantity] int NOT NULL,
    CONSTRAINT [PK_StockProducts] PRIMARY KEY ([StockId], [ProductId]),
    CONSTRAINT [FK_StockProducts_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_StockProducts_Stocks_StockId] FOREIGN KEY ([StockId]) REFERENCES [Stocks] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
GO

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
GO

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
GO

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO

CREATE INDEX [IX_StockProducts_ProductId] ON [StockProducts] ([ProductId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230325214319_init', N'7.0.4');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

EXEC sp_rename N'[Products].[Price]', N'WholesalePrice', N'COLUMN';
GO

ALTER TABLE [StockProducts] ADD [Discount] decimal(18,2) NOT NULL DEFAULT 0.0;
GO

ALTER TABLE [Products] ADD [RetailPrice] decimal(18,2) NOT NULL DEFAULT 0.0;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230401153813_update-products', N'7.0.4');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Stocks] ADD [SaleType] int NOT NULL DEFAULT 0;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230401154256_update-stock', N'7.0.4');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230410164925_fix-StockProducts', N'7.0.4');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'FirstName');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [AspNetUsers] DROP COLUMN [FirstName];
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'LastName');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [AspNetUsers] DROP COLUMN [LastName];
GO

ALTER TABLE [AspNetUsers] ADD [ClientId] int NULL;
GO

ALTER TABLE [AspNetUsers] ADD [CompanyId] int NULL;
GO

CREATE TABLE [Clients] (
    [Id] int NOT NULL IDENTITY,
    [FirstName] nvarchar(max) NOT NULL,
    [LastName] nvarchar(max) NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_Clients] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Clients_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Companies] (
    [Id] nvarchar(450) NOT NULL,
    [CompanyName] nvarchar(max) NOT NULL,
    [CompanyCode] nvarchar(max) NOT NULL,
    [Address] nvarchar(max) NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_Companies] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Companies_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX [IX_Clients_UserId] ON [Clients] ([UserId]);
GO

CREATE UNIQUE INDEX [IX_Companies_UserId] ON [Companies] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230416172336_Company-and-Client', N'7.0.4');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Companies] DROP CONSTRAINT [PK_Companies];
GO

DROP INDEX [IX_Companies_UserId] ON [Companies];
GO

ALTER TABLE [Clients] DROP CONSTRAINT [PK_Clients];
GO

DROP INDEX [IX_Clients_UserId] ON [Clients];
GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Companies]') AND [c].[name] = N'Id');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Companies] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [Companies] DROP COLUMN [Id];
GO

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Clients]') AND [c].[name] = N'Id');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Clients] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [Clients] DROP COLUMN [Id];
GO

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'ClientId');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [AspNetUsers] DROP COLUMN [ClientId];
GO

DECLARE @var5 sysname;
SELECT @var5 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'CompanyId');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var5 + '];');
ALTER TABLE [AspNetUsers] DROP COLUMN [CompanyId];
GO

ALTER TABLE [Companies] ADD CONSTRAINT [PK_Companies] PRIMARY KEY ([UserId]);
GO

ALTER TABLE [Clients] ADD CONSTRAINT [PK_Clients] PRIMARY KEY ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230416193722_fix-client-company', N'7.0.4');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Clients] ADD [Phone] nvarchar(max) NOT NULL DEFAULT N'';
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
    SET IDENTITY_INSERT [AspNetRoles] ON;
INSERT INTO [AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName])
VALUES (N'admin', N'71c18bde-401a-472a-9a46-0ae30382a7a4', N'Admin', N'ADMIN'),
(N'client', N'5594336a-09e1-4f2d-acdd-f33ece4016ad', N'Client', N'CLIENT');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
    SET IDENTITY_INSERT [AspNetRoles] OFF;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230416214728_add-phone', N'7.0.4');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DELETE FROM [AspNetRoles]
WHERE [Id] = N'admin';
SELECT @@ROWCOUNT;

GO

DELETE FROM [AspNetRoles]
WHERE [Id] = N'client';
SELECT @@ROWCOUNT;

GO

CREATE TABLE [Employees] (
    [UserId] nvarchar(450) NOT NULL,
    [FirstName] nvarchar(max) NOT NULL,
    [LastName] nvarchar(max) NOT NULL,
    [Phone] nvarchar(max) NOT NULL,
    [Role] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Employees] PRIMARY KEY ([UserId]),
    CONSTRAINT [FK_Employees_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
    SET IDENTITY_INSERT [AspNetRoles] ON;
INSERT INTO [AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName])
VALUES (N'dd00ee78-65b4-442e-8137-b9ae2a23d4ca', N'30d00054-54f7-4b5a-b006-7ee68a6da9d0', N'Admin', N'ADMIN');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
    SET IDENTITY_INSERT [AspNetRoles] OFF;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230417121426_add-employee', N'7.0.4');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DELETE FROM [AspNetRoles]
WHERE [Id] = N'dd00ee78-65b4-442e-8137-b9ae2a23d4ca';
SELECT @@ROWCOUNT;

GO

CREATE TABLE [Orders] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [StockId] int NOT NULL,
    [State] int NOT NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Orders_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Orders_Stocks_StockId] FOREIGN KEY ([StockId]) REFERENCES [Stocks] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [OrderInfo] (
    [Id] int NOT NULL IDENTITY,
    [Comment] nvarchar(max) NOT NULL,
    [Address] nvarchar(max) NOT NULL,
    [OrderDate] datetime2 NOT NULL,
    [OrderId] int NOT NULL,
    CONSTRAINT [PK_OrderInfo] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OrderInfo_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [OrderProducts] (
    [OrderId] int NOT NULL,
    [ProductId] int NOT NULL,
    [Quantity] int NOT NULL,
    CONSTRAINT [PK_OrderProducts] PRIMARY KEY ([OrderId], [ProductId]),
    CONSTRAINT [FK_OrderProducts_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrderProducts_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX [IX_OrderInfo_OrderId] ON [OrderInfo] ([OrderId]);
GO

CREATE INDEX [IX_OrderProducts_ProductId] ON [OrderProducts] ([ProductId]);
GO

CREATE INDEX [IX_Orders_StockId] ON [Orders] ([StockId]);
GO

CREATE INDEX [IX_Orders_UserId] ON [Orders] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230417143500_add-orders', N'7.0.4');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [OrderProducts] ADD [ProductPrice] decimal(18,2) NOT NULL DEFAULT 0.0;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230417145040_add-price-to-order', N'7.0.4');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [OrderProducts] ADD [ProductDescription] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [OrderProducts] ADD [ProductName] nvarchar(max) NOT NULL DEFAULT N'';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230417150251_dublicate-product-info', N'7.0.4');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Sales] (
    [Id] int NOT NULL IDENTITY,
    [SaleDate] datetime2 NOT NULL,
    [SaleType] int NOT NULL,
    [OrderId] int NOT NULL,
    CONSTRAINT [PK_Sales] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Sales_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_Sales_OrderId] ON [Sales] ([OrderId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230417150720_add-sales', N'7.0.4');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Companies] ADD [Phone] nvarchar(max) NOT NULL DEFAULT N'';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230417195455_add-phone-company', N'7.0.4');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Orders] DROP CONSTRAINT [FK_Orders_Stocks_StockId];
GO

DROP TABLE [OrderInfo];
GO

DECLARE @var6 sysname;
SELECT @var6 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Orders]') AND [c].[name] = N'StockId');
IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [Orders] DROP CONSTRAINT [' + @var6 + '];');
ALTER TABLE [Orders] ALTER COLUMN [StockId] int NULL;
GO

ALTER TABLE [Orders] ADD [ChangeDate] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

ALTER TABLE [Orders] ADD [ClientName] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [Orders] ADD [ContactPhone] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [Orders] ADD [OrderDate] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

ALTER TABLE [Orders] ADD [Type] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [Orders] ADD CONSTRAINT [FK_Orders_Stocks_StockId] FOREIGN KEY ([StockId]) REFERENCES [Stocks] ([Id]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230424183828_update-orders', N'7.0.4');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Orders] DROP CONSTRAINT [FK_Orders_Stocks_StockId];
GO

DROP INDEX [IX_Orders_StockId] ON [Orders];
GO

DECLARE @var7 sysname;
SELECT @var7 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Orders]') AND [c].[name] = N'StockId');
IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [Orders] DROP CONSTRAINT [' + @var7 + '];');
ALTER TABLE [Orders] DROP COLUMN [StockId];
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230424190701_fix-stock', N'7.0.4');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Orders] ADD [PaymentType] nvarchar(max) NOT NULL DEFAULT N'';
GO

CREATE TABLE [OrdersDelivery] (
    [Id] int NOT NULL,
    [OrderId] int NOT NULL,
    [DeliveryDate] datetime2 NOT NULL,
    [City] nvarchar(max) NOT NULL,
    [Street] nvarchar(max) NOT NULL,
    [House] nvarchar(max) NOT NULL,
    [Flat] nvarchar(max) NOT NULL,
    [PostalCode] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_OrdersDelivery] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OrdersDelivery_Orders_Id] FOREIGN KEY ([Id]) REFERENCES [Orders] ([Id]) ON DELETE CASCADE
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230425122452_add-delivery', N'7.0.4');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [OrdersDelivery] DROP CONSTRAINT [FK_OrdersDelivery_Orders_Id];
GO

ALTER TABLE [OrdersDelivery] DROP CONSTRAINT [PK_OrdersDelivery];
GO

DECLARE @var8 sysname;
SELECT @var8 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OrdersDelivery]') AND [c].[name] = N'Id');
IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [OrdersDelivery] DROP CONSTRAINT [' + @var8 + '];');
ALTER TABLE [OrdersDelivery] DROP COLUMN [Id];
GO

ALTER TABLE [OrdersDelivery] ADD CONSTRAINT [PK_OrdersDelivery] PRIMARY KEY ([OrderId]);
GO

ALTER TABLE [OrdersDelivery] ADD CONSTRAINT [FK_OrdersDelivery_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE CASCADE;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230425130611_fix-delivery', N'7.0.4');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Sales] ADD [StockId] int NOT NULL DEFAULT 0;
GO

CREATE INDEX [IX_Sales_StockId] ON [Sales] ([StockId]);
GO

ALTER TABLE [Sales] ADD CONSTRAINT [FK_Sales_Stocks_StockId] FOREIGN KEY ([StockId]) REFERENCES [Stocks] ([Id]) ON DELETE CASCADE;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230425154706_fix-sale', N'7.0.4');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var9 sysname;
SELECT @var9 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Orders]') AND [c].[name] = N'PaymentType');
IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [Orders] DROP CONSTRAINT [' + @var9 + '];');
ALTER TABLE [Orders] ALTER COLUMN [PaymentType] int NOT NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230429122548_change-payment-to-enum', N'7.0.4');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Orders] ADD [StockId] int NOT NULL DEFAULT 0;
GO

CREATE INDEX [IX_Orders_StockId] ON [Orders] ([StockId]);
GO

ALTER TABLE [Orders] ADD CONSTRAINT [FK_Orders_Stocks_StockId] FOREIGN KEY ([StockId]) REFERENCES [Stocks] ([Id]) ON DELETE SET DEFAULT;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230430002414_add-stockid-order', N'7.0.4');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Orders] DROP CONSTRAINT [FK_Orders_Stocks_StockId];
GO

ALTER TABLE [Orders] ADD CONSTRAINT [FK_Orders_Stocks_StockId] FOREIGN KEY ([StockId]) REFERENCES [Stocks] ([Id]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230430010105_fix-order-stockid', N'7.0.4');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var10 sysname;
SELECT @var10 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Orders]') AND [c].[name] = N'StockId');
IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [Orders] DROP CONSTRAINT [' + @var10 + '];');
ALTER TABLE [Orders] ALTER COLUMN [StockId] int NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230430010506_fix-order', N'7.0.4');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

EXEC sp_rename N'[Companies].[CompanyName]', N'Name', N'COLUMN';
GO

EXEC sp_rename N'[Companies].[CompanyCode]', N'Kpp', N'COLUMN';
GO

EXEC sp_rename N'[Companies].[Address]', N'Inn', N'COLUMN';
GO

ALTER TABLE [Companies] ADD [BankAccount] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [Companies] ADD [BankBic] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [Companies] ADD [BankName] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [Companies] ADD [Email] nvarchar(max) NOT NULL DEFAULT N'';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230430200731_update-company', N'7.0.4');
GO

COMMIT;
GO

