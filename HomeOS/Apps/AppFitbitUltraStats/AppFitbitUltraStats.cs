using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace AppFitbitUltraStats
{
    [System.AddIn.AddIn("AppFitbitUltraStats")]
    public class AppFitbitUltraStats :  ModuleBase
    {
        bool showWindow = true;
        DummyWindow dummyWindow;

        // the dictionary for other Fitbit Ultra ports in the system
        Dictionary<View.VPort, View.VCapability> otherFitbitUltraPorts;

        public override void Start()
        {
            logger.Log("Starting {0}", ToString());
 
            // instantiate the list of other ports that we are interested in
            otherFitbitUltraPorts = new Dictionary<View.VPort, View.VCapability>();

            // get the list of current ports from the platform
            IList<View.VPort> allPortsList = GetAllPortsFromPlatform();

            if (allPortsList != null)
                ProcessAllPortsList(allPortsList);

            if (showWindow)
                ShowWindow();

            Work();
        }

        public void ShowWindow()
        {
            SafeThread thread = new SafeThread(
                delegate()
                {
                    dummyWindow = new DummyWindow(this, logger, moduleInfo.FriendlyName());
                    dummyWindow.ShowDialog();
                },
                "Launching window for " + ToString(),
                logger);

            thread.SetApartmentState(System.Threading.ApartmentState.STA);
            thread.Start();
        }

        public override void Stop()
        {
            if (dummyWindow != null) 
                 dummyWindow.Invoke(new Action(delegate() { dummyWindow.Close(); }));

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
                    if (dummyWindow != null)
                        dummyWindow.Invoke(new Action(delegate() { dummyWindow.OtherPortsLog(String.Format("{0} active Fitbit Ultra ports", otherFitbitUltraPorts.Count)); }));

                    List<View.VPort> ports = otherFitbitUltraPorts.Keys.ToList();
                    foreach (View.VPort port in ports)
                    {
                        RequestCapabilities(port);

                        if (otherFitbitUltraPorts[port] != null)
                        {
                            SendGetActiveScoreRequest(port, otherFitbitUltraPorts[port], counter);
                            SendGetCaloriesOutRequest(port, otherFitbitUltraPorts[port], counter);
                            SendGetDistanceRequest(port, otherFitbitUltraPorts[port], counter);
                            SendGetStepsRequest(port, otherFitbitUltraPorts[port], counter);
                            SendGetTotalMinutesAsleepRequest(port, otherFitbitUltraPorts[port], counter);
                            SendGetTotalSleepRecordspRequest(port, otherFitbitUltraPorts[port], counter);
                            SendGetTotalTimeInBedRequest(port, otherFitbitUltraPorts[port], counter);
                        }
                    }
                }

                System.Threading.Thread.Sleep(1 * 30 * 1000); // sleep 30 seconds
            }
        }

        #region Driver Methods

        public void SendGetActiveScoreRequest(View.VPort port, View.VCapability capability, int counter)
        {
            IList<View.VParamType> retVals;

            try
            {
                IList<View.VParamType> parameters = new List<View.VParamType>();
                parameters.Add(new ParamType(ParamType.SimpleType.binary, "", DateTime.Today, "activityDate"));

                retVals = port.Invoke(RoleFitbitUltra.RoleName, RoleFitbitUltra.OpGetActiveScoreName, parameters, ControlPort, capability, ControlPortCapability);
            }
            catch (Exception e)
            {
                logger.Log("Error while calling getActiveScore request: {0}", e.ToString());
                return;
            }

            LogMessageToWindow(port, RoleFitbitUltra.OpGetActiveScoreName, retVals);
        }

        public void SendGetCaloriesOutRequest(View.VPort port, View.VCapability capability, int counter)
        {
            IList<View.VParamType> retVals;

            try
            {
                IList<View.VParamType> parameters = new List<View.VParamType>();
                parameters.Add(new ParamType(ParamType.SimpleType.binary, "", DateTime.Today, "activityDate"));

                retVals = port.Invoke(RoleFitbitUltra.RoleName, RoleFitbitUltra.OpGetCaloriesOutName, parameters, ControlPort, capability, ControlPortCapability);
            }
            catch (Exception e)
            {
                logger.Log("Error while calling getCaloriesOut request: {0}", e.ToString());
                return;
            }

            LogMessageToWindow(port, RoleFitbitUltra.OpGetCaloriesOutName, retVals);
        }

        public void SendGetDistanceRequest(View.VPort port, View.VCapability capability, int counter)
        {
            IList<View.VParamType> retVals;

            try
            {
                IList<View.VParamType> parameters = new List<View.VParamType>();
                parameters.Add(new ParamType(ParamType.SimpleType.binary, "", DateTime.Today, "activityDate"));

                retVals = port.Invoke(RoleFitbitUltra.RoleName, RoleFitbitUltra.OpGetDistanceName, parameters, ControlPort, capability, ControlPortCapability);
            }
            catch (Exception e)
            {
                logger.Log("Error while calling getDistance request: {0}", e.ToString());
                return;
            }

            LogMessageToWindow(port, RoleFitbitUltra.OpGetDistanceName, retVals);
        }

        public void SendGetStepsRequest(View.VPort port, View.VCapability capability, int counter)
        {
            IList<View.VParamType> retVals;

            try
            {
                IList<View.VParamType> parameters = new List<View.VParamType>();
                parameters.Add(new ParamType(ParamType.SimpleType.binary, "", DateTime.Today, "activityDate"));

                retVals = port.Invoke(RoleFitbitUltra.RoleName, RoleFitbitUltra.OpGetStepsName, parameters, ControlPort, capability, ControlPortCapability);
            }
            catch (Exception e)
            {
                logger.Log("Error while calling getSteps request: {0}", e.ToString());
                return;
            }

            LogMessageToWindow(port, RoleFitbitUltra.OpGetStepsName, retVals);
        }

        public void SendGetTotalMinutesAsleepRequest(View.VPort port, View.VCapability capability, int counter)
        {
            IList<View.VParamType> retVals;

            try
            {
                IList<View.VParamType> parameters = new List<View.VParamType>();
                parameters.Add(new ParamType(ParamType.SimpleType.binary, "", DateTime.Today, "activityDate"));

                retVals = port.Invoke(RoleFitbitUltra.RoleName, RoleFitbitUltra.OpGetTotalMinutesAsleep, parameters, ControlPort, capability, ControlPortCapability);
            }
            catch (Exception e)
            {
                logger.Log("Error while calling getTotalMinutesAsleep request: {0}", e.ToString());
                return;
            }

            LogMessageToWindow(port, RoleFitbitUltra.OpGetTotalMinutesAsleep, retVals);
        }

        public void SendGetTotalSleepRecordspRequest(View.VPort port, View.VCapability capability, int counter)
        {
            IList<View.VParamType> retVals;

            try
            {
                IList<View.VParamType> parameters = new List<View.VParamType>();
                parameters.Add(new ParamType(ParamType.SimpleType.binary, "", DateTime.Today, "activityDate"));

                retVals = port.Invoke(RoleFitbitUltra.RoleName, RoleFitbitUltra.OpGetTotalSleepRecords, parameters, ControlPort, capability, ControlPortCapability);
            }
            catch (Exception e)
            {
                logger.Log("Error while calling getTotalSleepRecords request: {0}", e.ToString());
                return;
            }

            LogMessageToWindow(port, RoleFitbitUltra.OpGetTotalSleepRecords, retVals);
        }

        public void SendGetTotalTimeInBedRequest(View.VPort port, View.VCapability capability, int counter)
        {
            IList<View.VParamType> retVals;

            try
            {
                IList<View.VParamType> parameters = new List<View.VParamType>();
                parameters.Add(new ParamType(ParamType.SimpleType.binary, "", DateTime.Today, "activityDate"));

                retVals = port.Invoke(RoleFitbitUltra.RoleName, RoleFitbitUltra.OpGetTotalTimeInBed, parameters, ControlPort, capability, ControlPortCapability);
            }
            catch (Exception e)
            {
                logger.Log("Error while calling getTotalTimeInBed request: {0}", e.ToString());
                return;
            }

            LogMessageToWindow(port, RoleFitbitUltra.OpGetTotalTimeInBed, retVals);
        }

        public void LogMessageToWindow(View.VPort port, string operationName, IList<View.VParamType> retVals)
        {
            string message = "";

            if (retVals[0].Maintype() != (int)ParamType.SimpleType.error)
            {
                string result =  retVals[0].Value().ToString();
                message = String.Format("{0} success to {1} result = {2}", operationName, port.ToString(), result);
            }
            else
            {
                message = String.Format("{0} failure to {1}", operationName, port.ToString());
            }

            logger.Log("{0} {1}", this.ToString(), message);

            if (dummyWindow != null)
                dummyWindow.Invoke(new Action(delegate() { dummyWindow.ConsoleLog(message); }));
        }

        #endregion

        #region Port Management

        public void RequestCapabilities(View.VPort port)
        {
            // if we do not have a capability, lets try to get one
            if (otherFitbitUltraPorts[port] == null)
            {
                otherFitbitUltraPorts[port] = GetCapability(port, Globals.UserSystem);

                // if we just got the capability, subscribe to the port
                if (otherFitbitUltraPorts[port] != null)
                {
                    port.Subscribe(RoleFitbitUltra.RoleName, RoleFitbitUltra.OpGetActiveScoreName, ControlPort, otherFitbitUltraPorts[port], ControlPortCapability);
                    port.Subscribe(RoleFitbitUltra.RoleName, RoleFitbitUltra.OpGetCaloriesOutName, ControlPort, otherFitbitUltraPorts[port], ControlPortCapability);
                    port.Subscribe(RoleFitbitUltra.RoleName, RoleFitbitUltra.OpGetDistanceName, ControlPort, otherFitbitUltraPorts[port], ControlPortCapability);
                    port.Subscribe(RoleFitbitUltra.RoleName, RoleFitbitUltra.OpGetStepsName, ControlPort, otherFitbitUltraPorts[port], ControlPortCapability);
                    port.Subscribe(RoleFitbitUltra.RoleName, RoleFitbitUltra.OpGetTotalMinutesAsleep, ControlPort, otherFitbitUltraPorts[port], ControlPortCapability);
                    port.Subscribe(RoleFitbitUltra.RoleName, RoleFitbitUltra.OpGetTotalSleepRecords, ControlPort, otherFitbitUltraPorts[port], ControlPortCapability);
                    port.Subscribe(RoleFitbitUltra.RoleName, RoleFitbitUltra.OpGetTotalTimeInBed, ControlPort, otherFitbitUltraPorts[port], ControlPortCapability);
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
                    // we'll get the capability the first time we send a message
                    otherFitbitUltraPorts[port] = null;
                    logger.Log("{0} added port {1}", this.ToString(), port.ToString());

                    if (dummyWindow != null)
                        dummyWindow.Invoke(new Action(delegate() { dummyWindow.ConsoleLog("added port " + port.ToString()); }));
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

                    if (dummyWindow != null)
                        dummyWindow.Invoke(new Action(delegate() { dummyWindow.ConsoleLog("removed port " + port.ToString()); }));
                }
            }
        }

        #endregion
    }
}