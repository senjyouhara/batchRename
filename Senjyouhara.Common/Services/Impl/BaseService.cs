using Senjyouhara.Common.Models;
using Senjyouhara.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senjyouhara.Common.Services.Impl
{
    public class BaseService<TEntity> : IBaseService<TEntity> where TEntity : class
    {

        protected HttpClientService httpClient = new HttpClientService();
        public BaseService()
        {
        }

        public BaseService(HttpClientService httpClient)
        {
            this.httpClient = httpClient;
        }

        public virtual Task<ResBase<TEntity>> CreateAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public virtual Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public virtual Task<ResBase<TEntity>> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public virtual Task<ResBase<List<TEntity>>> Search(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public virtual Task<ResBase<TEntity>> UpdateAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
