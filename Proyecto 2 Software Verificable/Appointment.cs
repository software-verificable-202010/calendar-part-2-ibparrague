using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;

namespace Proyecto_2_Software_Verificable
{
    [Serializable]
    public class Appointment
    {
        public string title { get; set; }
        public string description { get; set; }
        public DateTime date { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }

        public Appointment() { }
        public Appointment(string title, string description, DateTime date, DateTime startTime, DateTime endTime)
        {
            this.title = title;
            this.description = description;
            this.date = date;
            this.startTime = startTime;
            this.endTime = endTime;
        }

        public DateTime GetRealStartTime()
        {
            return date.Date.AddHours(startTime.Hour).AddMinutes(startTime.Minute).AddSeconds(startTime.Second);
        }
        public DateTime GetRealEndTime()
        {
            return date.Date.AddHours(endTime.Hour).AddMinutes(endTime.Minute).AddSeconds(endTime.Second);
        }
        public bool isBetweenDates(DateTime initialDate, DateTime finalDate)
        {
            if (this.GetRealStartTime() >= initialDate && this.GetRealStartTime() <= finalDate)
                return true;
            else
                return false;
        }
    }
}
