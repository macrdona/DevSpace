using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using MongoDB.Driver;

namespace backend.Helpers.Wrappers
{
    public interface IFindFluentWrapper<TDocument>
    {
        Task<TDocument> FirstOrDefaultAsync();
    }
    public class FindFluentWrapper<TDocument> : IFindFluentWrapper<TDocument>
    {
        private readonly IFindFluent<TDocument,TDocument> _findFluent;

        public FindFluentWrapper(IFindFluent<TDocument,TDocument> findFluent)
        {
            _findFluent = findFluent;
        }

        public async Task<TDocument> FirstOrDefaultAsync()
        {
            return await _findFluent.FirstOrDefaultAsync();
        }
    }
}
