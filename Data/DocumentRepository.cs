using Dapper;
using Microsoft.Data.SqlClient;
using ProjectManagementAPI.Models;
using System.Data;

namespace ProjectManagementAPI.Data
{
    public class DocumentRepository
    {
        private readonly string _connectionString;
        public DocumentRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> Create(DocumentModel doc)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.ExecuteScalarAsync<int>("usp_Document_Create",
                new { doc.FileName, doc.ContentType, doc.FileSizeBytes, doc.StoredPath, doc.ProjectId, doc.UploadedBy },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<DocumentModel>> GetAll()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<DocumentModel>("usp_Document_GetAll",
                commandType: CommandType.StoredProcedure);
        }

        public async Task<DocumentModel> GetById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QuerySingleOrDefaultAsync<DocumentModel>("usp_Document_GetById",
                new { DocumentId = id }, commandType: CommandType.StoredProcedure);
        }

        public async Task Delete(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync("usp_Document_Delete",
                new { DocumentId = id }, commandType: CommandType.StoredProcedure);
        }
    }
}