using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Collections.Concurrent;

namespace visgui
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    class VisualizerService : vis.VisInterface
    {
        ConcurrentDictionary<int, MatrixVisualizer> matrices = new ConcurrentDictionary<int, MatrixVisualizer>();
        int max = 0;


        public int CreateMatrix()
        {
            MatrixVisualizer v = null;
            VisualizerProgram.context.GuiContext.Send(_ =>
            {
                v = new MatrixVisualizer();
                v.Text = max.ToString();
                v.Show();
            }, null);
            matrices.TryAdd(max, v);
            return max++;
        }

        public void SetMatrixData(int matrix, int x, int y, string data)
        {
            matrices[matrix].SetMatrixData(x, y, data);
        }

        public void SetMatrixRange(int matrix, int minx, int maxx, int miny, int maxy)
        {
            matrices[matrix].SetMatrixRange(minx, maxx, miny, maxy);
        }
    }

    
}
