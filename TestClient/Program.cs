using ObjFormatter;
using ObjFormatter.Interfaces;
using ObjFormatter.ObjParser;
using ShapeReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //string source = @"f:\Development\SunMeData\Data\Env_Objs\Roof_CloudComp_withoutY_Invert.obj";
            string source = @"f:\Development\SunMeData\Data\Env_Objs\Roof_CloudComp_withoutY.obj";
            //string source = @"f:\Development\SunMeData\Data\Env_Objs\Roof_CloudComp_withY.obj";

            string shpSource = @"f:\Development\SunMeData\Data\tetoelemek_3D\Kemeny_3D.shp";

            IReader reader = new ObjReader(source);
            IWriter writer = new ObjWriter(source);

            ObjProcessor processor = new ObjProcessor(reader, writer, "_toUnity");
            processor.Process();

            ShpReader shpReader = new ShpReader(shpSource);
            shpReader.ReadShp();

            Console.WriteLine("Finished");
            Console.ReadKey();
        }
    }
}