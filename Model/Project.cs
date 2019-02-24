using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace ScottLane.SurgeonV2.Model
{
    [Serializable]
    public class Project
    {
        public string Name { get; set; }
       
        public List<Batch> Batches { get; set; }

        public List<Connection> Connections { get; set; }

        public Project()
        {
            Batches = new List<Batch>();
            Connections = new List<Connection>();
        }

        public void Save(string fileName)
        {
            //XmlSerializer serializer = new XmlSerializer(typeof(Project));
            BinaryFormatter serializer = new BinaryFormatter();
            FileStream stream = new FileStream(fileName, FileMode.Create);
            serializer.Serialize(stream, this);
            stream.Close();
        }

        public static Project Load(string fileName)
        {
            //XmlSerializer serializer = new XmlSerializer(typeof(Project));
            BinaryFormatter serializer = new BinaryFormatter();
            FileStream stream = new FileStream(fileName, FileMode.Open);
            Project project = (Project)serializer.Deserialize(stream);
            stream.Close();

            return project;
        }
    }
}
