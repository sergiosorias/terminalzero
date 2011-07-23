using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ZeroCommonClasses.Entities
{
    public class EntityValidationResult
    {
        public bool IsValid { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
