using _2_PeliculasAPI;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PeliculasApiTest
{
    public class LocalDbInitializer : BasePrueba
    {
        private static readonly string _dbName = "PruebasDeIntegracion";



        public static void Initialize(ApplicationDbContext context)
        {
            DeleteDB();
            CreateDB();
        }

       
        public static void End()
        {
            DeleteDB();
        }

        public static ApplicationDbContext GetDbContextLocalDb(bool beginTransaction = true)
        {
            //Initialize(ConstruirContext(_dbName)); //esto hace un bucle infinito

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer($"Data Source=.;Initial Catalog={_dbName};Integrated Security=True;",
                x =>
                {
                    x.UseNetTopologySuite();
                   
                })
                .Options;

            var context = new ApplicationDbContext(options);

            if (beginTransaction)
            {
                context.Database.BeginTransaction();
            }
            return context;
        }

        private static void CreateDB()
        {
            ExecuteCommand(Master, $@"
                CREATE DATABASE [{_dbName}]
                ON (NAME = '{_dbName}',
                FILENAME = '{Filename}')");

            using (var context = GetDbContextLocalDb(beginTransaction: false))
            {
                context.Database.Migrate();
                // Buen lugar para colocar data de prueba
                context.SaveChanges();
            }
        }

        static void DeleteDB()
        {
            var fileNames = GetDbFiles(Master, $@"
                SELECT [physical_name] FROM [sys].[master_files]
                WHERE [database_id] = DB_ID('{_dbName}')");

            if (fileNames.Any())
            {
                ExecuteCommand(Master, $@"
                    ALTER DATABASE [{_dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    EXEC sp_detach_db '{_dbName}'");

                foreach (var filename in fileNames)
                {
                    File.Delete(filename);
                }
            }
        }

        static void ExecuteCommand(string connectionString, string query)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand(query, connection))
                {
                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        static IEnumerable<string> GetDbFiles(
           string connectionString,
           string query)
        {
            IEnumerable<string> files;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var cmd = new SqlCommand(query, connection))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        files = from DataRow myRow in dt.Rows
                                select (string)myRow["physical_name"];
                    }
                }
            }
            return files;
        }

        static string Master =>
           new SqlConnectionStringBuilder
           {
               DataSource = @".",
               InitialCatalog = "master",
               IntegratedSecurity = true
           }.ConnectionString;

         static string Filename => Path.Combine(
            Path.GetDirectoryName(
                typeof(LocalDbInitializer).GetTypeInfo().Assembly.Location)!,
            $"{_dbName}.mdf");

         
        /*static string Filename => Path.Combine(
           "C:\\Users\\jesus\\1NO-DRIVE\\CURSOS\\CURSO ASP.NET\\EJERCICIOS\\2_PeliculasAPI\\PeliculasApiTest\\bin\\Debug\\net6.0\\",
           $"{_dbName}.mdf");

        */

    }  
}
