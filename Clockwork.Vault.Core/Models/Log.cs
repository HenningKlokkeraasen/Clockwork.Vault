using System.Collections.Generic;

namespace Clockwork.Vault.Core.Models
{
    public class Log
    {
        public string Title { get; set; }
        public IList<string> Messages { get; set; } = new List<string>();
        public IList<string> Statistics { get; set; } = new List<string>();
    }
}