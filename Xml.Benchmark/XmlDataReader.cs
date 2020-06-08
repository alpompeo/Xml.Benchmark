using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;


namespace Xml.Benchmark
{
    public class XmlDataReaderEventArgs : EventArgs
    {
        public IDictionary<string, object> Values { get; set; }
    }

    public class XmlParameters
    {
        public Type NodeType { get; set; }
        public string NodeName { get; set; }
        public string AttributeName { get; set; }

        public XmlParameters(string nodeName, Type type, string attributeName = "")
        {
            NodeType = type;
            NodeName = nodeName;
            AttributeName = attributeName;
        }
    }

    public class XmlDataReader : IDataReader
    {
        private readonly System.Xml.XmlReader _reader;
        private IDictionary<string, object> _values;
        private readonly List<XmlParameters> _parameters;
        private int _line;
        public event EventHandler<XmlDataReaderEventArgs> Changed;
        private XmlDataReaderEventArgs _eventArgs = new XmlDataReaderEventArgs();

        private DataTable SchemaTable { get; set; }

        public XmlDataReader(string filename, List<XmlParameters> parameters)
        {
            var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore };
            _reader = System.Xml.XmlReader.Create(filename, settings);
            _parameters = parameters;
            _values = new Dictionary<string, object>();
            SchemaTable = new DataTable();
            _line = 0;

            foreach (var col in parameters)
            {
                var fieldName = !string.IsNullOrEmpty(col.AttributeName) ? col.AttributeName : col.NodeName;

                _values.Add(fieldName, null);
                SchemaTable.Columns.Add(fieldName, col.NodeType);
            }
        }

        public bool GetBoolean(int i) => bool.Parse((string)_values.ElementAt(i).Value);
        
        public byte GetByte(int i) => byte.Parse((string)_values.ElementAt(i).Value);

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i) => char.Parse((string)_values.ElementAt(i).Value);
       
        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i) => _values.ElementAt(i).Value.GetType().ToString();
       
        public DateTime GetDateTime(int i) => DateTime.Parse((string)_values.ElementAt(i).Value);
       
        public decimal GetDecimal(int i) => int.Parse((string)_values.ElementAt(i).Value);
        
        public double GetDouble(int i) => double.Parse((string)_values.ElementAt(i).Value);

        public Type GetFieldType(int i) => _values.ElementAt(i).Value.GetType();

        public float GetFloat(int i) => float.Parse((string)_values.ElementAt(i).Value);
       
        public Guid GetGuid(int i) => Guid.Parse((string)_values.ElementAt(i).Value);
       
        public short GetInt16(int i) => short.Parse((string)_values.ElementAt(i).Value);
       
        public int GetInt32(int i) => int.Parse((string)_values.ElementAt(i).Value);
        
        public long GetInt64(int i) => long.Parse((string)_values.ElementAt(i).Value);
      
        public string GetName(int i) => _values.ElementAt(i).Key;

        public int GetOrdinal(string name)
        {
            var ordinal = Array.IndexOf(_values.Keys.ToArray(), name);

            if (ordinal == -1)
                throw new InvalidOperationException($"Unknown parameter name: {name}");

            return ordinal;
        }

        public string GetString(int i) => (string)_values.ElementAt(i).Value;

        public object GetValue(int i) => _values.ElementAt(i).Value;

        public int GetValues(object[] values)
        {
            if (_values.Count > 0)
            {
                Array.Copy(_values.ToArray(), values, _values.Count);
                return _values.Count;
            }
            return 0;
        }

        public bool IsDBNull(int i) => _values.ElementAt(i).Value == DBNull.Value;

        public int FieldCount => _values.Count;

        public object this[int i] => _values.ElementAt(i);

        public object this[string name] => GetValue(GetOrdinal(name));

        public void Dispose()
        {
            _reader.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Close() => this.Dispose();

        public DataTable GetSchemaTable() => SchemaTable;

        public bool NextResult() => _reader.Read();

        public bool Read()
        {
            bool isEof;

            try
            {
                do
                {
                    isEof = _reader.Read();
                    Depth = _reader.Depth;

                    foreach (var parameter in _parameters)
                    {
                        if (_reader.Name != parameter.NodeName)
                        {
                            HasValue = false;
                            continue;
                        }

                        var fieldName = !string.IsNullOrEmpty(parameter.AttributeName) ? parameter.AttributeName : parameter.NodeName;
                        var value = !string.IsNullOrEmpty(parameter.AttributeName) ? _reader.GetAttribute(parameter.AttributeName) : _reader.ReadInnerXml();

                        _values[fieldName] = value;

                        HasValue = true;
                    }

                } while (!HasValue && isEof);

                if (HasValue)
                {
                    _line++;
                    _eventArgs.Values = _values;
                    OnChanged(_eventArgs);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Line: {_line} {Environment.NewLine} " +
                                          $"StackTrace: {ex.StackTrace} {Environment.NewLine} " +
                                          $"Message: {ex.Message}");
            }

            IsClosed = isEof;
            RecordsAffected = _line;
           
            return isEof;
        }

        public bool HasValue { get; set; }
        public int Depth { get; set; }
        public bool IsClosed { get; set; }
        public int RecordsAffected { get; set; }

        protected virtual void OnChanged(XmlDataReaderEventArgs e)
        {
            var handler = Changed;
            handler?.Invoke(this, e);
        }
    }
}
