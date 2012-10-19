using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace AppFitbitUltraRunning
{
    [System.AddIn.AddIn("AppFitbitUltraRunning")]
    public class AppFitbitUltraRunning : ModuleBase
    {
        LogWindow logWindow;

        Dictionary<View.VPort, View.VCapability> otherFitbitUltraPorts;

        bool currentPresence = false;

        public override void Start()
        {
            logger.Log("Starting {0}", ToString());
 
            otherFitbitUltraPorts = new Dictionary<View.VPort, View.VCapability>();

            IList<View.VPort> allPortsList = GetAllPortsFromPlatform();

            if (allPortsList != null)
                ProcessAllPortsList(allPortsList);

            ShowWindow();

            Work();
        }

        public void ShowWindow()
        {
            SafeThread thread = new SafeThread(
                delegate()
                {
                    logWindow = new LogWindow(this, logger, moduleInfo.FriendlyName());
                    logWindow.ShowDialog();
                },
                "Launching window for " + ToString(),
                logger);

            thread.SetApartmentState(System.Threading.ApartmentState.STA);
            thread.Start();
        }

        public override void Stop()
        {
            if (logWindow != null) 
                 logWindow.Invoke(new Action(delegate() { logWindow.Close(); }));

            Finished();
        }

        public void Work()
        {
            int counter = 0;
            while (true)
            {
                counter++;

                lock (this)
                {
                    if (logWindow != null)
                        logWindow.Invoke(new Action(delegate() { logWindow.OtherPortsLog(String.Format("{0} active Fitbit Ultra ports", otherFitbitUltraPorts.Count)); }));

                    List<View.VPort> ports = otherFitbitUltraPorts.Keys.ToList();
                    foreach (View.VPort port in ports)
                    {
                        RequestCapabilities(port);

                        if (otherFitbitUltraPorts[port] != null)
                        {
                            SendGetDevicePresenceMethod(port, otherFitbitUltraPorts[port], counter);
                            SendGetHasRecentActivityMethod(port, otherFitbitUltraPorts[port], counter);
                        }
                    }
                }

                System.Threading.Thread.Sleep(1 * 30 * 1000); // sleep 30 seconds
            }
        }

        #region Driver Methods

        public void SendGetDevicePresenceMethod(View.VPort port, View.VCapability capability, int counter)
        {
            IList<View.VParamType> retVals;

            try
            {
                IList<View.VParamType> parameters = new List<View.VParamType>();

                retVals = port.Invoke(RoleFitbitUltra.RoleName, RoleFitbitUltra.OpGetDevicePresence, parameters, ControlPort, capability, ControlPortCapability);
            }
            catch (Exception e)
            {
                logger.Log("Error while calling getDevicePresence request: {0}", e.ToString());
                return;
            }

            bool result = false;

            string message = "";
            if (retVals[0].Maintype() != (int)ParamType.SimpleType.error)
            {
                result = Convert.ToBoolean(retVals[0].Value());
                message = String.Format("{0} success to {1} result = {2}", RoleFitbitUltra.OpGetDevicePresence, port.ToString(), result);
            }
            else
            {
                message = String.Format("{0} failure to {1}", RoleFitbitUltra.OpGetDevicePresence, port.ToString());
                return;
            }

            LogMessageToWindow(message);

            currentPresence = result;
        }

        public void SendGetHasRecentActivityMethod(View.VPort port, View.VCapability capability, int counter)
        {
            IList<View.VParamType> retVals;

            try
            {
                IList<View.VParamType> parameters = new List<View.VParamType>();
                
                retVals = port.Invoke(RoleFitbitUltra.RoleName, RoleFitbitUltra.OpGetHasRecentActivity, parameters, ControlPort, capability, ControlPortCapability);
            }
            catch (Exception e)
            {
                logger.Log("Error while calling getHasRecentActivity request: {0}", e.ToString());
                return;
            }

            bool result = false;

            string message = "";
            if (retVals[0].Maintype() != (int)ParamType.SimpleType.error)
            {
                result = Convert.ToBoolean(retVals[0].Value());
                message = String.Format("{0} success to {1} result = {2}", RoleFitbitUltra.OpGetHasRecentActivity, port.ToString(), result);
            }
            else
            {
                message = String.Format("{0} failure to {1}", RoleFitbitUltra.OpGetHasRecentActivity, port.ToString());
                return;
            }

            LogMessageToWindow(message);

            if (currentPresence == true &&
                result == true)
            {
                // lower thermostat temperature

                // turn on fans

                LogMessageToWindow("User has returned from a run, thermostat has been lowered and fans have been turned on.");
            }
        }

        public void LogMessageToWindow(string message)
        {
            logger.Log("{0} {1}", this.ToString(), message);

            if (logWindow != null)
                logWindow.Invoke(new Action(delegate() { logWindow.ConsoleLog(message); }));
        }

        #endregion

        #region Port Management

        public void RequestCapabilities(View.VPort port)
        {
            if (otherFitbitUltraPorts[port] == null)
            {
                otherFitbitUltraPorts[port] = GetCapability(port, Globals.UserSystem);

                if (otherFitbitUltraPorts[port] != null)
                {
                    port.Subscribe(RoleFitbitUltra.RoleName, RoleFitbitUltra.OpGetDevicePresence, ControlPort, otherFitbitUltraPorts[port], ControlPortCapability);
                }
            }
        }

        public void ProcessAllPortsList(IList<View.VPort> portList)
        {
            foreach (View.VPort port in portList)
            {
                PortRegistered(port);
            }
        }

        public override void PortRegistered(View.VPort port)
        {
            logger.Log("{0} got registeration notification for {1}", ToString(), port.ToString());

            lock (this)
            {
                if (!otherFitbitUltraPorts.ContainsKey(port) && Role.ContainsRole(port, RoleFitbitUltra.RoleName) && !IsMyPort(port))
                {
                    otherFitbitUltraPorts[port] = null;
                    logger.Log("{0} added port {1}", this.ToString(), port.ToString());

                    if (logWindow != null)
                        logWindow.Invoke(new Action(delegate() { logWindow.ConsoleLog("added port " + port.ToString()); }));
                }
            }
        }

        public override void PortDeregistered(View.VPort port)
        {
            lock (this)
            {
                if (otherFitbitUltraPorts.ContainsKey(port))
                {
                    otherFitbitUltraPorts.Remove(port);
                    logger.Log("{0} deregistered port {1}", this.ToString(), port.GetInfo().ModuleFacingName());

                    if (logWindow != null)
                        logWindow.Invoke(new Action(delegate() { logWindow.ConsoleLog("removed port " + port.ToString()); }));
                }
            }
        }

        #endregion
    }
}