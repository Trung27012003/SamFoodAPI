-- ============================================
-- Migration: Create SiteSetting Table
-- Date: 2026-07-26
-- ============================================

-- 1. Tạo bảng SiteSetting
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='SiteSetting' AND xtype='U')
BEGIN
    CREATE TABLE SiteSetting (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        SettingKey NVARCHAR(100) NOT NULL UNIQUE,
        SettingValue NVARCHAR(MAX) NULL,
        ValueType NVARCHAR(50) NOT NULL DEFAULT 'string',
        [Group] NVARCHAR(50) NOT NULL DEFAULT 'General',
        DisplayName NVARCHAR(200) NOT NULL,
        Description NVARCHAR(500) NULL,
        SortOrder INT NOT NULL DEFAULT 0,
        IsPublic BIT NOT NULL DEFAULT 1,
        IsDeleted BIT NOT NULL DEFAULT 0,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
        UpdatedDate DATETIME NOT NULL DEFAULT GETDATE(),
        CreatedBy NVARCHAR(50) NULL,
        UpdatedBy NVARCHAR(50) NULL
    );

    CREATE INDEX IX_SiteSetting_Group ON SiteSetting([Group]);
    CREATE INDEX IX_SiteSetting_IsDeleted ON SiteSetting(IsDeleted);
END

-- 2. Seed dữ liệu mặc định (nếu bảng mới tạo)
IF NOT EXISTS (SELECT 1 FROM SiteSetting WHERE SettingKey = 'logo_header')
BEGIN
    INSERT INTO SiteSetting (SettingKey, SettingValue, ValueType, [Group], DisplayName, SortOrder, IsPublic, IsDeleted, CreatedDate, UpdatedDate)
    VALUES
        -- Logo
        ('logo_header', 'assets/image/logo.png', 'image', 'Logo', 'Logo header (mobile)', 1, 1, 0, GETDATE(), GETDATE()),
        ('logo_sidebar', 'assets/image/logo.png', 'image', 'Logo', 'Logo sidebar admin', 2, 1, 0, GETDATE(), GETDATE()),
        ('logo_footer', 'assets/image/logo.png', 'image', 'Logo', 'Logo footer', 3, 1, 0, GETDATE(), GETDATE()),
        ('logo_auth', 'assets/image/logo.jpg', 'image', 'Logo', 'Logo trang đăng nhập/đăng ký', 4, 1, 0, GETDATE(), GETDATE()),
        ('favicon', 'assets/image/logo.png', 'image', 'Logo', 'Favicon', 5, 1, 0, GETDATE(), GETDATE()),

        -- Brand
        ('brand_name', 'SamFoods', 'string', 'Brand', 'Tên thương hiệu', 1, 1, 0, GETDATE(), GETDATE()),
        ('footer_tagline', N'SamFoods – Đặt đồ ăn nhanh, giao tận nơi. Thực đơn đa dạng, nguyên liệu tươi sạch, giao hàng siêu tốc trong 30 phút.', 'text', 'Brand', 'Tagline footer', 2, 1, 0, GETDATE(), GETDATE()),
        ('footer_copyright', N'© 2026 SamFoods. Đã đăng ký bản quyền. Phát triển bởi SamFoods Team.', 'text', 'Brand', 'Copyright', 3, 1, 0, GETDATE(), GETDATE()),
        ('business_license', N'Đã thông báo Bộ Công Thương · Số GP-1234/GPCĐ-XXX', 'text', 'Brand', 'Số GP Bộ Công Thương', 4, 1, 0, GETDATE(), GETDATE()),

        -- Contact
        ('contact_address', N'123 Nguyễn Văn Cừ, Long Biên, Hà Nội', 'text', 'Contact', 'Địa chỉ', 1, 1, 0, GETDATE(), GETDATE()),
        ('contact_phone_1', '0384657756', 'string', 'Contact', 'Số điện thoại 1', 2, 1, 0, GETDATE(), GETDATE()),
        ('contact_phone_2', '0966669001', 'string', 'Contact', 'Số điện thoại 2', 3, 1, 0, GETDATE(), GETDATE()),
        ('contact_email', 'support@samfoods.vn', 'string', 'Contact', 'Email hỗ trợ', 4, 1, 0, GETDATE(), GETDATE()),
        ('contact_hours', N'Mở cửa: 8:00 – 22:00 (Tất cả các ngày)', 'text', 'Contact', 'Giờ mở cửa', 5, 1, 0, GETDATE(), GETDATE()),

        -- Social
        ('social_zalo_url', 'https://zalo.me/0966669001', 'string', 'Social', 'Zalo URL', 1, 1, 0, GETDATE(), GETDATE()),
        ('social_facebook_url', 'https://www.facebook.com/NguyenVietHaiLong', 'string', 'Social', 'Facebook URL', 2, 1, 0, GETDATE(), GETDATE()),
        ('social_messenger_url', 'https://m.me/NguyenVietHaiLong', 'string', 'Social', 'Messenger URL', 3, 1, 0, GETDATE(), GETDATE()),
        ('social_phone', 'tel:0384657756', 'string', 'Social', 'Phone tel', 4, 1, 0, GETDATE(), GETDATE()),

        -- Footer nav (JSON array)
        ('nav_about', N'[{"id":1,"label":"Giới thiệu","link":"/home"},{"id":2,"label":"Tầm nhìn & Sứ mệnh","link":"/home"},{"id":3,"label":"Đội ngũ của chúng tôi","link":"/home"},{"id":4,"label":"Tuyển dụng","link":"/home"},{"id":5,"label":"Tin tức & Sự kiện","link":"/home"}]', 'json', 'Footer', 'Về SamFoods (JSON)', 1, 1, 0, GETDATE(), GETDATE()),
        ('nav_support', N'[{"id":1,"label":"Hướng dẫn đặt hàng","link":"/home"},{"id":2,"label":"Câu hỏi thường gặp","link":"/home"},{"id":3,"label":"Liên hệ hỗ trợ","link":"/home"},{"id":4,"label":"Đăng ký đối tác","link":"/home"},{"id":5,"label":"Đánh giá dịch vụ","link":"/home"}]', 'json', 'Footer', 'Hỗ trợ khách hàng (JSON)', 2, 1, 0, GETDATE(), GETDATE()),
        ('nav_policy', N'[{"id":1,"label":"Chính sách đổi trả","link":"/home"},{"id":2,"label":"Chính sách hoàn tiền","link":"/home"},{"id":3,"label":"Chính sách vận chuyển","link":"/home"},{"id":4,"label":"Chính sách bảo mật","link":"/home"},{"id":5,"label":"Điều khoản sử dụng","link":"/home"}]', 'json', 'Footer', 'Chính sách (JSON)', 3, 1, 0, GETDATE(), GETDATE());
END

-- 3. Theme colors (preset) cho header & footer
IF NOT EXISTS (SELECT 1 FROM SiteSetting WHERE SettingKey = 'header_bg_color')
BEGIN
    INSERT INTO SiteSetting (SettingKey, SettingValue, ValueType, [Group], DisplayName, SortOrder, IsPublic, IsDeleted, CreatedDate, UpdatedDate)
    VALUES
        ('header_bg_color',   '#80001c', 'string', 'Brand', N'Màu nền header (preset)', 5, 1, 0, GETDATE(), GETDATE()),
        ('footer_bg_color',   '#2c1810', 'string', 'Brand', N'Màu nền footer (preset)', 6, 1, 0, GETDATE(), GETDATE()),
        ('footer_text_color', '#d4c5b0', 'string', 'Brand', N'Màu chữ footer (preset)', 7, 1, 0, GETDATE(), GETDATE());
END
