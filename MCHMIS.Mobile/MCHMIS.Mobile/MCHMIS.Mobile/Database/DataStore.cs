using MCHMIS.Mobile.Interface;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;

namespace MCHMIS.Mobile.Database
{
    public class DataStore
    {
        private readonly SQLiteConnection _database;
        private readonly string nameSpace = "MCHMIS.Mobile.Database.";

        public DataStore()
        {
            _database = DependencyService.Get<ISQLite>().GetConnection();

            this._database.CreateTable<SystemCode>();
            this._database.CreateTable<SystemCodeDetail>();

            this._database.CreateTable<Ward>();
            this._database.CreateTable<Village>();
            this._database.CreateTable<CommunityArea>();
            //   this._database.CreateTable<Programme>();
            this._database.CreateTable<Enumerator>();

            this._database.CreateTable<Registration>();
            this._database.CreateTable<RegistrationMember>();

            this._database.CreateTable<RegistrationProgramme>();
            this._database.CreateTable<RegistrationMemberDisability>();
        }

        public virtual void AddOrUpdate<TEntity>(TEntity entity) where TEntity : class
        {
            this._database.InsertOrReplace(entity);
        }

        public virtual void Create<TEntity>(TEntity entity) where TEntity : class
        {
            this._database.Insert(entity);
        }

        public virtual void Delete<TEntity>(TEntity entity) where TEntity : class
        {
            this._database.Delete(entity);
        }

        public virtual void Delete<TEntity>(int primarykey) where TEntity : class
        {
            this._database.Delete<TEntity>(primarykey);
        }

        public virtual void Manage<TEntity>(TEntity entity) where TEntity : class
        {
        }

        public Village SubLocationGetById(int id)
        {
            return this._database.Query<Village>($"SELECT * FROM [SubLocation] WHERE [Id] = {id}").Single();
        }

        public List<Village> SubLocationGetByLocationId(int id)
        {
            return this._database.Query<Village>($"SELECT * FROM [SubLocation] WHERE [LocationId] = {id}");
        }

        public virtual void Update<TEntity>(TEntity entity) where TEntity : class
        {
            this._database.Update(entity);
        }

        #region SystemCodeDetail

        public List<SystemCodeDetail> SystemCodeDetailGetAll()
        {
            return this._database.Query<SystemCodeDetail>(
                "SELECT * FROM [SystemCodeDetail]");
        }

        public SystemCodeDetail SystemCodeDetailGetByCode(string parentCode, string childCode)
        {
            var sql1 = $"SELECT * FROM [SystemCode]  WHERE [Code] = '{parentCode}'";
            Debug.WriteLine(sql1);
            var systemCode = this._database.Query<SystemCode>(sql1).Single();

            var sql2 =
                $"SELECT * FROM [SystemCodeDetail] WHERE [Code] = '{childCode}' AND [SystemCodeId] ={systemCode.Id} ";
            Debug.WriteLine(sql2);
            return this._database.Query<SystemCodeDetail>(sql2).Single();
        }

        public SystemCodeDetail SystemCodeDetailGetById(int id)
        {
            return this._database.Query<SystemCodeDetail>($"SELECT * FROM [SystemCodeDetail] WHERE [Id] = {id}").Single();
        }

        public List<SystemCodeDetail> SystemCodeDetailsGetByCode(string code)
        {
            var systemCode = this._database.Query<SystemCode>($"select * from systemCode where code = '{code}'").Single();

            var systemcodetails = this._database.Query<SystemCodeDetail>(
                $"SELECT * FROM [SystemCodeDetail] WHERE SystemCodeId ={systemCode.Id} order by OrderNo asc");
            return systemcodetails;
        }

        #endregion SystemCodeDetail

        #region Abstract Get

        public object GetCvRegData(int id)
        {
            //var query =
            //    $"SELECT * FROM [CvRegistration] R INNER JOIN [SubLocation] A ON R.SubLocationId = A.Id and R.Id = {id}";
            var query =
                $"SELECT * FROM [CvRegistration]";
            object[] obj = new object[] { };
            TableMapping map = new TableMapping(Type.GetType(nameSpace + "CvRegistration"));
            return _database.Query(map, query, obj).Single();
        }

        public List<object> GetTable(string tableName)
        {
            // tableName =  + tableName;
            object[] obj = new object[] { };
            TableMapping map = new TableMapping(Type.GetType(nameSpace + tableName));
            string query = "SELECT * FROM [" + tableName + "]";
            return _database.Query(map, query, obj).ToList();
        }

        public T GetTableRow<T>(string tableName, string column, string value)
        {
            object[] obj = new object[] { };
            TableMapping map = new TableMapping(Type.GetType(nameSpace + tableName));
            string query = "SELECT * FROM " + tableName + " WHERE " + column + " = '" + value + "'";
            return _database.Query(map, query, obj).Cast<T>().Single();
        }

        public object GetTableRow(string tableName, string column, string value)
        {
            object[] obj = new object[] { };
            TableMapping map = new TableMapping(Type.GetType(nameSpace + tableName));
            string query = "SELECT * FROM " + tableName + " WHERE " + column + " = '" + value + "'";
            return _database.Query(map, query, obj).Single();
        }

        public object GetTableRow(string tableName, string column1, string value1, string column2, string value2)
        {
            object[] obj = new object[] { };
            TableMapping map = new TableMapping(Type.GetType(nameSpace + tableName));
            string query = "SELECT * FROM " + tableName + " WHERE " + column1 + " = '" + value1 + "' AND " + column2 + " = '" + value2 + "' LIMIT 1";
            return _database.Query(map, query, obj).Single();
        }

        public List<object> GetTableRows(string tableName, string column, string value)
        {
            object[] obj = new object[] { };
            TableMapping map = new TableMapping(Type.GetType(nameSpace + tableName));
            string query = "SELECT * FROM " + tableName + " WHERE " + column + " = '" + value + "'";
            return _database.Query(map, query, obj).ToList();
        }

        public List<T> GetTableRows<T>(string tableName, string column, string value)
        {
            object[] obj = new object[] { };
            TableMapping map = new TableMapping(Type.GetType(nameSpace + tableName));
            string query = "SELECT * FROM " + tableName + " WHERE " + column + " = '" + value + "'";
            return _database.Query(map, query, obj).Cast<T>().ToList();
        }

        public List<T> GetTableRows<T>(string tableName)
        {
            object[] obj = new object[] { };
            TableMapping map = new TableMapping(Type.GetType(nameSpace + tableName));
            string query = "SELECT * FROM " + tableName;
            return _database.Query(map, query, obj).Cast<T>().ToList();
        }

        #endregion Abstract Get
    }
}