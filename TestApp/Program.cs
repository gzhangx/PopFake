﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    class Address
    {
        public int id;
        public string name;

    }
    class Program : Veda.Tests.RandMock.SWriter
    {
        public string Name { get; set;}
        public int id;
        public string test1;
        public List<Address> addresses { get; set; }
        public decimal? testDec { get; set; }
        public Guid testGuid { get; set; }
        public int intVal { get; set; }
        static void Main(string[] args)
        {

            var mock = new Veda.Tests.RandMock().Generate<Program>(null, new Program());
            Console.WriteLine(mock.testGuid);
        }

        public void Write(object s)
        {
            Console.Write(s);
        }

        public void WriteLine(string s)
        {
            Console.WriteLine(s);
        }
    }
}
