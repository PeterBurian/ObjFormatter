using ObjFormatter.Interfaces;
using ObjFormatter.ObjParser;
using System;
using System.IO;

namespace ObjFormatter
{
    public class ObjProcessor
    {
        private readonly IReader reader;
        private readonly IWriter writer;

        private readonly string postFix;

        public ObjProcessor(IReader reader, IWriter writer, string resultPostfix)
        {
            this.reader = reader;
            this.writer = writer;
            this.postFix = resultPostfix;
        }

        public void Process()
        {
            if (reader != null && writer != null)
            {
                string source = reader.Source;

                if (!String.IsNullOrEmpty(source))
                {
                    if (!String.IsNullOrEmpty(postFix))
                    {
                        if (File.Exists(source))
                        {
                            try
                            {
                                string target = GetTarget(source);

                                reader.Parse();

                                writer.SetCenter(reader.Center);
                                writer.SetPoints(reader.Points);
                                writer.SetNormals(reader.Normals);
                                writer.SetFaces(reader.Faces);

                                writer.CalculateDiffedVectors();
                                writer.Write(target);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Source file doesn't exist!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Postfix must a valid value!");
                    }
                }
                else
                {
                    Console.WriteLine("Source path mut be set!");
                }
            }
            else
            {
                Console.WriteLine("Reader and writer must be set!");
            }
        }

        private string GetTarget(string source)
        {
            string fileName = Path.GetFileNameWithoutExtension(source);
            string ext = Path.GetExtension(source);
            string dir = Path.GetDirectoryName(source);
            fileName += postFix;

            return Path.Combine(dir, fileName + ext);
        }
    }
}