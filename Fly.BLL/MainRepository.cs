using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity.Core.Objects;
using System.Configuration;

namespace Fly.BLL
{
    interface MainIRepositories<T> where T : class
    {
        #region Properties
        ObjectContext Context { get; }

        #endregion

        #region	Methods

        T GetById(int id);

        void Attach(T entity);
        IEnumerable<T> GetAll();
        IEnumerable<T> Query(Expression<Func<T, bool>> filter);
        void Add(T entity);
        void Remove(T entity);

        #endregion
    }
    public abstract class MainRepository<T> : IDisposable, MainIRepositories<T> where T : class
    {
        #region Members

        protected IObjectSet<T> _objectSet;
        protected ObjectContext _context;
        //protected log4net.ILog logger;
        //#endregion
        #endregion

        #region Ctor

        public MainRepository(ObjectContext context)
        {
            _context = context;
            _objectSet = context.CreateObjectSet<T>();

            //Init Logger
            //logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        public MainRepository()
            : this(new ObjectContext(ConfigurationManager.ConnectionStrings["Main_DBEntities"].ConnectionString))
        {
        }

        #endregion

        #region BackLogIRepository<T> Members

        public ObjectContext Context
        {
            get
            {
                return _context;
            }
        }

        public IEnumerable<T> GetAll()
        {
            return _objectSet;
        }

        public abstract T GetById(int id);


        public IEnumerable<T> Query(Expression<Func<T, bool>> filter)
        {

            return _objectSet.Where(filter);
        }

        public void Add(T entity)
        {
            _objectSet.AddObject(entity);

        }


        public void Attach(T entity)
        {
            _objectSet.Attach(entity);
            Context.ObjectStateManager.ChangeObjectState(entity, System.Data.Entity.EntityState.Modified);
        }
        public void DevChangeState(T entity)
        {
            Context.ObjectStateManager.ChangeObjectState(entity, System.Data.Entity.EntityState.Modified);
        }

        public virtual void Attach(T entity, params object[] items)
        {
            _objectSet.Attach(entity);
            Context.ObjectStateManager.ChangeObjectState(entity, System.Data.Entity.EntityState.Modified);

            foreach (T item in items)
            {
                Context.ObjectStateManager.ChangeObjectState(item, System.Data.Entity.EntityState.Unchanged);
            }
            _context.SaveChanges();

        }
        public void Remove(T entity)
        {
            _objectSet.Attach(entity);
            Context.ObjectStateManager.ChangeObjectState(entity, System.Data.Entity.EntityState.Deleted);
            _objectSet.DeleteObject(entity);

        }
        public void Save()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion

    }

}
