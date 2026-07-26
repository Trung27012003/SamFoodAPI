-- ============================================
-- Migration: Create Banner and BannerDetail Tables
-- Date: 2026-07-20
-- ============================================

-- 1. Tạo bảng Banner
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Banner' AND xtype='U')
BEGIN
    CREATE TABLE Banner (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        BannerCode NVARCHAR(50) NOT NULL UNIQUE,
        BannerName NVARCHAR(200) NOT NULL,
        Description NVARCHAR(MAX) NULL,
        SlideshowInterval INT DEFAULT 5,
        IsActive BIT DEFAULT 1,
        CreatedDate DATETIME DEFAULT GETDATE(),
        UpdatedDate DATETIME DEFAULT GETDATE(),
        IsDeleted BIT DEFAULT 0
    );
END

-- 2. Tạo bảng BannerDetail
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='BannerDetail' AND xtype='U')
BEGIN
    CREATE TABLE BannerDetail (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        BannerID INT NOT NULL,
        ImageName NVARCHAR(MAX) NOT NULL,
        SortOrder INT DEFAULT 0,
        LinkURL NVARCHAR(500) NULL,
        CreatedDate DATETIME DEFAULT GETDATE(),
        UpdatedDate DATETIME DEFAULT GETDATE(),
        IsDeleted BIT DEFAULT 0,
        CONSTRAINT FK_BannerDetail_Banner FOREIGN KEY (BannerID) REFERENCES Banner(ID) ON DELETE CASCADE
    );
END

-- 3. Thêm Index cho BannerDetail
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_BannerDetail_BannerID' AND object_id = OBJECT_ID('BannerDetail'))
BEGIN
    CREATE INDEX IX_BannerDetail_BannerID ON BannerDetail(BannerID);
END

-- 4. Thêm Index cho Banner
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_Banner_IsActive_IsDeleted' AND object_id = OBJECT_ID('Banner'))
BEGIN
    CREATE INDEX IX_Banner_IsActive_IsDeleted ON Banner(IsActive, IsDeleted);
END
