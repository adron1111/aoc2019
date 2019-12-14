using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace vis
{
    [ServiceContract]
    public interface VisInterface
    {
        [OperationContract]
        int CreateMatrix();
        [OperationContract]
        void SetMatrixData(int matrix, int x, int y, string data);
        [OperationContract]
        void SetMatrixRange(int matrix, int minx, int maxx, int miny, int maxy);
    }
}
