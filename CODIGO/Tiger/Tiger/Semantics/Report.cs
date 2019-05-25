using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiger.Semantics
{
    public class Report
    {
        public List<Error> errors { get; private set; }

        public Report() { errors = new List<Error>(); }

        public void Add(int line, int column, string message)
        {
            errors.Add(new Error(line, column, message));
        }
    }

    public class Error {
        public int line { get; private set; }
        public int column { get; private set; }
        public string message { get; private set; }

        public Error(int line, int column, string message)
        {
            this.line = line;
            this.message = message;
            this.column = column;
        }
    }
}
