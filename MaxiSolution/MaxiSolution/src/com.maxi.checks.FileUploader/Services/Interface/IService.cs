using Boz.Services.Contracts.Domain.ServiceAggregates;
using Boz.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Maxi.Services.SouthSide.SouthsideFile.Types;

namespace Maxi.Services.SouthSide.Services.Interface
{
    public interface IService
    {
        void SetConstantFields();
        void SetConstant();
        void StartProcess();
        void GetData();
        string CreateFile(FileWf objFile);
        void UploadFile(string fileName);
        void UpdateStatus(FileWf objFile, string fileName);
        void UpdateBunbles(string fileName, FileWf objFile);
    }
}
