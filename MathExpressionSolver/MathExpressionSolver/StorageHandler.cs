using MathExpressionSolver.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace MathExpressionSolver
{
    class StorageHandler<T>
    {
        public Dictionary<string, T> Variables { get; private set; } = new Dictionary<string, T>();
        public Dictionary<string, IToken<T>> Functions { get; private set; } = new Dictionary<string, IToken<T>>();

        public void AddVariable(string variableName, T value)
        {
            Variables[variableName] = value;
            SaveVariables();
        }

        public void AddFunction (string funcName, IToken<T> value)
        {
            Functions[funcName] = value;
        }

        public StorageHandler()
        {
            LoadVariables();
        }

        public void SaveVariables()
        {
            List<SerializVariables<T>> toBeSerialized = new List<SerializVariables<T>>();
            foreach (KeyValuePair<string, T> entry in Variables)
            {
                toBeSerialized.Add(new SerializVariables<T>()
                {
                    Name = entry.Key,
                    Value = entry.Value,
                });
            }


            using (StreamWriter fileWriter = new StreamWriter("variables.xml"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<SerializVariables<T>>));
                serializer.Serialize(fileWriter, toBeSerialized);
            }
        }

        public void LoadVariables()
        {
            if(File.Exists("variables.xml"))
            {
                List<SerializVariables<T>> deserializedVariables;
                using (StreamReader fileReader = new StreamReader("variables.xml"))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<SerializVariables<T>>));
                    deserializedVariables = (List<SerializVariables<T>>)serializer.Deserialize(fileReader);
                }

                if(deserializedVariables != null)
                {
                    foreach(SerializVariables<T> deseralizedVariable in deserializedVariables)
                    {
                        Variables[deseralizedVariable.Name] = deseralizedVariable.Value;
                    }
                }
            }
        }
    }

    public class SerializVariables<T>
    {
        public string Name { get; set; }
        public T Value { get; set; }
    }
}
