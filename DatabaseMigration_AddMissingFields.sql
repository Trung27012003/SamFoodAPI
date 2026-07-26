-- ============================================
-- Migration: Add Missing Fields to Promotion and UnitCount Tables
-- Date: 2026-07-19
-- ============================================

-- 1. Thêm các trường vào bảng Promotion
ALTER TABLE Promotion ADD COLUMN DiscountType INT NULL;
ALTER TABLE Promotion ADD COLUMN DiscountValue DECIMAL(18,2) NULL;
ALTER TABLE Promotion ADD COLUMN MinOrderAmount DECIMAL(18,2) NULL;
ALTER TABLE Promotion ADD COLUMN MaxDiscountAmount DECIMAL(18,2) NULL;
ALTER TABLE Promotion ADD COLUMN UsageLimit INT NULL;
ALTER TABLE Promotion ADD COLUMN UsedCount INT NULL DEFAULT 0;
ALTER TABLE Promotion ADD COLUMN IsActive BIT NULL DEFAULT 1;

-- 2. Thêm các trường vào bảng UnitCount
ALTER TABLE UnitCount ADD COLUMN Descriptions NVARCHAR(MAX) NULL;
ALTER TABLE UnitCount ADD COLUMN IsDeleted BIT NULL DEFAULT 0;

-- ============================================
-- Optional: Update existing records with defaults
-- ============================================

-- Set IsActive = 1 for existing promotions (assuming they were active)
UPDATE Promotion SET IsActive = 1 WHERE IsActive IS NULL;

-- Set IsDeleted = 0 for existing unit counts
UPDATE UnitCount SET IsDeleted = 0 WHERE IsDeleted IS NULL;

-- ============================================
-- 3. Thêm các trường vào bảng InvoiceDetail
-- ============================================
ALTER TABLE InvoiceDetail ADD COLUMN UnitPrice DECIMAL(18,2) NULL;

-- ============================================
-- 4. Thêm các trường vào bảng Invoice
-- ============================================
ALTER TABLE Invoice ADD COLUMN TotalAmount DECIMAL(18,2) NULL;
ALTER TABLE Invoice ADD COLUMN DiscountAmount DECIMAL(18,2) NULL DEFAULT 0;
ALTER TABLE Invoice ADD COLUMN PromotionID INT NULL;
ALTER TABLE Invoice ADD COLUMN PaymentMethod INT NULL;

-- ============================================
-- 5. Thêm các trường vào bảng User
-- ============================================
ALTER TABLE [User] ADD COLUMN FullName NVARCHAR(255) NULL;
ALTER TABLE [User] ADD COLUMN Email NVARCHAR(255) NULL;
ALTER TABLE [User] ADD COLUMN PhoneNumber NVARCHAR(50) NULL;
ALTER TABLE [User] ADD COLUMN IsActive BIT NULL DEFAULT 1;

-- ============================================
-- 6. Thêm các trường vào bảng ProductImage
-- ============================================
ALTER TABLE ProductImage ADD COLUMN IsPrimary BIT NULL DEFAULT 0;

-- ============================================
-- Optional: Update existing records with defaults
-- ============================================

-- Set IsActive = 1 for existing users (assuming they were active)
UPDATE [User] SET IsActive = 1 WHERE IsActive IS NULL;
