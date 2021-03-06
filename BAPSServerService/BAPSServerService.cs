using System.ServiceProcess;

namespace BAPSServerService
{
    public partial class BAPSServerService : ServiceBase
    {
        public BAPSServerService()
        {
            InitializeComponent();
#if DEBUG
            this.ServiceName = "BAPS (DEBUG) Server Service";
#else
			this.ServiceName = "BAPS Server Service";
#endif
        }

        protected override void OnStart(string[] args)
        {
            //System.Diagnostics.Debugger.Launch();
            BAPSServerAssembly.Utility.start();
        }

        protected override void OnStop()
        {
            BAPSServerAssembly.Utility.stop();
        }

        protected override void OnShutdown()
        {
            OnStop();
        }
    }
}