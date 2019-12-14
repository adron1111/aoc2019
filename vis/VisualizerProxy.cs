using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace vis
{
    public static class Settings
    {
        public static int delay = 0;
    }

    public class Visualize : ClientBase<VisInterface>, IDisposable
    {
        static EventWaitHandle runningevent = new EventWaitHandle(false, EventResetMode.ManualReset, "VisualizerServiceRunning");
        static Visualize _proxy;
        static Visualize proxy
        {
            get
            {
                if (_proxy == null) {
                    _proxy = Visualize.Start();
                }
                return _proxy;
            }
        }

        private Visualize() :
                    base(new System.ServiceModel.Description.ServiceEndpoint(System.ServiceModel.Description.ContractDescription.GetContract(typeof(VisInterface)), 
                                                                       new System.ServiceModel.NetNamedPipeBinding(),
                                                                       new System.ServiceModel.EndpointAddress("net.pipe://localhost/VisualizerGui/VisualizerService")))
        {

        }

        private static Visualize Start()
        {
            if(!runningevent.WaitOne(0)) {
                Process p = new Process();
                p.StartInfo = new ProcessStartInfo {
                    FileName = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), @"..\..\..\visgui\bin\debug\visgui.exe"),
                    UseShellExecute = false
                };
                p.Start();
            }
            runningevent.WaitOne();
            return new Visualize();
        }

        public static int CreateMatrix()
        {
            return proxy.Channel.CreateMatrix();
        }

        public static void SetMatrixData(int matrix, int x, int y, string data)
        {
            proxy.Channel.SetMatrixData(matrix, x, y, data);
            if (Settings.delay != 0) {
                Thread.Sleep(Settings.delay);
            }
        }

        public static void SetMatrixRange(int matrix, int minx, int maxx, int miny, int maxy)
        {
            proxy.Channel.SetMatrixRange(matrix, minx, maxx, miny, maxy);
        }
    }

}
