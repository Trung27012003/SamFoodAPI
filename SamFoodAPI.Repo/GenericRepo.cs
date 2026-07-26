using Microsoft.EntityFrameworkCore;
using SamFoodAPI.IRepo;
using SamFoodAPI.Model.Context;
using SamFoodAPI.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SamFoodAPI.Repo
{
    public class GenericRepo<T> : IGenericRepo<T> where T : class, new()
    {
        protected SamFoodContext db { get; set; }
        protected DbSet<T> table;

        public GenericRepo(CurrentUser currentUser)
        {
            db = new SamFoodContext();
            table = db.Set<T>();
            db.CurrentUser = currentUser;
        }

        public GenericRepo(SamFoodContext db, CurrentUser currentUser)
        {
            this.db = db;
            table = db.Set<T>();
            db.CurrentUser = currentUser;
        }

        public List<T> GetAll(Expression<Func<T, bool>> predicate = null)
        {
            try
            {
                if (predicate == null) return table.ToList() ?? new List<T>();
                else return table.Where(predicate).ToList() ?? new List<T>(); ; // EF sẽ dịch sang SQL WHERE
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        

        public async Task<T> GetByIDAsync(int id)
        {
            try
            {
                T model = await table.FindAsync(id) ?? new T();
                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        

        public async Task<int> CreateAsync(T item)
        {
            try
            {
                await table.AddAsync(item);
                return await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<int> CreateRangeAsync(List<T> items)
        {
            try
            {
                await table.AddRangeAsync(items);
                return await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<int> UpdateAsync(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            try
            {
                var entryDto = db.Entry(item);

                // Lấy ID
                var idProp = entryDto.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey());
                if (idProp == null)
                    throw new Exception("Entity must have primary key");

                var id = idProp.CurrentValue;

                // 1. Lấy entity thật từ DB (QUAN TRỌNG)
                var entity = await db.Set<T>().FindAsync(id);
                if (entity == null)
                    throw new Exception($"Entity with ID {id} not found");

                var entry = db.Entry(entity);

                // 2. Copy value nhanh bằng EF (KHÔNG loop reflection thủ công)
                entry.CurrentValues.SetValues(item);

                // 3. Không overwrite null (điểm quan trọng nhất)
                foreach (var prop in entry.Properties)
                {
                    if (prop.Metadata.IsPrimaryKey())
                        continue;

                    var newValue = entryDto.Property(prop.Metadata.Name).CurrentValue;

                    if (newValue == null)
                    {
                        prop.IsModified = false;
                    }
                }

                // 4. UpdatedDate auto
                var updatedDateProp = entry.Properties
                    .FirstOrDefault(p => p.Metadata.Name == "UpdatedDate");

                if (updatedDateProp != null)
                {
                    updatedDateProp.CurrentValue = DateTime.Now;
                    updatedDateProp.IsModified = true;
                }

                var result = await db.SaveChangesAsync();

                if (result == 0)
                    throw new Exception("Update failed: no rows affected");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating {typeof(T).Name}: {ex.Message}", ex);
            }
        }


        public async Task<int> DeleteAsync(int id)
        {
            try
            {
                T model = await table.FindAsync(id) ?? new T();
                await UpdateAsync(model);
                return await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public int DeleteRange(List<T> items)
        {
            try
            {
                table.RemoveRange(items);
                return db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<int> DeleteRangeAsync(List<T> items)
        {
            try
            {
                var isDeletedProp = typeof(T).GetProperty("IsDeleted");
                if (isDeletedProp != null && isDeletedProp.CanWrite)
                {
                    foreach (var item in items)
                    {
                        var propType = isDeletedProp.PropertyType;
                        if (propType == typeof(bool) || propType == typeof(bool?))
                            isDeletedProp.SetValue(item, true);
                        else if (propType == typeof(int) || propType == typeof(int?))
                            isDeletedProp.SetValue(item, 1);
                    }
                    table.UpdateRange(items);
                }
                else
                {
                    table.RemoveRange(items);
                }
                return await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
