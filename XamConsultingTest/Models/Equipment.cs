using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace XamConsultingTest.Models
{
    public class Equipment
    {
        public int Id { get; set; }
        public string Name  { get;set; }
        public string Comments  { get;set; }
        public string PhotoUri { get; set; }
    }
}
