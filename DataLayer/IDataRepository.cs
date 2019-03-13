using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace DataSentinel.DataLayer{
    public interface IDataRepository
    {
        Task Add( string table, string obj);
        Task Save( string table, string obj);
        Task<long> Delete( string table, string keyColumn, string value);
        Task<IList<Object>> Get(string table, string keyColumn, string value);
    }
}