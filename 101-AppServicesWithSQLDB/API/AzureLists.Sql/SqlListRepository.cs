using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace AzureLists.Sql
{
    public class SqlListRepository : Library.IListRepository
    {
        private readonly string connectionString;

        public SqlListRepository()
        {
            this.connectionString = WebConfigurationManager.ConnectionStrings["AzureListsDBConnectionString"].ConnectionString;
        }

        public async Task<IEnumerable<T>> Get<T>() where T : class, Library.IIdentifiable, new()
        {
            return await this.Get<T>(string.Empty);
        }

        public async Task<IEnumerable<T>> Get<T>(string id = null, bool? important = null, bool? completed = null) where T : class, Library.IIdentifiable, new()
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            string condition = this.BuildCondition<T>(id, important, completed, parameters);
            Type type = typeof(T);
            string query = type == typeof(Library.List) ?
                $"SELECT {this.BuildSelect(typeof(Library.List), typeof(Library.Task))} FROM List List LEFT JOIN Task Task ON List.Id = Task.ListId {condition}" :
                $"SELECT {this.BuildSelect(typeof(T))} FROM {type.Name} {condition}";

            Action<SqlDataReader, List<T>> action = this.GetTypeUnpacker<T>();
            return await this.ExecuteQuery<T>(query, action, string.IsNullOrWhiteSpace(condition) ? null : parameters);
        }       

        public async Task Create<T>(string listId, T item) where T : Library.IIdentifiable
        {
            await this.InsertOrUpdate<T>(listId, item);
        }

        public Task Delete<T>(string id) where T : Library.IIdentifiable
        {
            return Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(this.connectionString))
                {
                    connection.Open();
                    Type type = typeof(T);
                    string query = $"DELETE FROM {type.Name} WHERE id = @id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@id", SqlDbType.NVarChar, 50).Value = id;
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                    }
                }
            });
        }

        public async Task Update<T>(string id, T item) where T : Library.IIdentifiable
        {
            await this.InsertOrUpdate<T>(id, item, false);
        }

        private Func<Type, List<PropertyInfo>> getProperties = o => o.GetProperties().Where(x => x.PropertyType.IsPrimitive || x.PropertyType.IsValueType || x.PropertyType == typeof(string)).ToList();

        private Task InsertOrUpdate<T>(string id, T item, bool insert = true) where T : Library.IIdentifiable
        {
            return Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(this.connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand())
                    {
                        List<SqlParameter> parameters = new List<SqlParameter>();
                        parameters.Add(new SqlParameter("id", SqlDbType.NVarChar, 50) { Value = id });

                        command.Connection = connection;

                        if (typeof(T) == typeof(Library.List))
                        {
                            Library.List list = item as Library.List;
                            command.CommandText += this.BuildListQuery(list, parameters, insert);
                            if (list.Tasks != null) command.CommandText += this.BuildTaskQuery(parameters, insert, list.Tasks.ToArray());
                        }
                        else if (typeof(T) == typeof(Library.Task))
                        {
                            Library.Task task = item as Library.Task;
                            command.CommandText += this.BuildTaskQuery(parameters, insert, task);
                        }

                        command.Parameters.AddRange(parameters.ToArray());
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                    }
                }
            });
        } 

        private Action<SqlDataReader, List<T>> GetTypeUnpacker<T>() where T : class, Library.IIdentifiable, new()
        {
            Action<SqlDataReader, List<T>> action = null;
            if (typeof(T) == typeof(Library.List))
            {
                action = (r, l) => UnpackList(r, l as List<Library.List>);
            }
            else
            {
                action = (r, l) => UnpackTask(r, l as List<Library.Task>);
            }

            return action;
        }

        private Task<List<T>> ExecuteQuery<T>(string query, Action<SqlDataReader, List<T>> unpacker, List<SqlParameter> parameters = null) where T : Library.IIdentifiable
        {
            return Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(this.connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (parameters != null) command.Parameters.AddRange(parameters.ToArray());
                        SqlDataReader reader = command.ExecuteReader();
                        try
                        {
                            List<T> items = new List<T>();

                            while (reader.Read())
                            {
                                unpacker(reader, items);
                            }
                            return items;
                        }
                        catch (Exception exception)
                        {
                            // look at working with throwing certain type exceptions so the api tier can return a proper status code
                            throw exception;
                        }
                        finally
                        {
                            reader.Close();
                        }
                    }
                }
            });
        }

        private string BuildCondition<T>(string id, bool? important, bool? completed, List<SqlParameter> parameters)
        {
            bool whered = false;
            StringBuilder builder = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(id))
            {
                SqlParameter parameter = new SqlParameter("id", SqlDbType.NVarChar)
                {
                    Value = id
                };
                parameters.Add(parameter);
                builder.Append($"WHERE {typeof(T).Name}.Id = @id");
                whered = true;
            }

            if (important != null)
            {
                int n = important.Value ? 1 : 0;
                string condition = whered ? " AND" : "WHERE";
                whered = true;
                builder.Append($"{condition} Task.Important = {n}");
            }

            if (completed != null)
            {
                string condition = whered ? " AND" : "WHERE";                
                builder.Append(completed.Value ? $"{condition} Task.CompletedDate IS NOT NULL" : $"{condition} Task.CompletedDate IS NULL");
            }            

            return builder.ToString();
        }

        private string BuildSelect(params Type[] types)
        {
            StringBuilder builder = new StringBuilder();
            foreach (Type type in types)
            {
                this.getProperties(type).ForEach(x => builder.Append($"{type.Name}.{x.Name} AS {type.Name}{x.Name},"));
            }
            string selection = builder.ToString();
            selection = selection.Remove(selection.Length - 1, 1); // remove last comma
            return selection;
        }

        private static void UnpackList(SqlDataReader reader, List<Library.List> items)
        {
            string id = reader["ListId"].ToString();

            Library.List list = items.FirstOrDefault(x => x.Id == id) as Library.List;

            if (list == null)
            {
                list = new Library.List
                {
                    Id = id,
                    Name = reader["ListName"].ToString()
                };
                items.Add(list);
            }

            // if the list contains tasks then unpack them
            if (reader["TaskId"].GetType() == typeof(DBNull)) return;

            if (list.Tasks == null) list.Tasks = new List<Library.Task>();
            UnpackTask(reader, list.Tasks);
        }

        private static void UnpackTask(SqlDataReader reader, List<Library.Task> items)
        {
            string id = reader["TaskId"].ToString();

            Library.Task task = items.FirstOrDefault(x => x.Id == id) as Library.Task;

            if (task == null)
            {
                DateTime completedDate;
                bool completedDateFlag = DateTime.TryParse(reader["TaskCompletedDate"].ToString(), out completedDate);
                DateTime dueDate;
                bool dueDateFlag = DateTime.TryParse(reader["TaskDueDate"].ToString(), out dueDate);

                task = new Library.Task
                {
                    Id = reader["TaskId"].ToString(),
                    Title = reader["TaskTitle"].ToString(),
                    Notes = reader["TaskNotes"].ToString(),
                    Important = (bool)reader["TaskImportant"],
                    CompletedDate = completedDateFlag ? completedDate : default(DateTime?),
                    DueDate = dueDateFlag ? dueDate : default(DateTime?),
                };
                items.Add(task);
            }
        }

        private string BuildListQuery(Library.List list, List<SqlParameter> parameters, bool insert)
        {
            parameters.Add(new SqlParameter("name", SqlDbType.NVarChar, 50) { Value = list.Name });
            if (insert) return $"INSERT INTO List(Id, Name) VALUES (@id, @name);";
            return $"UPDATE List SET Name = @name WHERE Id = @id;";
        }

        private string BuildTaskQuery(List<SqlParameter> parameters, bool insert, params Library.Task[] tasks)
        {
            StringBuilder builder = new StringBuilder();
            for (int n = 0; n < tasks.Length; n++)
            {
                parameters.Add(new SqlParameter($"id{n}", SqlDbType.NVarChar, 50) { Value = tasks[n].Id });
                parameters.Add(new SqlParameter($"title{n}", SqlDbType.NVarChar, 75) { Value = tasks[n].Title });
                parameters.Add(new SqlParameter($"notes{n}", SqlDbType.NVarChar, 200) { Value = !string.IsNullOrWhiteSpace(tasks[n].Notes) ? (object)tasks[n].Notes : DBNull.Value});
                parameters.Add(new SqlParameter($"important{n}", SqlDbType.Bit) { Value = tasks[n].Important });
                parameters.Add(new SqlParameter($"completedDate{n}", SqlDbType.DateTime) { Value = tasks[n].CompletedDate != null ? (object)tasks[n].CompletedDate.Value : DBNull.Value });
                parameters.Add(new SqlParameter($"dueDate{n}", SqlDbType.DateTime) { Value = tasks[n].DueDate != null ? (object)tasks[n].DueDate.Value : DBNull.Value });

                if (insert)
                {
                    builder.Append($"INSERT INTO Task(Id, Title, Notes, Important, CompletedDate, DueDate, ListId) VALUES(@id{n}, @title{n}, @notes{n}, @important{n}, @completedDate{n}, @dueDate{n}, @id);");
                    continue;
                }                
                builder.Append($"UPDATE TASK SET Title = @title{n}, Notes = @notes{n}, Important = @important{n}, CompletedDate = @completedDate{n}, DueDate = @dueDate{n} WHERE Id = @id{n};");
                
            }

            return builder.ToString();
        }

    }
}