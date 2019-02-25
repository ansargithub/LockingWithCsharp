using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nizams.Models;
using System.Threading;
//API  for Nizam's Message Announcement Public System

namespace Nizams.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        /// <summary>
        /// Holds the Static List of Announcements in Memory
        /// </summary>
        public static List<Announcement> InMemoryList = new List<Announcement>();
        /// <summary>
        /// TimeOut Period when new area requested is not posted by any user;
        /// </summary>
        private static int milliseconds = 20000;
        /// <summary>
        /// Holds the Locks Information on the Area Names Not Exist in Memory by many Users
        /// </summary>
        public static List<LockControl> WaitLocks = new List< LockControl>();


   /// <summary>
   /// An Api Method to Add the Annoucement
   /// </summary>
   /// <param name="myobj"></param>
        [HttpPost]
        public async void Post([FromBody] Announcement myobj)
        {
            await Task.Run(()=>
            {

                if (myobj != null)
                {
                  
                        if (myobj.TimeStamp == null || myobj.TimeStamp == DateTime.MinValue) { myobj.TimeStamp = DateTime.Now; }
                        InMemoryList.Add(myobj);
                    
                    if (WaitLocks.Count>0  )
                    {
                          
                           for( int i=0; i< WaitLocks.Count;i++)
                          
                            {

                            if (WaitLocks[i] != null)
                            {
                                if (WaitLocks[i].AreaName == myobj.AreaName)
                                {
                                    lock (WaitLocks[i].LockObject)
                                    {
                                        Monitor.Pulse(WaitLocks[i].LockObject);

                                        WaitLocks[i].IsLocked = false;

                                    }
                                }
                            }
                            }
                      }


                       
                    
                    
                }
            });


        }


      

        
    
/// <summary>
/// A API Method which List all the Areas
/// </summary>
/// <param name="AreaName"></param>
/// <param name="DateTimeStamp"></param>
/// <returns></returns>

        [Route("AreaAnnouncementsListByAreaName")]
        [HttpGet]
        public async Task<List<Announcement>> GetAnnouncementsByAreaTimeLocknew(string AreaName, DateTime? DateTimeStamp = null)
        {
            List<Announcement> result = new List<Announcement>();

            await Task.Run<List<Announcement>>(() =>
            {
                LockControl mylock = null;
                try
                {


                    if (DateTimeStamp == null) DateTimeStamp = DateTime.Now.AddDays(-1);
                    result = InMemoryList.Where(myarea => myarea.AreaName == AreaName && myarea.TimeStamp >= DateTimeStamp).ToList();

                    if (result.Count == 0)
                    {
                        mylock = new LockControl();
                        mylock.AreaName = AreaName;
                        mylock.IsLocked = true;
                        WaitLocks.Add(mylock);


                        Monitor.TryEnter(mylock.LockObject, 500);
                        {


                            Monitor.Wait(mylock.LockObject, milliseconds);
                        }

                        if (mylock.IsLocked == false)
                        {
                            result = InMemoryList.Where(myarea => myarea.AreaName == AreaName && myarea.TimeStamp >= DateTimeStamp).ToList();
                        }
                    }
                    return result;



                }


                finally
                {
                    if (mylock != null)
                    {
                        mylock.IsLocked = false;
                        Monitor.Exit(mylock.LockObject);
                        

                    }


                }

            });

            return result;




        }
        /// <summary>
        /// A Api Method which List All Announcements in the Memory 
        /// </summary>
        /// <returns>List of Announcements</returns>
        [Route("AnnouncementsList")]
        [HttpGet]
        public async Task<List<Announcement>> GetAnnouncementsList()
        {
                List<Announcement> mylist = InMemoryList;
            
            await Task.Run<List<Announcement>>(() =>
            {
                

                return mylist;
            });
            return mylist;
        }
    }
}
