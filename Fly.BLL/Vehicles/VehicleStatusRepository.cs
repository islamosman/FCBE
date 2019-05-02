using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fly.DomainModel.Helper;
using Fly.DomainModel;
using Fly.Resources;

namespace Fly.BLL
{
    public class VehicleStatusRepository : MainRepository<VehicleStatus>
    {
        public override RequestResponse AddUpdate(VehicleStatus entity)
        {
            Validate(entity);

            if (responseObj.ErrorMessages.Count <= 0)
            {
                if (entity.VehicleId > 0)
                {
                    Attach(entity);
                }
                else
                {
                    Add(entity);
                }
                Save();
                responseObj.Messages.Add("succss", OperationLP.SuccessMsg);
            }
            return responseObj;
        }

        public bool UpdateInRide(int vId, bool inRide = true, bool needPrepare = false)
        {
            VehicleStatus vModel = GetById(vId);
            vModel.InRide = inRide;
            vModel.InService = needPrepare;

            AddUpdate(vModel);

            return true;
        }

        public bool UpdateCordinates(string qrId, decimal batteryV, string lng, string lat)
        {
            VehicleStatus vModel = _objectSet.FirstOrDefault(x=>x.VehicleQR == qrId);
            if (vModel != null)
            {
                vModel.LongV = lng;
                vModel.LatV = lat;
                vModel.BatteryStatus = batteryV;

                AddUpdate(vModel);

                return vModel.InRide; 
            }

            return false;
        }

        public override VehicleStatus GetById(int id)
        {
            return _objectSet.FirstOrDefault(x => x.VehicleId == id);
        }

        public RequestResponse GetByIdService(int id)
        {
            VehicleStatus dataModel= _objectSet.FirstOrDefault(x => x.VehicleId == id); 

            //VehicleStatus returnModel = new VehicleStatus()
            //{
            //    LatV=dataModel.LatV,
            //    LongV=dataModel.LongV
            //};

            //responseObj.ReturnedObject = returnModel;
            responseObj.Messages.Add("lat", dataModel.LatV);
            responseObj.Messages.Add("long", dataModel.LongV);

            return responseObj;
        }

        public VehicleStatus GetByQR(string qrId)
        {
            VehicleStatus dataModel = _objectSet.FirstOrDefault(x => x.VehicleQR == qrId);

            return dataModel;
        }

        public override void Validate(VehicleStatus entity)
        {

        }
    }
}
