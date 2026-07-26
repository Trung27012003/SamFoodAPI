using Microsoft.EntityFrameworkCore;
using SamFoodAPI.Model.Common;
using SamFoodAPI.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamFoodAPI.Model.Context
{
    public partial class SamFoodContext
    {
        public CurrentUser CurrentUser { get; set; } = new CurrentUser();
        public SamFoodContext()
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(Config.ConnectionString);

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var now = DateTime.Now;
                var loginName = CurrentUser?.LoginName ?? "Admin";

                // 1. Lấy danh sách entity cần xử lý (materialize ngay tránh bị thay đổi sau)
                var entries = ChangeTracker.Entries()
                    .Where(e => e.Entity != null &&
                               (e.State == EntityState.Added || e.State == EntityState.Modified))
                    .ToList();

                // 2. Set audit field (KHÔNG dùng reflection nhiều lần)
                foreach (var entry in entries)
                {
                    var entity = entry.Entity;
                    var type = entity.GetType();

                    // cache property (giảm reflection cost)
                    var createdBy = type.GetProperty("CreatedBy");
                    var createdDate = type.GetProperty("CreatedDate");
                    var updatedBy = type.GetProperty("UpdatedBy");
                    var updatedDate = type.GetProperty("UpdatedDate");
                    var isDeleted = type.GetProperty("IsDeleted");

                    if (entry.State == EntityState.Added)
                    {
                        createdBy?.SetValue(entity, loginName);
                        createdDate?.SetValue(entity, now);

                        updatedBy?.SetValue(entity, loginName);
                        updatedDate?.SetValue(entity, now);

                        //var isDeletedVal = isDeleted?.GetValue(entry);
                        //isDeleted?.SetValue(entity,   false);

                        if (isDeleted != null)
                        {
                            var val = isDeleted.GetValue(entity);
                            if (val == null) isDeleted.SetValue(entity, false);
                        }
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        updatedBy?.SetValue(entity, loginName);

                        if (updatedDate != null)
                        {
                            var val = updatedDate.GetValue(entity);
                            if (val == null)
                                updatedDate.SetValue(entity, now);
                        }
                    }
                }

                // 3. Tạo audit log NHƯNG KHÔNG add vào DbContext ngay
                //var logs = BuildAuditLogs();

                // 4. Save entity chính trước (transaction ngắn lại)
                var result = await base.SaveChangesAsync(cancellationToken);

                // 5. Save log riêng (tránh lock + timeout)
                //if (logs.Count > 0)
                //{
                //    await SaveAuditLogsAsync(logs);
                //}

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"SaveChanges failed: {ex.Message}", ex);
            }
        }

    }
}
