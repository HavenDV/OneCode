using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace OneCode.VsExtension.Services
{
    [Export]
    public class ExceptionsService
    {
        public List<Exception> Exceptions { get; set; } = new List<Exception>();

        public void Add(Exception value)
        {
            Exceptions.Add(value);
        }
    }
}
