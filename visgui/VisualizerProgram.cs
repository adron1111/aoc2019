using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceModel;
using System.Threading;

namespace visgui
{
    static class VisualizerProgram
    {
        static public VisualizerApplicationContext context;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            context = new VisualizerApplicationContext();
            Application.Run(context);
        }
    }

    class VisualizerApplicationContext: ApplicationContext
    {
        ServiceHost servicehost;
        VisualizerService service;
        EventWaitHandle runningevent;

        public int runningforms = 0;
        public SynchronizationContext GuiContext;

        public VisualizerApplicationContext()
        {
            var uri = new Uri("net.pipe://localhost/VisualizerGui");
            service = new VisualizerService();
            servicehost = new ServiceHost(service, uri);
            servicehost.Description.Behaviors.Find<System.ServiceModel.Description.ServiceDebugBehavior>().IncludeExceptionDetailInFaults = true;
            servicehost.AddServiceEndpoint(typeof(vis.VisInterface), new NetNamedPipeBinding  {ReceiveTimeout = TimeSpan.MaxValue}, "VisualizerService");
            servicehost.Open();
            new Control();
            GuiContext = SynchronizationContext.Current;
            runningevent = new EventWaitHandle(false, EventResetMode.ManualReset, "VisualizerServiceRunning");
            runningevent.Set();
        }
    }
}
