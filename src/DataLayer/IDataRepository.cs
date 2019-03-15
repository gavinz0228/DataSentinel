using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace DataSentinel.DataLayer{
    public interface IDataRepository
    {
        Task Add( string table,  Stream stream);
        Task Save( string table,  Stream stream, string filter);
        Task<long> Delete( string table, string filter);
        Task<IList<Object>> Get(string table, string filter);
    }
}