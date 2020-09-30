using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Type type = args.GetType();
            type.GetMethods();
            //type.
            foreach (var p in type.GetProperties())
            {
                Console.WriteLine(p.Name);
            }
        }
    }
}
