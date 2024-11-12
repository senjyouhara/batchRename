using Senjyouhara.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senjyouhara.Common.Services
{
    public interface IBaseService<TEntity> where TEntity : class
    {

        Task<ResBase<TEntity>> CreateAsync(TEntity entity);

        Task<ResBase<TEntity>> UpdateAsync(TEntity entity);
        Task<ResBase<TEntity>> GetAsync(int id);

        Task DeleteAsync(int id);

        Task<ResBase<List<TEntity>>> Search(TEntity entity);

    }
}
