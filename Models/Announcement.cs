using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nizams.Models
{

    /// <summary>
    /// Holds the Announcement Object
    /// </summary>
    public class Announcement
    {
       
        private DateTime m_timestamp;

        public DateTime TimeStamp
        { get
            { return m_timestamp;
            }


            set
            {
                if (value == null)
                {
                    m_timestamp = DateTime.Now;
                }
                else

                {
                    m_timestamp = value;
                }
            }
        }
        public string Text { get; set; }
        public string AreaName { get; set; }
        public string PostedBy { get; set; }
    }

    /// <summary>
    /// Holds the Object to Lock Objects to control waiting and notify user when result available
    /// </summary>
    public class  LockControl
    {
       public Object LockObject { get; set; }
       public Boolean IsLocked { get; set; }
        public string AreaName { get; set; }
        public DateTime LockDateTime { get; set; }
        public LockControl()
        {
            
            LockObject = new object();
            IsLocked = false;
            LockDateTime = DateTime.Now;
            
        }
    }

}
