using Fly.DAL;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Fly.BLL
{
    public class ScotterRepo
    {
        protected FlyEntities db = new FlyEntities();
        public Scotter Add(Scotter model)
        {
            db.Scotter.Add(model);
            db.SaveChanges();

            return model;
        }

        public List<Scotter> GetAll()
        {
            return db.Scotter.ToList();
        }

        public Scotter GetOne(int idVar)
        {
            return db.Scotter.FirstOrDefault(x=>x.Id == idVar);
        }
    }
}
