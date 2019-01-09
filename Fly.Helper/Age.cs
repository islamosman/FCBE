using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.Helper
{
    public class Age
    {
        public int Years;
        public int Months;
        public int Days;

        public Age() { }
        public Age(DateTime Bday)
        {
            this.Count(Bday);
        }

        public Age(DateTime Bday, DateTime Cday)
        {
            this.Count(Bday, Cday);
        }

        public Age Count(DateTime Bday)
        {
            return this.Count(Bday, DateTime.Today);
        }

        public Age Count(DateTime Bday, DateTime Cday)
        {
            if ((Cday.Year - Bday.Year) > 0 ||
                (((Cday.Year - Bday.Year) == 0) && ((Bday.Month < Cday.Month) ||
                  ((Bday.Month == Cday.Month) && (Bday.Day <= Cday.Day)))))
            {
                int DaysInBdayMonth = DateTime.DaysInMonth(Bday.Year, Bday.Month);
                int DaysRemain = Cday.Day + (DaysInBdayMonth - Bday.Day);

                if (Cday.Month > Bday.Month)
                {
                    this.Years = Cday.Year - Bday.Year;
                    this.Months = Cday.Month - (Bday.Month + 1) + Math.Abs(DaysRemain / DaysInBdayMonth);
                    this.Days = (DaysRemain % DaysInBdayMonth + DaysInBdayMonth) % DaysInBdayMonth;
                }
                else if (Cday.Month == Bday.Month)
                {
                    if (Cday.Day >= Bday.Day)
                    {
                        this.Years = Cday.Year - Bday.Year;
                        this.Months = 0;
                        this.Days = Cday.Day - Bday.Day;
                    }
                    else
                    {
                        this.Years = (Cday.Year - 1) - Bday.Year;
                        this.Months = 11;
                        this.Days = DateTime.DaysInMonth(Bday.Year, Bday.Month) - (Bday.Day - Cday.Day);
                    }
                }
                else
                {
                    this.Years = (Cday.Year - 1) - Bday.Year;
                    this.Months = Cday.Month + (11 - Bday.Month) + Math.Abs(DaysRemain / DaysInBdayMonth);
                    this.Days = (DaysRemain % DaysInBdayMonth + DaysInBdayMonth) % DaysInBdayMonth;
                }
            }
            else
            {
                throw new ArgumentException("Birthday date must be earlier than current date");
            }
            return this;
        }


        public Age CountNullable(DateTime? Bday, DateTime? Cday)
        {
            if ((Cday.Value.Year - Bday.Value.Year) > 0 ||
                (((Cday.Value.Year - Bday.Value.Year) == 0) && ((Bday.Value.Month < Cday.Value.Month) ||
                  ((Bday.Value.Month == Cday.Value.Month) && (Bday.Value.Day <= Cday.Value.Day)))))
            {
                int DaysInBdayMonth = DateTime.DaysInMonth(Bday.Value.Year, Bday.Value.Month);
                int DaysRemain = Cday.Value.Day + (DaysInBdayMonth - Bday.Value.Day);

                if (Cday.Value.Month > Bday.Value.Month)
                {
                    this.Years = Cday.Value.Year - Bday.Value.Year;
                    this.Months = Cday.Value.Month - (Bday.Value.Month + 1) + Math.Abs(DaysRemain / DaysInBdayMonth);
                    this.Days = (DaysRemain % DaysInBdayMonth + DaysInBdayMonth) % DaysInBdayMonth;
                }
                else if (Cday.Value.Month == Bday.Value.Month)
                {
                    if (Cday.Value.Day >= Bday.Value.Day)
                    {
                        this.Years = Cday.Value.Year - Bday.Value.Year;
                        this.Months = 0;
                        this.Days = Cday.Value.Day - Bday.Value.Day;
                    }
                    else
                    {
                        this.Years = (Cday.Value.Year - 1) - Bday.Value.Year;
                        this.Months = 11;
                        this.Days = DateTime.DaysInMonth(Bday.Value.Year, Bday.Value.Month) - (Bday.Value.Day - Cday.Value.Day);
                    }
                }
                else
                {
                    this.Years = (Cday.Value.Year - 1) - Bday.Value.Year;
                    this.Months = Cday.Value.Month + (11 - Bday.Value.Month) + Math.Abs(DaysRemain / DaysInBdayMonth);
                    this.Days = (DaysRemain % DaysInBdayMonth + DaysInBdayMonth) % DaysInBdayMonth;
                }
            }
            else
            {
                throw new ArgumentException("Birthday date must be earlier than current date");
            }
            return this;
        }


    }

}
