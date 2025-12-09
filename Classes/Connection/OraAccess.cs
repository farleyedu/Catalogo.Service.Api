using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace Catalogo.Service.Api
{
    /// <summary>
    /// Contexto de Conexão com o banco de dados Oracle.
    /// </summary>
    /// <remarks>Leon Denis @ByteOn</remarks>
    public class OraAccess : IDisposable
    {
        #region Connection

        /// <summary>
        /// Auxiliar de Conexão.
        /// </summary>
        private OracleConnection ConnectionAux { get; set; }

        /// <summary>
        /// Auxiliar comando padrão Oracle.
        /// </summary>
        private OracleCommand CommandAux { get; set; }

        /// <summary>
        /// Conexão padrão Oracle.
        /// </summary>
        protected OracleConnection Connection
        {
            get
            {
                if (ConnectionAux == null || ConnectionAux.State == ConnectionState.Closed || ConnectionAux.State == ConnectionState.Broken)
                {
                    if (ConnectionAux != null)
                    {
                        ConnectionAux.Close();
                        ConnectionAux.Dispose();
                    }
                    ConnectionAux = new OracleConnection(Constants.Connection);
                    ConnectionAux.Open();
                }
                return ConnectionAux;
            }
        }

        /// <summary>
        /// Comando padrão Oracle.
        /// </summary>
        protected OracleCommand Command
        {
            get
            {
                if (this.CommandAux == null)
                    this.CommandAux = new OracleCommand() { Connection = this.Connection };
                else
                    this.CommandAux.Connection = this.Connection;

                if (this.Transaction != null)
                    this.CommandAux.Transaction = this.Transaction;

                return this.CommandAux;
            }
        }

        /// <summary>
        /// Transação oracle.
        /// </summary>
        protected OracleTransaction Transaction { get; set; }

        /// <summary>
        /// Cria uma transação oracle na conexão atual.
        /// </summary>
        /// <returns>Transação oracle.</returns>
        public OracleTransaction UseTransaction()
        {
            if (this.Transaction == null)
                this.Transaction = this.Connection.BeginTransaction();
            return this.Transaction;
        }

        /// <summary>
        /// Finaliza a transação oracle na conexão atual.
        /// </summary>
        /// <returns>Transação oracle.</returns>
        public void EndTransaction()
        {
            this.EndMe();
        }

        /// <summary>
        /// Finaliza todos os recursos utilizados.
        /// </summary>
        private void EndMe()
        {
            try
            {
                if (this.CommandAux != null)
                {
                    this.CommandAux.Dispose();
                    this.CommandAux = null;
                }
                if (this.Transaction != null)
                {
                    this.Transaction.Dispose();
                    this.Transaction = null;
                }
                if (this.ConnectionAux != null)
                {
                    this.ConnectionAux.Close();
                    this.ConnectionAux.Dispose();
                    this.ConnectionAux = null;
                }
            }
            catch
            {
                // TODO: Nothing.
            }
        }

        /// <summary>
        /// Finaliza todos os recursos utilizados.
        /// </summary>
        public void Dispose()
        {
            EndMe();
        }

        /// <summary>
        /// Finaliza todos os recursos utilizados pelo Garbage Collector.
        /// </summary>
        ~OraAccess()
        {
            EndMe();
        }

        #endregion Conection
    }

    /// <summary>
    /// Extensões para banco de dados oracle.
    /// </summary>
    /// <remarks>Leon Denis @ByteOn</remarks>
    public static class OraAccessExtensions
    {
        #region Usable

        /// <summary>
        /// Cria uma lista do tipo informado.
        /// </summary>
        /// <typeparam name="T">Tipo da lista.</typeparam>
        /// <param name="oraCommand">Comando de execução SQL oracle.</param>
        /// <param name="closeConnection">Fecha a conexão após o uso.</param>
        /// <returns>Lista do Tipo informado.</returns>
        public static async Task<List<T>> ExecuteList<T>(this OracleCommand oraCommand, bool closeConnection = true)
        {
            using IDataReader reader = await oraCommand.ExecuteReaderAsync();
            List<T> items = new();
            // IsSealed = Correção para identificar o tipo string que é considerado como não primitivo e como classe pelo .net
            if (typeof(T).IsPrimitive || !typeof(T).IsClass || typeof(T).IsSealed)
            {
                while (reader.Read())
                {
                    T item = default;
                    if (reader[0] is object value && value != null && value != DBNull.Value)
                    {
                        if (typeof(T) != value.GetType())
                        {
                            if (Nullable.GetUnderlyingType(typeof(T)) is Type underType && underType != null)
                                item = (T)Convert.ChangeType(value, underType);
                            else
                                item = (T)Convert.ChangeType(value, typeof(T));
                        }
                        else
                            item = (T)value;
                    }
                    items.Add(item);
                }
            }
            else
            {
                List<string> cols = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();
                PropertyInfo[] props = typeof(T).GetProperties();
                while (reader.Read())
                {
                    T item = Activator.CreateInstance<T>();
                    foreach (PropertyInfo prop in props)
                        if (cols.Contains(prop.Name) && prop.CanWrite)
                            if (reader[prop.Name] is object value && value != null && value != DBNull.Value)
                            {
                                if (prop.PropertyType != value.GetType())
                                {
                                    if (Nullable.GetUnderlyingType(prop.PropertyType) is Type underType && underType != null)
                                        prop.SetValue(item, Convert.ChangeType(value, underType));
                                    else
                                        prop.SetValue(item, Convert.ChangeType(value, prop.PropertyType));
                                }
                                else
                                    prop.SetValue(item, value);
                            }
                    items.Add(item);
                }
            }
            if (closeConnection) oraCommand.Connection.Close();
            return items;
        }

        /// <summary>
        /// Define a query ao comando Oracle.
        /// (Limpa o conteúdo antigo do comando)
        /// </summary>
        /// <param name="oraCommand">Comando de execução SQL oracle.</param>
        /// <param name="cmdText">Instrução SQL.</param>
        /// <param name="arrayBindCount">Quantidade de itens no array de parâmentros.</param>
        /// <param name="cmdType">Tipo da Instrução SQL.</param>
        /// <returns>Commando Oracle.</returns>
        public static OracleCommand UseQuery(this OracleCommand oraCommand, string cmdText, int? arrayBindCount = null, CommandType cmdType = CommandType.Text)
        {
            if (oraCommand.Parameters.Count > 0)
                oraCommand.Parameters.Clear();
            oraCommand.CommandText = cmdText;
            oraCommand.CommandType = cmdType;
            oraCommand.ArrayBindCount = arrayBindCount ?? default;
            return oraCommand;
        }

        /// <summary>
        /// Adiciona um parâmetro no comando Oracle.
        /// </summary>
        /// <param name="oraCommand">Comando de execução SQL oracle.</param>
        /// <param name="paramName">Nome do parâmetro.</param>
        /// <param name="paramValue">Valor do parâmetro.</param>
        /// <returns>Commando Oracle.</returns>
        public static OracleCommand AddParameter(this OracleCommand oraCommand, string paramName, object paramValue)
        {
            OracleParameter param = oraCommand.CreateParameter();
            param.ParameterName = paramName;
            param.Value = paramValue ?? DBNull.Value;
            oraCommand.Parameters.Add(param);
            return oraCommand;
        }

        /// <summary>
        /// Adiciona uma sequêcia de parâmetros no comando Oracle.
        /// </summary>
        /// <param name="oraCommand">Comando de execução SQL oracle.</param>
        /// <param name="paramName">Nome do parâmetro.</param>
        /// <param name="paramValues">Valores do parâmetro.</param>
        /// <returns>Commando Oracle.</returns>
        public static OracleCommand AddParameters<T>(this OracleCommand oraCommand, string paramName, IEnumerable<T> paramValues)
        {
            foreach (T paramValue in paramValues)
                oraCommand.AddParameter(paramName, paramValue);
            return oraCommand;
        }

        /// <summary>
        /// Adiciona um parâmetro no comando Oracle com uma condição.
        /// </summary>
        /// <param name="oraCommand">Comando de execução SQL oracle.</param>
        /// <param name="paramName">Nome do parâmetro.</param>
        /// <param name="paramValue">Valor do parâmetro.</param>
        /// <param name="coditionToAdd">Condição para adicionar.</param>
        /// <returns>Commando Oracle.</returns>
        public static OracleCommand AddParameter(this OracleCommand oraCommand, string paramName, object paramValue, bool coditionToAdd)
        {
            if (coditionToAdd) oraCommand.AddParameter(paramName, paramValue);
            return oraCommand;
        }

        /// <summary>
        /// Adiciona um parâmetro no comando Oracle repedido.
        /// </summary>
        /// <param name="oraCommand">Comando de execução SQL oracle.</param>
        /// <param name="paramName">Nome do parâmetro.</param>
        /// <param name="paramValue">Valor do parâmetro.</param>
        /// <param name="bindCount">Quantidade de vezes.</param>
        /// <returns>Commando Oracle.</returns>
        public static OracleCommand AddParameter(this OracleCommand oraCommand, string paramName, object paramValue, int bindCount)
        {
            for (int i = 0; i < bindCount; i++)
                oraCommand.AddParameter(paramName, paramValue);
            return oraCommand;
        }

        /// <summary>
        /// Adiciona um parâmetro no comando Oracle repedido com uma condição.
        /// </summary>
        /// <param name="oraCommand">Comando de execução SQL oracle.</param>
        /// <param name="paramName">Nome do parâmetro.</param>
        /// <param name="paramValue">Valor do parâmetro.</param>
        /// <param name="bindCount">Quantidade de vezes.</param>
        /// <param name="coditionToAdd">Condição para adicionar.</param>
        /// <returns>Commando Oracle.</returns>
        public static OracleCommand AddParameter(this OracleCommand oraCommand, string paramName, object paramValue, int bindCount, bool coditionToAdd)
        {
            if (coditionToAdd)
            {
                for (int i = 0; i < bindCount; i++)
                    oraCommand.AddParameter(paramName, paramValue);
            }
            return oraCommand;
        }

        /// <summary>
        /// Adiciona um array de parâmetros no comando Oracle.
        /// </summary>
        /// <typeparam name="T">Tipo do parâmetro.</typeparam>
        /// <param name="oraCommand">Comando de execução SQL oracle.</param>
        /// <param name="paramValue">Valor do parâmetro.</param>
        /// <param name="statusValue">Status do parâmetro.</param>
        /// <returns>Commando Oracle.</returns>
        public static OracleCommand AddParameterArray<T>(this OracleCommand oraCommand, IEnumerable<T> paramValue, OracleParameterStatus[] statusValue = null)
        {
            OracleParameter param = oraCommand.CreateParameter();
            if (paramValue == null)
                param.Value = DBNull.Value;
            else
            {
                if (statusValue != null)
                    param.ArrayBindStatus = statusValue;
                param.Value = paramValue.ToArray();
            }
            oraCommand.Parameters.Add(param);
            return oraCommand;
        }

        /// <summary>
        /// Adiciona um modelo no comando Oracle.
        /// </summary>
        /// <typeparam name="T">Tipo do modelo.</typeparam>
        /// <param name="oraCommand">Comando de execução SQL oracle.</param>
        /// <param name="model">Modelo.</param>
        /// <returns>Commando Oracle.</returns>
        public static OracleCommand AddModel<T>(this OracleCommand oraCommand, T model)
        {
            Regex regex = new(@"(?<param>:\w*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection matches = regex.Matches(oraCommand.CommandText);
            Type type = model?.GetType();
            if (type != null)
                foreach (Match match in matches)
                {
                    PropertyInfo prop = type.GetProperty(match.Value.Remove(0, 1));
                    if (prop != null)
                        oraCommand.AddParameter(prop.Name, prop.GetValue(model));
                }
            return oraCommand;
        }

        /// <summary>
        /// Adiciona uma lista modelo no comando Oracle. (para insert's multiplos)
        /// </summary>
        /// <typeparam name="T">Tipo do modelo.</typeparam>
        /// <param name="oraCommand">Comando de execução SQL oracle.</param>
        /// <param name="list">Lista modelo.</param>
        /// <returns>Commando Oracle.</returns>
        public static OracleCommand AddModelList<T>(this OracleCommand oraCommand, IEnumerable<T> list)
        {
            Regex regex = new(@"(?<param>:\w*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Type type = typeof(T);
            if (type != null)
                foreach (T model in list)
                {
                    MatchCollection matches = regex.Matches(oraCommand.CommandText);
                    foreach (Match match in matches)
                    {
                        PropertyInfo prop = type.GetProperty(match.Value.Remove(0, 1));
                        if (prop != null)
                            oraCommand.AddParameter(prop.Name, prop.GetValue(model));
                    }
                }
            return oraCommand;
        }

        /// <summary>
        /// Executa uma declaração SQL em um objeto de conexão.
        /// </summary>
        /// <param name="oraCommand">Comando de execução SQL oracle.</param>
        /// <param name="closeConnection">Fecha a conexão após o uso.</param>
        /// <returns>Numero de linhas afetadas.</returns>
        public static async Task<int> ExecNonQuery(this OracleCommand oraCommand, bool closeConnection = true)
        {
            int result = await oraCommand.ExecuteNonQueryAsync();
            if (closeConnection) oraCommand.Connection.Close();
            return result;
        }

        /// <summary>
        /// Obtem a primeira coluna da primeira linha do resultado select.
        /// </summary>
        /// <param name="oraCommand">Comando de execução SQL oracle.</param>
        /// <param name="closeConnection">Fecha a conexão após o uso.</param>
        /// <returns>Primeira linha da primeira coluna.</returns>
        public static async Task<object> ExecScalar(this OracleCommand oraCommand, bool closeConnection = true)
        {
            object result = await oraCommand.ExecuteScalarAsync();
            if (closeConnection) oraCommand.Connection.Close();
            return result;
        }

        /// <summary>
        /// Obtem a primeira coluna da primeira linha do resultado select tipado.
        /// </summary>
        /// <param name="oraCommand">Comando de execução SQL oracle.</param>
        /// <param name="closeConnection">Fecha a conexão após o uso.</param>
        /// <returns>Primeira linha da primeira coluna.</returns>
        public static async Task<T> ExecScalar<T>(this OracleCommand oraCommand, bool closeConnection = true)
        {
            object scalar = await oraCommand.ExecuteScalarAsync();
            T result = default;
            if (closeConnection) oraCommand.Connection.Close();
            if (scalar != null)
            {
                if (Nullable.GetUnderlyingType(typeof(T)) is Type underType && underType != null)
                    result = (T)Convert.ChangeType(scalar, underType);
                else
                    result = (T)Convert.ChangeType(scalar, typeof(T));
            }
            return result;
        }

        #endregion
    }
}

