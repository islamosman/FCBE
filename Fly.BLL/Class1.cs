using Fly.DAL;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Fly.BLL
{
    public class ScotterRepo
    {
        protected FlyEntities db = new FlyEntities();
        public Vehicles Add(Vehicles model)
        {
            db.Vehicles.Add(model);
            db.SaveChanges();

            return model;
        }

        public List<Vehicles> GetAll()
        {
            return db.Vehicles.ToList();
        }

        public Vehicles GetOne(int idVar)
        {
            return db.Vehicles.FirstOrDefault(x=>x.Id == idVar);
        }
    }
}
