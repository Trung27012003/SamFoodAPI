USE [SamFood]
GO

-- Insert USER role
IF NOT EXISTS (SELECT 1 FROM [Role] WHERE RoleCode = 'USER' AND IsDeleted = 0)
BEGIN
    INSERT INTO [Role] (RoleCode, RoleName, CreatedDate, IsDeleted)
    VALUES (N'USER', N'Người dùng', GETDATE(), 0)
END
GO
-- Insert ADMIN role
IF NOT EXISTS (SELECT 1 FROM [Role] WHERE RoleCode = 'ADMIN' AND IsDeleted = 0)
BEGIN
    INSERT INTO [Role] (RoleCode, RoleName, CreatedDate, IsDeleted)
    VALUES (N'ADMIN', N'Quản trị viên', GETDATE(), 0)
END
GO

-- =============================================
-- Author:		SamFoods
-- Create date: 2026-07-19
-- Description:	Đăng nhập - lấy thông tin user kèm RoleCodes
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spLogin')
    DROP PROCEDURE [dbo].[spLogin]
GO

CREATE PROCEDURE [dbo].[spLogin]
    @UserName NVARCHAR(255),
    @Password NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    -- Check credentials and get user info with RoleCodes
    SELECT
        u.ID,
        u.UserName,
        u.FullName,
        u.Email,
        u.PhoneNumber,
        STUFF((
            SELECT ',' + r.RoleCode
            FROM [RoleUser] ru
            INNER JOIN [Role] r ON ru.RoleID = r.ID
            WHERE ru.UserID = u.ID AND ru.IsDeleted = 0 AND r.IsDeleted = 0
            FOR XML PATH('')
        ), 1, 1, '') AS RoleCodes
    FROM [User] u
    WHERE u.UserName = @UserName
        AND u.PasswordHash = @Password
        AND u.IsDeleted = 0
        AND u.IsActive = 1
END
GO
GRANT EXECUTE ON [dbo].[spLogin] TO PUBLIC
GO

-- =============================================
-- Author:		SamFoods
-- Create date: 2026-07-19
-- Description:	Đăng ký tài khoản mới
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spRegister')
    DROP PROCEDURE [dbo].[spRegister]
GO

CREATE PROCEDURE [dbo].[spRegister]
    @UserName NVARCHAR(255),
    @Password NVARCHAR(255),
    @FullName NVARCHAR(255),
    @Email NVARCHAR(255) = '',
    @PhoneNumber NVARCHAR(50) = ''
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if username already exists
    IF EXISTS (SELECT 1 FROM [User] WHERE UserName = @UserName AND IsDeleted = 0)
    BEGIN
        SELECT 0 AS Result, N'Tên đăng nhập đã tồn tại' AS Message, NULL AS UserID
        RETURN
    END

    -- Check if email already exists (only if email is provided)
    IF @Email <> '' AND EXISTS (SELECT 1 FROM [User] WHERE Email = @Email AND IsDeleted = 0)
    BEGIN
        SELECT 0 AS Result, N'Email đã được sử dụng' AS Message, NULL AS UserID
        RETURN
    END

    -- Insert new user
    INSERT INTO [User] (UserName, PasswordHash, FullName, Email, PhoneNumber, IsDeleted, CreatedDate, IsActive)
    VALUES (@UserName, @Password, @FullName, @Email, @PhoneNumber, 0, GETDATE(), 1)

    DECLARE @NewUserID INT = SCOPE_IDENTITY()

    -- Get USER role ID and insert into RoleUser
    DECLARE @UserRoleID INT
    SELECT @UserRoleID = ID FROM [Role] WHERE RoleCode = 'USER' AND IsDeleted = 0

    IF @UserRoleID IS NOT NULL
    BEGIN
        INSERT INTO [RoleUser] (UserID, RoleID, CreatedDate, IsDeleted)
        VALUES (@NewUserID, @UserRoleID, GETDATE(), 0)
    END

    SELECT 1 AS Result, N'Đăng ký thành công' AS Message, @NewUserID AS UserID
END
GO
GRANT EXECUTE ON [dbo].[spRegister] TO PUBLIC
GO

-- =============================================
-- Author:		SamFoods
-- Create date: 2026-07-19
-- Description:	Đổi mật khẩu
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spChangePassword')
    DROP PROCEDURE [dbo].[spChangePassword]
GO

CREATE PROCEDURE [dbo].[spChangePassword]
    @UserID INT,
    @OldPassword NVARCHAR(255),
    @NewPassword NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    -- Verify old password
    IF NOT EXISTS (SELECT 1 FROM [User] WHERE ID = @UserID AND PasswordHash = @OldPassword AND IsDeleted = 0)
    BEGIN
        SELECT 0 AS Result, N'Mật khẩu cũ không đúng' AS Message
        RETURN
    END

    -- Update password
    UPDATE [User]
    SET PasswordHash = @NewPassword,
        UpdatedDate = GETDATE()
    WHERE ID = @UserID

    SELECT 1 AS Result, N'Đổi mật khẩu thành công' AS Message
END
GO
GRANT EXECUTE ON [dbo].[spChangePassword] TO PUBLIC
GO
